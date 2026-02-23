using UnityEngine;

public class SmokeParticlesScript : MonoBehaviour
{
    private ParticleSystem system;
    private ParticleSystem.Particle[] particles;

    [SerializeField] private float fadeDuration = 2.0f; 
    private float currentFadeTime = 0f;
    public bool startFading = false;
    public bool leekClosed = false;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (startFading)
        {
            if (particles == null || particles.Length < system.main.maxParticles)
                particles = new ParticleSystem.Particle[system.main.maxParticles];

            int numParticlesAlive = system.GetParticles(particles);
            currentFadeTime += Time.deltaTime;
            float alphaPercent = Mathf.Clamp01(1 - (currentFadeTime / fadeDuration));

            for (int i = 0; i < numParticlesAlive; i++)
            {
                Color c = particles[i].startColor;
                c.a = alphaPercent;
                particles[i].startColor = c;
            }

            system.SetParticles(particles, numParticlesAlive);

           

            if (alphaPercent <= 0)
            {
                //FinalizarEfecto();
            }
        }
    }

    public void StartToDisipate()
    {
        startFading = true;
    }
    void FinalizarEfecto()
    {
        startFading = false;
        this.gameObject.SetActive(false);
    }
}