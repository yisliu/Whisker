using UnityEngine;

public class DebugParticleSystem : MonoBehaviour
{
    public ParticleSystem targetParticleSystem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && targetParticleSystem != null)
        {
            Debug.Log("=== Particle System Debug Info ===");
            Debug.Log($"Is Playing: {targetParticleSystem.isPlaying}");
            Debug.Log($"Is Emitting: {targetParticleSystem.isEmitting}");
            Debug.Log($"Particle Count: {targetParticleSystem.particleCount}");
            Debug.Log($"Time: {targetParticleSystem.time}");

            var main = targetParticleSystem.main;
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Play On Awake: {main.playOnAwake}");
            Debug.Log($"Loop: {main.loop}");
            Debug.Log($"Duration: {main.duration}");

            var emission = targetParticleSystem.emission;
            Debug.Log($"Emission Enabled: {emission.enabled}");
            Debug.Log($"Rate Over Time: {emission.rateOverTime.constant}");

            Debug.Log("=== End Debug Info ===");
        }

        if (Input.GetKeyDown(KeyCode.T) && targetParticleSystem != null)
        {
            Debug.Log("Manual test play triggered!");
            targetParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            targetParticleSystem.Clear();
            targetParticleSystem.Simulate(0, true, true);
            targetParticleSystem.Play();
        }
    }
}
