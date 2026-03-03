using System;
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

    public static event Action<int> OnAISend = delegate { };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _groqChat = GetComponent<GroqChat>();
        _stt = GetComponent<NewSTT>();
        _tts = GetComponent<TTS>();
    }

    public void SendFunc()
    {
        if (!string.IsNullOrEmpty(UIManager.Instance.InterPerInputFieldText))
        {
            _groqChat.SendMessage(UIManager.Instance.InterPerInputFieldText, (response) =>
            {
                var responseObj = JsonUtility.FromJson<ResponseObj>(response);

                Debug.Log("index: ----------->" + responseObj.index);

                OnAISend.Invoke(responseObj.index);

                UIManager.Instance.InterPerResponseText = responseObj.response;
                _tts.SpeakText(responseObj.response);
            });
        }
        else
        {
            UIManager.Instance.InterPerResponseText = "???";
        }
    }
}
