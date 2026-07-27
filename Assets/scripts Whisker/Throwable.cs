/*

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Throwable : MonoBehaviour
{
    public int throwDamage = 15;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld;
    private bool isArmed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        isHeld = true;
        isArmed = false;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        isHeld = false;
        transform.SetParent(null);
        col.enabled = true;
        rb.isKinematic = false;
    }

    public void Throw(Vector3 direction, float speed, float loft)
    {
        isHeld = false;
        transform.SetParent(null);
        // Offset away from the player before re-enabling the collider so the
        // rigidbody doesn't overlap the CharacterController and push it upward.
        transform.position += direction.normalized * 0.6f;
        col.enabled = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * speed + Vector3.up * loft;
        Invoke(nameof(Arm), 0.2f);
    }

    void Arm() => isArmed = true;

    void OnCollisionEnter(Collision collision)
    {
        if (!isArmed) return;

        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
            enemy.TakeDamage(throwDamage);

        Destroy(gameObject);
    }
}

*/

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Throwable : MonoBehaviour
{
    public int throwDamage = 15;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld;
    private bool isArmed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        isHeld = true;
        isArmed = false;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        isHeld = false;
        transform.SetParent(null);
        col.enabled = true;
        rb.isKinematic = false;
    }

    public void Throw(Vector3 direction, float speed, float loft)
    {
        isHeld = false;
        transform.SetParent(null);
        // Offset away from the player before re-enabling the collider so the
        // rigidbody doesn't overlap the CharacterController and push it upward.
        transform.position += direction.normalized * 0.6f;
        col.enabled = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * speed + Vector3.up * loft;
        Invoke(nameof(Arm), 0.2f);
    }

    void Arm() => isArmed = true;

    void OnCollisionEnter(Collision collision)
    {
        if (!isArmed) return;

        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
            enemy.TakeDamage(throwDamage);

        Destroy(gameObject);
    }
}