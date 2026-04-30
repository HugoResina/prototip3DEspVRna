using System;
using System.Collections;
using UnityEngine;
using Vosk;

/// <summary>
/// Captura àudio del micròfon i el transcriu en temps real mitjançant Vosk (offline).
/// Quan es detecta text finalitzat, l'acumula al camp d'input de la UI i dispara <see cref="TurnOffWalkie"/>.
/// </summary>
public class STT : MonoBehaviour
{
    [Serializable]
    class RecognizerResult
    {
        public string text;
    }

    public static bool Recording = false;

    [Header("General Settings")]
    [SerializeField] private int _sampleRate = 16000;

    #region Vosk Variables
    [Header("Vosk Settings")]
    [SerializeField] private string _relativeVoskModelPath = "./Assets/VoskModels/vosk-model-small-ca-0.4";

    private Model _voskModel;
    private VoskRecognizer _voskRecognizer;
    private string _voskModelPath = string.Empty;
    #endregion

    #region Microphone Variables
    [Header("Microphone Settings")]
    [SerializeField] private int _micClipLength = 10;

    private const int MAX_CHUNCK_SIZE = 4096;

    private string _micDeviceName = string.Empty;
    private AudioClip _micClip;
    private int _micLastSamplePos = 0;
    private bool _isProcessing = false;

    // Evita reentrada si OnDisable s'executa mentre GetRecordResult encara itera
    private bool _isInsideProcessingLoop = false;
    // Si OnDisable arriba durant el loop, difereix el cleanup fins que el loop acabi
    private bool _pendingTurnOff = false;
    #endregion

    /// <summary>Disparat quan Vosk retorna un resultat final, per indicar que el walkie es pot tancar.</summary>
    public static event Action TurnOffWalkie;

    private void OnEnable()
    {
        SetMicrophone();
        UIManager.InterPerToggleMicrophone += ToggleMicrophone;
    }

    private void OnDisable()
    {
        if (_isInsideProcessingLoop)
        {
            // No podem netejar ara: el loop encara usa _voskRecognizer i _micClip.
            // Marquem el flag perquè GetRecordResult faci el cleanup en sortir.
            _pendingTurnOff = true;
            return;
        }
        UnsetMicrophone();
        UIManager.InterPerToggleMicrophone -= ToggleMicrophone;
    }

    private void Awake()
    {
        _voskModelPath = System.IO.Path.GetFullPath(_relativeVoskModelPath);
        if (string.IsNullOrEmpty(_voskModelPath)) { Debug.LogError("No s'ha trobat el model de Vosk."); return; }

        _voskModel = new Model(_voskModelPath);
        _voskRecognizer = new VoskRecognizer(_voskModel, _sampleRate);

        InteractablePersonEvents.UpdateInputFieldText(string.Empty);
    }

    private void Update()
    {
        if (!Recording) return;

        string result = GetRecordResult();

        if (!string.IsNullOrEmpty(result))
        {
            InteractablePersonEvents.UpdateInputFieldText(
                UIManager.Instance.InterPerInputFieldText + " " + result);

            TurnOffWalkie?.Invoke();
        }
    }

    private void SetMicrophone()
    {
        _isProcessing = true;

        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No s'han trobat micròfons ara, reintentant...");
            StartCoroutine(RetrySetMicrophone());
            return;
        }

        if (string.IsNullOrEmpty(_micDeviceName))
            _micDeviceName = Microphone.devices[0];

