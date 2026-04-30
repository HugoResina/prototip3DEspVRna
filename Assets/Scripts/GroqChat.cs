using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Gestiona la comunicació amb l'API de Groq per generar respostes de language model
/// dins d'un context de joc Unity. Utilitza el model configurat per processar prompts
/// i retornar respostes via callback.
/// </summary>
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

    /// <summary>
    /// Callback subscrit a <see cref="PlayerInteraction.GetPrompt"/>.
    /// Estableix el prompt de sistema que condicionarà el comportament del model
    /// en totes les peticions posteriors.
    /// </summary>
    /// <param name="prompt">Prompt de sistema (rol, context, restriccions, etc.).</param>
    public void AsignInitialPrompt(string prompt)
    {
        if (prompt == null)
        {
            Debug.Log("No hi ha cap prompt SO asignat");
            return;
        }
        _initialPrompt = prompt;
    }

    /// <summary>
    /// Carrega la API key des de <c>Assets/Resources/env.txt</c>.
    /// El fitxer ha de contenir una línia amb el format: <c>GROQ_API_KEY=xxxx</c>
    /// </summary>
    /// <returns>La API key com a string, o buit si no es troba.</returns>
    private string LoadApiKey()
    {
        TextAsset envFile = Resources.Load<TextAsset>("env");
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

    /// <summary>
    /// Inicia una petició asíncrona a l'API de Groq amb el prompt proporcionat.
    /// La resposta es retorna via callback un cop completada la crida HTTP.
    /// </summary>
    /// <param name="prompt">Missatge de l'usuari que es vol enviar al model.</param>
    /// <param name="onResponse">
    /// Callback invocat amb el text de resposta del model,
    /// o amb <c>"Error de xarxa"</c> si la petició falla.
    /// </param>
    public void SendMessage(string prompt, Action<string> onResponse)
    {
        Debug.Log("response: ----------->" + prompt);
        StartCoroutine(ChatCoroutine(prompt, onResponse));
    }

    /// <summary>
    /// Coroutine que construeix i envia la petició POST a l'endpoint de Groq,
    /// parseja la resposta JSON i invoca el callback.
    /// </summary>
    /// <remarks>
    /// La petició inclou dos missatges: el <c>system</c> prompt (definit per
    /// <see cref="AsignInitialPrompt"/>) i el missatge <c>user</c> actual.
    /// S'utilitza <see cref="JsonUtility"/> en lloc de serialització manual
    /// perquè les classes de request estan marcades amb [Serializable].
    /// </remarks>
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