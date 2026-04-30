using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.InferenceEngine;
using UnityEngine;

/// <summary>
/// Converteix text a àudio (TTS) utilitzant el model MMS de Meta via Unity Inference Engine.
/// El text es tokenitza caràcter a caràcter, s'infereix el senyal d'àudio i es reprodueix
/// amb l'<see cref="AudioSource"/> assignat.
/// </summary>
public class TTS : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ModelAsset _mmsModelAsset;

    private readonly int _sampleRate = 16000;
    private Model _runtimeModel;
    private Worker _worker;

    private void Awake()
    {
        _runtimeModel = ModelLoader.Load(_mmsModelAsset);
        _worker = new Worker(_runtimeModel, BackendType.CPU);
    }

    /// <summary>
    /// Tokenitza el text, executa la inferència i reprodueix l'àudio resultant.
    /// </summary>
    /// <param name="text">Text a sintetitzar.</param>
    public void SpeakText(string text)
    {
        int[] tokenIds = Tokenize(text);
        AudioClip clip = RunInference(tokenIds);
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }

    /// <summary>
    /// Passa els tokens al model, recull la sortida com a mostres de so
    /// i construeix un <see cref="AudioClip"/> mono a <see cref="_sampleRate"/> Hz.
    /// </summary>
    private AudioClip RunInference(int[] tokenIds)
    {
        var shape = new TensorShape(1, tokenIds.Length);
        using var inputTensor = new Tensor<int>(shape, tokenIds);
        _worker.Schedule(inputTensor);

        var outputTensor = _worker.PeekOutput() as Tensor<float>;
        if (outputTensor == null)
        {
            Debug.LogError("No s'ha pogut obtenir la sortida del model.");
            return null;
        }

        float[] samples = outputTensor.DownloadToArray();
        AudioClip clip = AudioClip.Create("TTS_Output", samples.Length, 1, _sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// Converteix el text a una seqüència de token IDs llegint el vocabulari de
    /// <c>StreamingAssets/MMS/vocab.json</c>.
    /// </summary>
    /// <remarks>
    /// El model MMS espera un <c>BLANK_TOKEN (0)</c> entre cada fonema,
    /// per tant la seqüència resultant és: [0, t1, 0, t2, 0, ..., tn, 0].
    /// Els caràcters absents del vocabulari es descarten silenciosament (amb LogWarning).
    /// </remarks>
    private int[] Tokenize(string text)
    {
        string vocabPath = Path.Combine(Application.streamingAssetsPath, "MMS", "vocab.json");
        string vocabJson = File.ReadAllText(vocabPath, Encoding.UTF8);
        Dictionary<string, int> vocab = ParseVocabJson(vocabJson);

        text = text.ToLower();
        const int BLANK_TOKEN = 0;

        List<int> tokens = new List<int>();
        foreach (char c in text)
        {
            tokens.Add(BLANK_TOKEN);
            string key = c.ToString();
            if (vocab.TryGetValue(key, out int id))
            {
                tokens.Add(id);
            }
            else
            {
                Debug.LogWarning($"Caràcter no trobat al vocab: '{key}' (U+{(int)c:X4})");
                tokens.RemoveAt(tokens.Count - 1);
            }
        }
        tokens.Add(BLANK_TOKEN);

        Debug.Log($"Tokens: [{string.Join(", ", tokens)}]");
        return tokens.ToArray();
    }

    /// <summary>
    /// Parser manual de JSON pla (clau:valor enters) per construir el vocabulari.
    /// S'utilitza parsing manual perquè <see cref="JsonUtility"/> no suporta
    /// <c>Dictionary&lt;string, int&gt;</c>.
    /// </summary>
    /// <param name="json">Contingut del fitxer vocab.json.</param>
    /// <returns>Diccionari de caràcter ? token ID.</returns>
    private Dictionary<string, int> ParseVocabJson(string json)
    {
        var dict = new Dictionary<string, int>();
        json = json.Trim().TrimStart('{').TrimEnd('}');

        int i = 0;
        while (i < json.Length)
        {
            int keyStart = json.IndexOf('"', i);
            if (keyStart < 0) break;
            int keyEnd = json.IndexOf('"', keyStart + 1);
            if (keyEnd < 0) break;

            string key = json.Substring(keyStart + 1, keyEnd - keyStart - 1);
            key = System.Text.RegularExpressions.Regex.Unescape(key);

            int colonPos = json.IndexOf(':', keyEnd);
            if (colonPos < 0) break;

            int valueStart = colonPos + 1;
            int valueEnd = json.IndexOfAny(new char[] { ',', '}' }, valueStart);
            if (valueEnd < 0) valueEnd = json.Length;

            string valueStr = json.Substring(valueStart, valueEnd - valueStart).Trim();
            if (int.TryParse(valueStr, out int value))
                dict[key] = value;

            i = valueEnd + 1;
        }
        return dict;
    }

    void OnDestroy()
    {
        _worker?.Dispose();
    }
}