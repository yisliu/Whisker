using UnityEngine;
using UnityEditor;

public class FixJumpStarEffect : EditorWindow
{
    [MenuItem("Tools/Fix Jump Star Effect")]
    public static void FixJumpEffect()
    {
        // Find the player in the scene
        GameObject player = Selection.activeGameObject;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select the player GameObject in the hierarchy", "OK");
            return;
        }

        // Find existing particle system or create new one
        ParticleSystem ps = player.GetComponentInChildren<ParticleSystem>();
        GameObject particleObj;

        if (ps == null)
        {
            // Create new particle system
            particleObj = new GameObject("JumpStarParticles");
            particleObj.transform.SetParent(player.transform);
            particleObj.transform.localPosition = Vector3.zero;
            ps = particleObj.AddComponent<ParticleSystem>();
        }
        else
        {
            particleObj = ps.gameObject;
        }

        // Configure particle system
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var colorOverLifetime = ps.colorOverLifetime;
        var sizeOverLifetime = ps.sizeOverLifetime;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        // CRITICAL FIXES
        main.duration = 1f; // Increased from 0.1 to allow bursts to work properly
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.gravityModifier = 0.3f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.maxParticles = 1000;
        main.stopAction = ParticleSystemStopAction.None; // CHANGED FROM DISABLE!

        // Emission - burst only
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20) // Emit 20 particles at start
        });

        // Shape - sphere at character feet
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;
        shape.position = new Vector3(0f, 0.1f, 0f);

        // Color gradient
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.9f, 0.2f), 0f),    // Bright yellow
                new GradientColorKey(new Color(1f, 0.3f, 0.6f), 0.3f),  // Pink
                new GradientColorKey(new Color(0.4f, 0.6f, 1f), 0.6f),  // Blue
                new GradientColorKey(new Color(0.9f, 0.4f, 1f), 1f)     // Purple
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Size over lifetime
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(0.5f, 1.3f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Renderer settings
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Try to use an existing good material or create one
        Material particleMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
        if (particleMat != null)
        {
            renderer.material = particleMat;
        }
        else
        {
            // Fallback to standard shader
            Material fallbackMat = new Material(Shader.Find("Particles/Standard Unlit"));
            fallbackMat.SetColor("_Color", Color.white);
            renderer.material = fallbackMat;
        }

        // Ensure GameObject is enabled
        particleObj.SetActive(true);

        // Link to PlayerMovementFixed
        var playerMovement = player.GetComponent<PlayerMovementFixed>();
        if (playerMovement != null)
        {
            var field = typeof(PlayerMovementFixed).GetField("jumpStarParticles",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(playerMovement, ps);
                EditorUtility.SetDirty(playerMovement);
            }
        }

        EditorUtility.DisplayDialog("Success",
            $"Jump Star Effect fixed!\n\n" +
            $"Key changes:\n" +
            $"- Stop Action: None (was Disable)\n" +
            $"- Duration: 1s (was 0.1s)\n" +
            $"- Burst: 20 particles\n" +
            $"- Material: {renderer.material.name}\n\n" +
            $"Test it now!", "OK");

        Selection.activeGameObject = particleObj;
    }
}
