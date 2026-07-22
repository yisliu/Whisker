using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerThrowBomb : MonoBehaviour
{
    [Header("Bomb")]
    public GameObject bombPrefab;        // assign bomb prefab in Inspector
    public Transform throwOrigin;        // optional: empty child in front of camera
    public float throwSpeed = 12f;       // forward speed
    public float throwUpward = 3f;       // upward speed for arc
    public LayerMask ignoreCollisionWith; // layers to ignore (e.g. Player layer)

    [Header("Input")]
    public bool useLegacyInput = true;   // use Input.GetMouseButtonDown
    public float cooldown = 0.5f;
    float lastThrowTime;

    void Update()
    {
        if (useLegacyInput)
        {
            if (Input.GetMouseButtonDown(0) && Time.time >= lastThrowTime + cooldown)
            {
                ThrowBomb();
                lastThrowTime = Time.time;
            }
        }
        // If you're using the new Input System call ThrowBomb() from the action callback instead.
    }

    // Public so you can wire it to an Input System action (Send Message / PlayerInput) or call it from other scripts
    public void ThrowBomb()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("[PlayerThrowBomb] bombPrefab not assigned in Inspector.");
            return;
        }

        Vector3 spawnPos;
        Quaternion spawnRot;
        if (throwOrigin != null)
        {
            spawnPos = throwOrigin.position;
            spawnRot = throwOrigin.rotation;
        }
        else
        {
            spawnPos = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
            spawnRot = transform.rotation;
        }

        GameObject bomb = Instantiate(bombPrefab, spawnPos, spawnRot);

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("[PlayerThrowBomb] bombPrefab is missing a Rigidbody component.");
        }
        else
        {
            // set initial velocity for predictable arc
            Vector3 velocity = (spawnRot * Vector3.forward) * throwSpeed + (spawnRot * Vector3.up) * throwUpward;
            rb.linearVelocity = velocity;
        }

        // Try to ignore collisions with layers set in ignoreCollisionWith
        if (ignoreCollisionWith != 0)
        {
            Collider[] bombCols = bomb.GetComponentsInChildren<Collider>();
            Collider[] sceneCols = FindObjectsOfType<Collider>();
            foreach (var sc in sceneCols)
            {
                int mask = 1 << sc.gameObject.layer;
                if ((ignoreCollisionWith & mask) != 0)
                {
                    foreach (var bc in bombCols)
                        if (bc != null && sc != null)
                            Physics.IgnoreCollision(bc, sc, true);
                }
            }
        }
        else
        {
            Collider[] myCols = GetComponentsInChildren<Collider>();
            Collider[] bombCols = bomb.GetComponentsInChildren<Collider>();
            foreach (var myCol in myCols)
            foreach (var bCol in bombCols)
                Physics.IgnoreCollision(myCol, bCol, true);
        }

        Debug.Log("[PlayerThrowBomb] Bomb instantiated at " + spawnPos);
    }
}