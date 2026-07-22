using UnityEngine;

public class PlayerThrowBomb2 : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;         // prefab with Rigidbody + bomb logic
    public Transform throwOrigin;         // where bomb spawns (e.g. hand)
    public float throwSpeed = 12f;        // forward force
    public float throwUpward = 3f;        // upward arc
    public LayerMask ignoreCollisionWith; // optional — prevent self collision

    [Header("Input & Cooldown")]
    public bool useLegacyInput = true;    // uses mouse left click
    public float cooldown = 1.2f;         // delay between throws
    private float lastThrowTime;

    void Update()
    {
        if (useLegacyInput && Input.GetMouseButtonDown(0))
        {
            TryThrow();
        }
    }

    void TryThrow()
    {
        // Check cooldown
        if (Time.time < lastThrowTime + cooldown)
            return;

        ThrowBomb2();
        lastThrowTime = Time.time;
    }

    void ThrowBomb2()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("⚠️ Bomb prefab not assigned!");
            return;
        }

        // Spawn position (front of player or custom origin)
        Vector3 spawnPos = throwOrigin != null
            ? throwOrigin.position
            : transform.position + transform.forward * 1.5f + Vector3.up * 1f;

        // Create bomb
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        // Add throwing force
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwForce = transform.forward * throwSpeed + Vector3.up * throwUpward;
            rb.AddForce(throwForce, ForceMode.Impulse);
        }

        Debug.Log("💣 Bomb thrown!");
    }
}