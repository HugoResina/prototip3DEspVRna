using System;
using System.Collections;
using UnityEngine;
using Vosk;

public class NewSTT : MonoBehaviour
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

    // NUEVO: flag para evitar reentrada durante el loop de procesamiento
    private bool _isInsideProcessingLoop = false;
    // NUEVO: acumula si hay que invocar el evento DESPUÉS del loop
    private bool _pendingTurnOff = false;
    #endregion

    public static event Action TurnOffWalkie;

    private void OnEnable()
    {
        SetMicrophone();
        UIManager.InterPerToggleMicrophone += ToggleMicrophone;
    }

    private void OnDisable()
    {
        // NUEVO: esperamos a que el loop termine antes de limpiar
        if (_isInsideProcessingLoop)
        {
            // Marcamos para que se limpie al salir del loop en el mismo frame
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

    
        if (_voskRecognizer != null)
        {
            _voskRecognizer.Dispose();
            _voskRecognizer = null;
        }
        if (_voskModel != null)
        {
            _voskModel.Dispose();
            _voskModel = null;
        }
    }

    private void ToggleMicrophone()
    {
        if (!string.IsNullOrEmpty(_micDeviceName) && Microphone.IsRecording(_micDeviceName))
        {
            EndMicrophone();
        }
        else
        {
            StartMicrophone();
        }
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

    private string GetRecordResult()
    {
        if (!_isProcessing) return "";
        if (_micClip == null || _voskRecognizer == null) return "";

        int micCurrentPos = Microphone.GetPosition(_micDeviceName);
        if (micCurrentPos < 0 || micCurrentPos == _micLastSamplePos) return "";

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

            int startPos = _micLastSamplePos;
            if (startPos + thisChunk > _micClip.samples)
                thisChunk = _micClip.samples - startPos;

            _micClip.GetData(floatData, startPos);

            short[] shortData = new short[thisChunk];
            for (int i = 0; i < thisChunk; i++)
            {
                float f = Mathf.Clamp(floatData[i], -1f, 1f);
                shortData[i] = (short)(f * short.MaxValue);
            }

            byte[] bytes = new byte[thisChunk * 2];
            Buffer.BlockCopy(shortData, 0, bytes, 0, bytes.Length);

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

        if (_pendingTurnOff)
        {
            _pendingTurnOff = false;
            UnsetMicrophone();
            UIManager.InterPerToggleMicrophone -= ToggleMicrophone;
        }

        return result;
    }
}