using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GroqChat : MonoBehaviour
{
    [SerializeField] private string _model = "llama-3.1-8b-instant";

    private string _apiKey;
    private string _initialPrompt = "";

    private void OnEnable()
    {
        PlayerInteraction.GetPrompt += AsignInitialPrompt;
    }
    private void OnDisable()
    {
        PlayerInteraction.GetPrompt -= AsignInitialPrompt;
    }

    void Awake()
    {
        _apiKey = LoadApiKey();
        if (string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("No API key trobada! Crea Assets/.env");
        }
    }

    public void AsignInitialPrompt(string prompt)
    {
        if (prompt == null)
        {
            Debug.Log("No hi ha cap prompt SO asignat");
            return;
        }
        _initialPrompt = prompt;
    }

    private string LoadApiKey()
    {
        TextAsset envFile = Resources.Load<TextAsset>("env"); // Assets/Resources/env.txt
        if (envFile != null)
        {
            string[] lines = envFile.text.Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("GROQ_API_KEY="))
                {
                    return line.Split('=')[1].Trim();
                }
            }
        }
        return "";
    }

    public void SendMessage(string prompt, Action<string> onResponse)
    {

        Debug.Log("response: ----------->" + prompt);

        StartCoroutine(ChatCoroutine(prompt, onResponse));
    }

    IEnumerator ChatCoroutine(string prompt, Action<string> onResponse)
    {
        string url = "https://api.groq.com/openai/v1/chat/completions";

        GroqRequest request = new GroqRequest()
        {
            model = _model,
            messages = new GroqMessage[]
            {
                new() { role = "system", content = _initialPrompt },
                new() { role = "user", content = prompt }
            }
        };

        // JSON manual (Unity JsonUtility no suporta arrays complexes)
        //string json = $"{{\"model\":\"llama-3.1-8b-instant\",\"messages\":[{{\"role\":\"user\",\"content\":\"{prompt}\"}}],\"max_tokens\":500,\"temperature\":0.7}}";
        string json = JsonUtility.ToJson(request);
        Debug.Log("Enviant a IA: " + json);

        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Authorization", $"Bearer {_apiKey}");
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Resposta crua: " + req.downloadHandler.text);

                GroqResponse response = JsonUtility.FromJson<GroqResponse>(req.downloadHandler.text);

                //UIManager.Instance.SetAiOutputText(response.choices[0].message.content);
                Debug.Log("resposta textual : " + response.choices[0].message.content);
                onResponse?.Invoke(response.choices[0].message.content);
            }
            else
            {
                Debug.LogError($"Groq Error: {req.downloadHandler.text}");
                onResponse?.Invoke("Error de xarxa");
            }
        }
    }
}
