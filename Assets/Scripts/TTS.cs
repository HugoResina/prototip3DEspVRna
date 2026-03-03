using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.InferenceEngine;
using UnityEngine;

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

        // DownloadToArray() en lloc de ToReadOnlyArray()
        float[] samples = outputTensor.DownloadToArray();

        AudioClip clip = AudioClip.Create("TTS_Output", samples.Length, 1, _sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

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

    private Dictionary<string, int> ParseVocabJson(string json)
    {
        var dict = new Dictionary<string, int>();

        json = json.Trim().TrimStart('{').TrimEnd('}');

        int i = 0;
        while (i < json.Length)
        {
            // Troba la clau
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
