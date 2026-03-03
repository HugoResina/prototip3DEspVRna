using System;
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
    #endregion

    private void OnEnable()
    {
        //SetMicrophone();
        UIManager.InterPerToggleMicrophone += ToggleMicrophone;
    }

    private void OnDisable()
    {
        UnsetMicrophone();
        UIManager.InterPerToggleMicrophone -= ToggleMicrophone;
    }

    private void Awake()
    {
        #region Vosk Initialize
        _voskModelPath = System.IO.Path.GetFullPath(_relativeVoskModelPath);

        if (string.IsNullOrEmpty(_voskModelPath)) { Debug.LogError("No s'ha trobat el model de Vosk."); return; }

        _voskModel = new Model(_voskModelPath);
        _voskRecognizer = new VoskRecognizer(_voskModel, _sampleRate);
        #endregion

        UIManager.Instance.InterPerInputFieldText = "";
    }

    private void Update()
    {
        UIManager.Instance.InterPerWarningTextState = Recording;
        
        //Debug.Log(Recording);
        if (Recording)
        {
            //UIManager.Instance.InterPerInputFieldText = GetRecordResult();

            string result = GetRecordResult();
            if (!string.IsNullOrEmpty(result))
            {
                UIManager.Instance.InterPerInputFieldText = result;
            }
        }
    }

    private void SetMicrophone()
    {
        //try
        //{



            if (Microphone.devices.Length == 0) { Debug.LogError("No s'han trobat micròfons disponibles."); return; }
            if (string.IsNullOrEmpty(_micDeviceName)) _micDeviceName = Microphone.devices[0];

            StartMicrophone();
        //}
        //catch(Exception e)
        //{
            Debug.Log("asdf");
        //}
    }

    private void UnsetMicrophone()
    {
        EndMicrophone();

        _voskRecognizer?.Dispose();
        _voskModel?.Dispose();
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

        if (_micClip == null)
        {
            Debug.LogError("No s'ha pogut iniciar el micròfon.");
            return;
        }
        Recording = true;

        Debug.Log($"Gravant des de : '{_micDeviceName}'");
    }

    private void EndMicrophone()
    {
        if (!string.IsNullOrEmpty(_micDeviceName) && Microphone.IsRecording(_micDeviceName))
        {
            Microphone.End(_micDeviceName);
            Debug.Log("Fi de la gravació");
            Recording = false;
        }
    }

    private string GetRecordResult()
    {
        if (_micClip == null || _voskRecognizer == null) return "";

        int micCurrentPos = Microphone.GetPosition(_micDeviceName);
        if (micCurrentPos < 0 || micCurrentPos == _micLastSamplePos) return "";

        int samplesToRead = (micCurrentPos > _micLastSamplePos)
            ? micCurrentPos - _micLastSamplePos
            : (_micClip.samples - _micLastSamplePos) + micCurrentPos;

        string result = string.Empty;
        while (samplesToRead > 0)
        {
            int thisChunk = Mathf.Min(samplesToRead, MAX_CHUNCK_SIZE);

            float[] floatData = new float[thisChunk];

            int startPos = _micLastSamplePos;
            if (startPos + thisChunk > _micClip.samples)
            {

                thisChunk = _micClip.samples - startPos;
            }

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

                if (recoResult == null || string.IsNullOrEmpty(recoResult.text)) return "";

                result += recoResult.text + " ";
                Debug.Log("Microphone Record Result: " + recoResult.text);
            }

            _micLastSamplePos = (_micLastSamplePos + thisChunk) % _micClip.samples;
            samplesToRead -= thisChunk;
        }

        return result;
    }
}
