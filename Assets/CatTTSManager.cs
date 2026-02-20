using System.Collections.Generic;
using Unity.Collections;
using Unity.InferenceEngine; 
using UnityEngine;
using UnityEngine.Timeline;
using Unity.InferenceEngine;

public class CatTTSManager : MonoBehaviour
{
    public ModelAsset modelAsset; // Arrastra aquí el archivo .sentis
    private IWorker worker;
    private Model runtimeModel;

    void Start()
    {
        // 1. Cargar el modelo
        runtimeModel = ModelLoader.Load(modelAsset);
        // 2. Crear el motor de ejecución (GPU es más rápido para TTS)
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
    }

    public void Speak(string text)
    {
        // 3. Convertir texto a Tensores (Input)
        // NOTA: Aquí debes haber convertido el texto a IDs de fonemas previamente
        int[] inputIds = PhonemizeCatalan(text);
        using var inputTensor = new TensorInt(new TensorShape(1, inputIds.Length), inputIds);

        // 4. Ejecutar el modelo
        worker.Execute(inputTensor);

        // 5. Obtener el audio
        TensorFloat outputAudio = worker.PeekOutput() as TensorFloat;

        // Convertir el tensor a un AudioClip de Unity
        float[] audioData = outputAudio.ToReadOnlyArray();
        PlayAudio(audioData);
    }

    void PlayAudio(float[] data)
    {
        AudioClip clip = AudioClip.Create("TTS_Output", data.Length, 1, 22050, false);
        clip.SetData(data, 0);
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    private void OnDestroy()
    {
        worker?.Dispose();
    }
}