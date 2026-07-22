using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerStomp : MonoBehaviour
{
    [Header("Stomp Settings")]
    public int stompDamage = 10;
    public int bounceForce = 8;
    public int stompRange = 1; // distance below player to detect enemies (approx in units)

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private bool isFalling;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Detect falling
        isFalling = !controller.isGrounded && verticalVelocity < 0;

        // Track downward velocity manually
        if (!controller.isGrounded)
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        else
            verticalVelocity = 0f;

        CheckForStomp();
    }

    void CheckForStomp()
    {
        if (!isFalling) return;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.down * (controller.height / 2f - 0.05f);

        if (Physics.Raycast(origin, Vector3.down, out hit, (float)stompRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Deal damage to the enemy
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(stompDamage);
                    Debug.Log("Enemy stomped!");
                }

                // Bounce the player back up
                Bounce();
            }
        }
    }

    void Bounce()
    {
        verticalVelocity = (float)bounceForce;
        controller.Move(Vector3.up * (float)bounceForce * Time.deltaTime);
    }
}