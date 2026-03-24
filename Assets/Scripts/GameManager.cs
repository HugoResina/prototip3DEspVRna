using System;
using System.IO;
using Unity.AppUI.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool SttEnabled { get => _stt.enabled; set => _stt.enabled = value; }

    [Serializable]
    public class ResponseObj
    {
        public int index;
        public string response;
    }


    private GroqChat _groqChat;
    private NewSTT _stt;
    private TTS _tts;
   

    private string nombreArchivo = "RegistreInteraccions.txt";
    private string rutaCompleta;

    public static event Action<int> OnAISend = delegate { };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            rutaCompleta = Path.Combine(Application.dataPath, nombreArchivo);

            File.AppendAllText(rutaCompleta, $"\n--- Nova Sessón: {DateTime.Now} ---\n");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _groqChat = GetComponent<GroqChat>();
        _stt = GetComponent<NewSTT>();
        _tts = GetComponent<TTS>();

        SttEnabled = false;
    }

    public void SendFunc()
    {
        if (!string.IsNullOrEmpty(UIManager.Instance.InterPerInputFieldText))
        {
            _groqChat.SendMessage(UIManager.Instance.InterPerInputFieldText, (response) =>
            {
                var responseObj = JsonUtility.FromJson<ResponseObj>(response);

                Debug.Log("index: ----------->" + responseObj.index);
                Debug.Log("response: ----------->" + responseObj.response);

                RegistrarEnArchivo(responseObj);

                OnAISend.Invoke(responseObj.index);

                InteractablePersonEvents.UpdateResponseText(responseObj.response);
                _tts.SpeakText(responseObj.response);
            });
        }
        else
        {
            InteractablePersonEvents.UpdateResponseText("???");
        }
    }

    private void RegistrarEnArchivo(ResponseObj obj)
    {
        try
        {
            
            using (StreamWriter sw = new StreamWriter(rutaCompleta, true))
            {
                sw.WriteLine($"[{DateTime.Now:HH:mm:ss}] Index: {obj.index}");
                sw.WriteLine($"Resposta: {obj.response}");
                //get prompt
                sw.WriteLine("------------------------------------------");
            }

         

        }
        catch (Exception e)
        {
            Debug.LogError($"Error al escriure l'arxiu: {e.Message}");
        }
    }

}
