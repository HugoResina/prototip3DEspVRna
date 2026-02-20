using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using JetBrains.Annotations;

public class LocalAIClient : MonoBehaviour
{
    [SerializeField] private string apiUrl = "http://localhost:11434/api/generate";
    [SerializeField] private string modelName = "qwen2.5:3b";
    //[SerializeField] private string modelName = "qwen2.5:7b";
    //[SerializeField] private string modelName = "gemma3:latest";
    //[SerializeField] private string modelName = "gemma3:12b";

    private string interactedPropmt = "";
    private void OnEnable()
    {
        PlayerInteraction.GetPrompt += asignString;
    }
    private void OnDisable()
    {
        PlayerInteraction.GetPrompt -= asignString;
    }
    public void asignString(string prompt)
    {
        if (prompt == null)
        {
            Debug.Log("no hi ha un prompt SO asignat");
            return;
        }
        interactedPropmt = prompt;
    }

    public async Task<string> CallLocalAIAsync(string userText)
    {
    
        
        ChatRequest req = new ChatRequest
        {
            model = modelName,
            stream = false,
            messages = new[]
            {
                new ChatMessage { role = "system", content = interactedPropmt},
                new ChatMessage { role = "user", content = userText }
            },
            think = false,
            format = "json"
        };

        string json = JsonUtility.ToJson(req);
        Debug.Log("Enviant a IA: " + json);

        using(UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            var op = www.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error IA: " + www.error + " | " + www.downloadHandler.text);
                return "Error de connexió";
            }

            Debug.Log("Resposta crua: " + www.downloadHandler.text);

            ChatResponse resp = JsonUtility.FromJson<ChatResponse>(www.downloadHandler.text);
            string resposta = resp?.message?.content ?? "Resposta buida";

            

            return resposta;
        }
        
    }

   
}
