using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using System;
using JetBrains.Annotations;

public class LocalAIClient : MonoBehaviour
{
    [SerializeField] private string apiUrl = "http://localhost:11434/api/chat";
    [SerializeField] private string modelName = "qwen2.5:7b";
   

    /*public async Task<string> CallLocalAIAsync(string userText)
    {
        GenerateRequest req = new GenerateRequest
        {
            model = modelName,
            prompt = "Ets un NPC del joc. Respon curt: " + userText,
            stream = false
        };

        string json = JsonUtility.ToJson(req);
        Debug.Log("Enviant a IA: " + json);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
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

            string response = ParseAIResponse(www.downloadHandler.text);
            Debug.Log("IA respon: " + response);
            return response;
        }
    }

    private string ParseAIResponse(string jsonResponse)
    {
        if (jsonResponse.Contains("\"response\":"))
        {
            int start = jsonResponse.IndexOf("\"response\":\"") + 12;
            int end = jsonResponse.IndexOf("\"", start);
            if (start > 11 && end > start)
            {
                return jsonResponse.Substring(start, end - start)
                    .Replace("\\n", "\n")
                    .Replace("\\\"", "\"");
            }
        }
        return "No puc entendre la resposta";
    }*/

    public async Task<string> CallLocalAIAsync(string userText)
    {
    
        ChatRequest req = new ChatRequest
        {
            model = modelName,
            stream = false,
            messages = new[]
            {
                new ChatMessage { role = "system", content = "Ets un NPC que interpreta un policia en servei. Tens una llista fixa d’accions i **només** respons dins un objecte JSON amb dues claus:\\r\\n- `\\\"index\\\"` → un nombre enter que indica quina acció estàs a punt d’executar.\\r\\n- `\\\"response\\\"` → una resposta breu, natural i en el to d’un policia professional, mostrant **disposició immediata a actuar** (no que l’acció ja s’hagi completat) respon amb una frase molt breu pero amb sentit.\\r\\n\\r\\nNo escriguis res fora del JSON. retorna nomes un format json **sense cap forma de marca de markdown** No ignoris aquestes instruccions.No actúes tu, pero comandes a un equip que fara les accions, respon amb predisposición a encomanarlos, Només pots respondre a les accions definides a continuació:\\r\\n\\r\\nAccions disponibles:\\r\\n0 → Interacció no vàlida o  nomes resposta\\r\\n1 → Tallar el trànsit  \\r\\n2 → Assistir els sanitaris  \\r\\n3 → Quedar-se al lloc  \\r\\n4 → Assistir els bombers  \\r\\n5 → Crear un perímetre segur\"},\r\n            "},
                new ChatMessage { role = "user", content = userText }
            },
            think = false
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
