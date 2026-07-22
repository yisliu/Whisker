using UnityEngine;
using UnityEditor;

public class JumpStarEffectSetup : EditorWindow
{
    [MenuItem("Tools/Setup Jump Star Effect")]
    public static void SetupJumpStarEffect()
    {
        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = Selection.activeGameObject;
            if (player == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select the player GameObject in the hierarchy or tag it as 'Player'", "OK");
                return;
            }
        }

        // Create a child GameObject for the particle system
        GameObject particleObj = new GameObject("JumpStarParticles");
        particleObj.transform.SetParent(player.transform);
        particleObj.transform.localPosition = Vector3.zero;

        // Add particle system component
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var colorOverLifetime = ps.colorOverLifetime;
        var sizeOverLifetime = ps.sizeOverLifetime;
        var velocityOverLifetime = ps.velocityOverLifetime;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        // Main Module Settings
        main.duration = 0.1f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 0.7f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.gravityModifier = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.stopAction = ParticleSystemStopAction.Disable;

        // Emission Module
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 10)
        });

        // Shape Module - emit from bottom of character
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;
        shape.position = new Vector3(0f, 0.1f, 0f);

        // Color over Lifetime - Ribbon-like gradient
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.8f, 0f), 0f),      // Gold
                new GradientColorKey(new Color(1f, 0.2f, 0.8f), 0.3f),  // Pink
                new GradientColorKey(new Color(0.3f, 0.5f, 1f), 0.6f),  // Blue
                new GradientColorKey(new Color(0.8f, 0.3f, 1f), 1f)     // Purple
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Size over Lifetime
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(0.5f, 1.2f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Velocity over Lifetime - spiral upward motion
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(1.5f);

        // Renderer settings
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");

        // Create star texture or use default
        CreateStarTexture(renderer);

        // Add the JumpStarEffect script to the player
        JumpStarEffect jumpEffect = player.GetComponent<JumpStarEffect>();
        if (jumpEffect == null)
        {
            jumpEffect = player.AddComponent<JumpStarEffect>();
        }

        // Use reflection to set the particle system reference
        var field = typeof(JumpStarEffect).GetField("starParticleSystem",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(jumpEffect, ps);
        }

        EditorUtility.DisplayDialog("Success", "Jump Star Effect has been set up successfully!\n\nThe particle effect will play when the character jumps.", "OK");

        // Select the particle system so user can see it
        Selection.activeGameObject = particleObj;
    }

    private static void CreateStarTexture(ParticleSystemRenderer renderer)
    {
        // Try to find an existing star texture
        string[] starTextures = new string[] {
            "star",
            "sparkle",
            "particle"
        };

        foreach (string texName in starTextures)
        {
            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D tex in textures)
            {
                if (tex.name.ToLower().Contains(texName))
                {
                    Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
                    mat.mainTexture = tex;
                    mat.SetFloat("_Mode", 2); // Fade mode
                    mat.EnableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.renderQueue = 3000;
                    renderer.material = mat;
                    return;
                }
            }
        }

        // If no star texture found, create a simple procedural one
        CreateProceduralStarMaterial(renderer);
    }

    private static void CreateProceduralStarMaterial(ParticleSystemRenderer renderer)
    {
        // Create a simple glowing particle material
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));

        // Create a simple soft particle texture
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        Color[] colors = new Color[64 * 64];

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dx = (x - 32f) / 32f;
                float dy = (y - 32f) / 32f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                // Create a star-like shape
                float angle = Mathf.Atan2(dy, dx);
                float star = Mathf.Abs(Mathf.Sin(angle * 5f)) * 0.3f + 0.7f;

                float alpha = Mathf.Clamp01((1f - dist) * star);
                alpha = Mathf.Pow(alpha, 2f); // Make it sharper

                colors[y * 64 + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        // Save the texture
        string path = "Assets/Textures/StarParticle.png";
        System.IO.Directory.CreateDirectory("Assets/Textures");
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        // Load and assign the texture
        Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (savedTexture != null)
        {
            mat.mainTexture = savedTexture;
        }

        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.renderQueue = 3000;

        // Save the material
        string matPath = "Assets/Materials/StarParticleMaterial.mat";
        System.IO.Directory.CreateDirectory("Assets/Materials");
        AssetDatabase.CreateAsset(mat, matPath);
        AssetDatabase.SaveAssets();

        renderer.material = mat;
    }
}