        StartMicrophone();
    }

    /// <summary>
    /// Reintenta trobar un micròfon disponible de forma periòdica.
    /// Útil en plataformes on els dispositius d'àudio poden trigar a inicialitzar-se.
    /// </summary>
    private IEnumerator RetrySetMicrophone(int maxRetries = 10, float waitSeconds = 0.75f)
    {
        int attempts = 0;

        while (attempts < maxRetries)
        {
            yield return new WaitForSeconds(waitSeconds);

            if (Microphone.devices.Length > 0)
            {
                Debug.Log($"Micròfon trobat després de {attempts + 1} intent(s).");

                if (string.IsNullOrEmpty(_micDeviceName))
                    _micDeviceName = Microphone.devices[0];

                StartMicrophone();
                yield break;
            }

            attempts++;
            Debug.LogWarning($"Reintent {attempts}/{maxRetries}: encara no hi ha micròfons.");
        }

        Debug.LogError("No s'ha pogut trobar cap micròfon després de tots els intents.");
    }

    private void UnsetMicrophone()
    {
        _isProcessing = false;
        EndMicrophone();

        _voskRecognizer?.Dispose();
        _voskRecognizer = null;
        _voskModel?.Dispose();
        _voskModel = null;
    }

    private void ToggleMicrophone()
    {
        if (!string.IsNullOrEmpty(_micDeviceName) && Microphone.IsRecording(_micDeviceName))
            EndMicrophone();
        else
            StartMicrophone();
    }

    private void StartMicrophone()
    {
        _micClip = Microphone.Start(_micDeviceName, true, _micClipLength, _sampleRate);
        if (_micClip == null) { Debug.LogError("No s'ha pogut iniciar el micròfon."); return; }

        Recording = true;
        Debug.Log($"Gravant des de: '{_micDeviceName}'");
    }

    private void EndMicrophone()
    {
        if (!string.IsNullOrEmpty(_micDeviceName) && Microphone.IsRecording(_micDeviceName))
        {
            Microphone.End(_micDeviceName);
            Recording = false;
            Debug.Log("Fi de la gravació");
        }
    }

    /// <summary>
    /// Llegeix les mostres noves del <see cref="_micClip"/> circular des de l'última posició
    /// processada, les converteix a PCM16 i les passa a Vosk en chunks de màxim <see cref="MAX_CHUNCK_SIZE"/>.
    /// </summary>
    /// <returns>
    /// El text transcrit acumulat dels resultats finals de Vosk, o <see cref="string.Empty"/> si no n'hi ha.
    /// </returns>
    /// <remarks>
    /// La conversió float→short es fa amb Clamp(-1,1) × <see cref="short.MaxValue"/> per evitar clipping.
    /// El wrapping del buffer circular es gestiona amb mòdul sobre <c>_micClip.samples</c>.
    /// </remarks>
    private string GetRecordResult()
    {
        if (!_isProcessing) return "";
        if (_micClip == null || _voskRecognizer == null) return "";

        int micCurrentPos = Microphone.GetPosition(_micDeviceName);
        if (micCurrentPos < 0 || micCurrentPos == _micLastSamplePos) return "";

        // Calcula quantes mostres noves hi ha, tenint en compte el wrap del buffer circular
        int samplesToRead = (micCurrentPos > _micLastSamplePos)
            ? micCurrentPos - _micLastSamplePos
            : (_micClip.samples - _micLastSamplePos) + micCurrentPos;

        string result = string.Empty;

        _isInsideProcessingLoop = true;

        while (samplesToRead > 0)
        {
            if (!_isProcessing || _voskRecognizer == null) break;

            int thisChunk = Mathf.Min(samplesToRead, MAX_CHUNCK_SIZE);
            float[] floatData = new float[thisChunk];

            // Ajust per no sobrepassar el final del buffer circular
            int startPos = _micLastSamplePos;
            if (startPos + thisChunk > _micClip.samples)
                thisChunk = _micClip.samples - startPos;

            _micClip.GetData(floatData, startPos);

            // Conversió float[] → PCM16 (bytes) que Vosk espera
            short[] shortData = new short[thisChunk];
            for (int i = 0; i < thisChunk; i++)
            {
                float f = Mathf.Clamp(floatData[i], -1f, 1f);
                shortData[i] = (short)(f * short.MaxValue);
            }

            byte[] bytes = new byte[thisChunk * 2];
            Buffer.BlockCopy(shortData, 0, bytes, 0, bytes.Length);

            // AcceptWaveform retorna true quan Vosk considera que hi ha un resultat final (silenci detectat)
            if (_voskRecognizer.AcceptWaveform(bytes, bytes.Length))
            {
                string json = _voskRecognizer.Result();
                RecognizerResult recoResult = JsonUtility.FromJson<RecognizerResult>(json);

                if (recoResult != null && !string.IsNullOrEmpty(recoResult.text))
                {
                    result += recoResult.text + " ";
                    Debug.Log("Microphone Record Result: " + recoResult.text);
                }
            }

            _micLastSamplePos = (_micLastSamplePos + thisChunk) % _micClip.samples;
            samplesToRead -= thisChunk;
        }

        _isInsideProcessingLoop = false;

        // Cleanup diferit: OnDisable va arribar mentre érem dins el loop
        if (_pendingTurnOff)
        {
            _pendingTurnOff = false;
            UnsetMicrophone();
            UIManager.InterPerToggleMicrophone -= ToggleMicrophone;
        }

        return result;
    }
}