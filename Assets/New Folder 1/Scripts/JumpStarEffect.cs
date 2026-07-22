using UnityEngine;

public class JumpStarEffect : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem starParticleSystem;

    [Header("Jump Detection")]
    private bool wasGrounded = true;
    private bool hasPlayedEffect = false;
    private bool isInitialized = false;
    private CharacterController characterController;
    private float initTimer = 0f;

    private void Start()
    {
        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();

        // If particle system not assigned, try to find it as a child
        if (starParticleSystem == null)
        {
            starParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        // Make sure particles don't play on start
        if (starParticleSystem != null)
        {
            var main = starParticleSystem.main;
            main.playOnAwake = false;
            starParticleSystem.Stop();
            starParticleSystem.Clear();
        }
    }

    private void Update()
    {
        if (characterController == null || starParticleSystem == null)
            return;

        // Wait a short time before enabling jump detection to avoid startup triggers
        if (!isInitialized)
        {
            initTimer += Time.deltaTime;
            if (initTimer >= 0.5f)
            {
                isInitialized = true;
                wasGrounded = characterController.isGrounded;
            }
            return;
        }

        // Detect when character just left the ground (started jumping)
        if (wasGrounded && !characterController.isGrounded && !hasPlayedEffect)
        {
            // Stop any existing particles and play fresh burst
            starParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            starParticleSystem.Clear();
            starParticleSystem.Simulate(0, true, true); // Reset simulation
            starParticleSystem.Play();
            hasPlayedEffect = true;
        }

        // Reset the flag when character lands
        if (characterController.isGrounded && hasPlayedEffect)
        {
            hasPlayedEffect = false;
        }

        // Update grounded state for next frame
        wasGrounded = characterController.isGrounded;
    }

    // Method to manually trigger the effect (can be called from other scripts)
    public void PlayJumpEffect()
    {
        if (starParticleSystem != null)
        {
            starParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            starParticleSystem.Clear();
            starParticleSystem.Simulate(0, true, true); // Reset simulation
            starParticleSystem.Play();
        }
    }
}
