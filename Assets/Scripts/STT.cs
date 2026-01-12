using System;
using UnityEngine;
using UnityEngine.UI;

using Vosk;

public class STT : MonoBehaviour
{
    [Serializable]
    class OutputObj
    {
        public string text;
    }

    private Model model;
    private VoskRecognizer rec;
    public LocalAIClient localIAClient;


    //public string modelPath = "C:/vosk-model-small-es-0.42";

    public string modelPath = "C:/vosk-model-small-ca-0.4";
    public int sampleRate = 16000;

    public string deviceName = null;
    public int clipLengthSec = 10;
  

    public Text outputText;
    public Text AiOuptutText;

    private AudioClip micClip;
    private int lastSamplePos = 0;

    [SerializeField] private Button Erase;
    [SerializeField] private Button Send;

    public static event Action<int> OnSend;

  

    [System.Serializable]
    public class responseObj
    {
        public int index;
        public string response;
    }

   
    private void Start()
    {

        Erase.onClick.AddListener(EraseFunc);
        Send.onClick.AddListener(SendFunc);
        model = new Model(modelPath);
        rec = new VoskRecognizer(model, sampleRate);


        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No hay micrófonos disponibles.");
            return;
        }

        if (string.IsNullOrEmpty(deviceName))
            deviceName = Microphone.devices[0];

        micClip = Microphone.Start(deviceName, true, clipLengthSec, sampleRate);
        if (micClip == null)
        {
            Debug.LogError("No se pudo iniciar el micrófono.");
            return;
        }

        Debug.Log("Grabando desde: " + deviceName);
        if (outputText != null)
            outputText.text = "";
    }

    public void EraseFunc()
    {
        outputText.text = "";
    }

    public async void SendFunc()
    {
        localIAClient = GetComponent<LocalAIClient>();
        //Debug.Log(localIAClient);
        string response = await localIAClient.CallLocalAIAsync(outputText.text);

        var responseobj = JsonUtility.FromJson<responseObj>(response);
        //Debug.Log("Resposta de la IA local: " + response);
        AiOuptutText.text = responseobj.response;
        Debug.Log("index: ----------->" + responseobj.index);
       
        OnSend?.Invoke(responseobj.index);
    }

    private void Update()
    {
     


            if (micClip == null || rec == null)
                return;

            int currentPos = Microphone.GetPosition(deviceName);
            if (currentPos < 0 || currentPos == lastSamplePos)
                return;

            int samplesToRead = (currentPos > lastSamplePos)
                ? currentPos - lastSamplePos
                : (micClip.samples - lastSamplePos) + currentPos;

            const int maxChunkSize = 4096;

            while (samplesToRead > 0)
            {
                int thisChunk = Mathf.Min(samplesToRead, maxChunkSize);

                float[] floatData = new float[thisChunk];

                int startPos = lastSamplePos;
                if (startPos + thisChunk > micClip.samples)
                {

                    thisChunk = micClip.samples - startPos;
                }

                micClip.GetData(floatData, startPos);


                short[] shortData = new short[thisChunk];
                for (int i = 0; i < thisChunk; i++)
                {
                    float f = Mathf.Clamp(floatData[i], -1f, 1f);
                    shortData[i] = (short)(f * short.MaxValue);
                }

                byte[] bytes = new byte[thisChunk * 2];
                Buffer.BlockCopy(shortData, 0, bytes, 0, bytes.Length);

                if (rec.AcceptWaveform(bytes, bytes.Length))
                {

                    string json = rec.Result();
                    OutputObj output = JsonUtility.FromJson<OutputObj>(json);

                    if (output != null && !string.IsNullOrEmpty(output.text))
                    {
                        if (outputText != null)
                            outputText.text += output.text + " ";
                        Debug.Log("RESULT: " + output.text);
                    }
                }


                lastSamplePos = (lastSamplePos + thisChunk) % micClip.samples;
                samplesToRead -= thisChunk;
            }
        
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(deviceName) && Microphone.IsRecording(deviceName))
            Microphone.End(deviceName);

        rec?.Dispose();
        model?.Dispose();
    }
}