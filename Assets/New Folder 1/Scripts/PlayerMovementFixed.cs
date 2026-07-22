/*

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float rotationSpeed = 180f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Effects")]
    public ParticleSystem jumpStarParticles;

    [Header("Pickup & Throw")]
    public Transform holdPoint;
    public float pickupRadius = 2.5f;
    public float throwSpeed = 15f;
    public float throwLoft = 4f;

    [Header("Animation Settings")]
    [SerializeField] private string horizontalParam = "Hor";
    [SerializeField] private string verticalParam = "Vert";
    [SerializeField] private string stateParam = "State";
    [SerializeField] private string jumpParam = "IsJump";
    [SerializeField] private float animationBlendSpeed = 4.5f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded = true;

    // Animation smoothing
    private Vector2 currentAnimAxis;
    private float currentAnimState;

    // Camera rotation
    private float cameraPitch = 0f;
    private Throwable heldObject;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Auto-find particle system if not assigned
        if (jumpStarParticles == null)
        {
            jumpStarParticles = GetComponentInChildren<ParticleSystem>();
        }

        // Ensure particles don't play on start
        if (jumpStarParticles != null)
        {
            var main = jumpStarParticles.main;
            main.playOnAwake = false;
            jumpStarParticles.Stop();
            jumpStarParticles.Clear();
        }
    }

    void Update()
    {
        // Improved ground check - use both methods
        bool controllerGrounded = controller.isGrounded;
        bool sphereGrounded = false;

        if (groundCheck != null && groundMask != 0)
        {
            sphereGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        // Consider grounded if either method detects ground
        isGrounded = controllerGrounded || sphereGrounded;

        // Also check with a raycast from center as backup
        if (!isGrounded)
        {
            RaycastHit hit;
            float rayDistance = (controller.height / 2f) + 0.3f;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
            {
                isGrounded = true;
            }
        }

        // Reset velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Rotation input
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationInput = 1f;
        }

        // Apply rotation
        if (rotationInput != 0f)
        {
            transform.Rotate(0, rotationInput * rotationSpeed * Time.deltaTime, 0);
        }

        // Mouse look - horizontal rotates player, vertical tilts camera
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate player body left/right with mouse X
        transform.Rotate(0, mouseX * mouseSensitivity, 0);

        // Tilt camera up/down with mouse Y
        if (playerCamera != null)
        {
            cameraPitch -= mouseY * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            playerCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Running
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && isGrounded ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump input
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log($"Space pressed! Final isGrounded: {isGrounded}, controller: {controllerGrounded}, sphere: {sphereGrounded}");

            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                Debug.Log($"JUMP EXECUTED! velocity.y = {velocity.y}");
                PlayJumpEffect();
            }
            else
            {
                Debug.Log("JUMP BLOCKED - Not grounded!");
            }
        }

        // Apply gravity
        float oldVelocityY = velocity.y;
        velocity.y += gravity * Time.deltaTime;

        Vector3 moveAmount = velocity * Time.deltaTime;
        controller.Move(moveAmount);

        // Debug: Check if velocity was changed
        if (oldVelocityY > 5f) // Only log when we just jumped
        {
            Debug.Log($"After Move: velocity.y went from {oldVelocityY} to {velocity.y}, moved {moveAmount.y}m");
        }

        // Pickup / Drop
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject != null)
            {
                heldObject.Drop();
                heldObject = null;
            }
            else
            {
                Transform cam = playerCamera != null ? playerCamera : transform;
                Throwable found = null;

                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit pickupHit, pickupRadius))
                {
                    Debug.Log($"[Pickup] Raycast hit: {pickupHit.collider.gameObject.name}");
                    found = pickupHit.collider.GetComponentInParent<Throwable>();
                }

                if (found == null)
                {
                    Collider[] nearby = Physics.OverlapSphere(transform.position, pickupRadius, ~0, QueryTriggerInteraction.Collide);
                    foreach (var col in nearby)
                    {
                        Throwable t = col.GetComponentInParent<Throwable>();
                        if (t != null) { found = t; break; }
                    }
                }

                if (found != null)
                {
                    heldObject = found;
                    heldObject.PickUp(holdPoint != null ? holdPoint : transform);
                    Debug.Log($"[Pickup] Picked up {found.gameObject.name}");
                }
                else
                {
                    Debug.Log("[Pickup] Nothing to pick up.");
                }
            }
        }

        // Throw
        if (Input.GetMouseButtonDown(0) && heldObject != null)
        {
            Camera throwCam = null;
            if (playerCamera != null)
            {
                throwCam = playerCamera.GetComponent<Camera>();
                if (throwCam == null) throwCam = playerCamera.GetComponentInChildren<Camera>();
            }
            if (throwCam == null) throwCam = Camera.main;

            Vector3 direction = transform.forward;
            if (throwCam != null)
            {
                Ray ray = throwCam.ScreenPointToRay(Input.mousePosition);
                direction = Physics.Raycast(ray, out RaycastHit throwHit, 200f)
                    ? (throwHit.point - (holdPoint != null ? holdPoint.position : transform.position)).normalized
                    : ray.direction;
            }

            heldObject.Throw(direction, throwSpeed, throwLoft);
            heldObject = null;
        }

        // Update animations
        UpdateAnimations(x, z, currentSpeed);

        // Track grounded state for next frame
        wasGrounded = isGrounded;
    }

    void UpdateAnimations(float inputX, float inputZ, float currentSpeed)
    {
        if (animator == null) return;

        // Calculate animation axis relative to character's local space
        Vector2 targetAnimAxis = new Vector2(inputX, inputZ);

        // Smooth the animation values for blend tree
        if (targetAnimAxis.sqrMagnitude > 0.01f)
        {
            Vector2 direction = (targetAnimAxis - currentAnimAxis).normalized;
            currentAnimAxis = Vector2.ClampMagnitude(
                currentAnimAxis + animationBlendSpeed * Time.deltaTime * direction,
                1f
            );
        }
        else
        {
            // Smoothly return to zero when no input
            currentAnimAxis = Vector2.ClampMagnitude(
                currentAnimAxis - animationBlendSpeed * Time.deltaTime * currentAnimAxis.normalized,
                Mathf.Max(0, currentAnimAxis.magnitude - animationBlendSpeed * Time.deltaTime)
            );
        }

        // Calculate state (0 = walk, 1 = run)
        float targetState = Input.GetKey(KeyCode.LeftShift) && isGrounded ? 1f : 0f;
        currentAnimState = Mathf.MoveTowards(currentAnimState, targetState, animationBlendSpeed * Time.deltaTime);

        // Set animator parameters
        animator.SetFloat(horizontalParam, currentAnimAxis.x);
        animator.SetFloat(verticalParam, currentAnimAxis.y);
        animator.SetFloat(stateParam, currentAnimState);
        animator.SetBool(jumpParam, !isGrounded);
    }

    void PlayJumpEffect()
    {
        if (jumpStarParticles == null)
        {
            Debug.LogError("jumpStarParticles is NULL! Assign it in Inspector or add a ParticleSystem as child.");
            return;
        }

        // Completely reset and restart the particle system
        jumpStarParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpStarParticles.Clear();
        jumpStarParticles.Simulate(0, true, true);
        jumpStarParticles.Play();

        // Debug info
        var main = jumpStarParticles.main;
        var emission = jumpStarParticles.emission;
        var renderer = jumpStarParticles.GetComponent<ParticleSystemRenderer>();

        Debug.Log($"Jump effect - Playing: {jumpStarParticles.isPlaying}, Emitting: {jumpStarParticles.isEmitting}, " +
                  $"MaxParticles: {main.maxParticles}, Duration: {main.duration}, " +
                  $"Material: {(renderer.material != null ? renderer.material.name : "NULL")}, " +
                  $"Emission enabled: {emission.enabled}");
    }

    // Draw ground check sphere in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}

*/

/*

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float rotationSpeed = 180f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Effects")]
    public ParticleSystem jumpStarParticles;

    [Header("Animation Settings")]
    [SerializeField] private string horizontalParam = "Hor";
    [SerializeField] private string verticalParam = "Vert";
    [SerializeField] private string stateParam = "State";
    [SerializeField] private string jumpParam = "IsJump";
    [SerializeField] private float animationBlendSpeed = 4.5f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded = true;

    // Animation smoothing
    private Vector2 currentAnimAxis;
    private float currentAnimState;

    // Camera rotation
    private float cameraPitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Auto-find particle system if not assigned
        if (jumpStarParticles == null)
        {
            jumpStarParticles = GetComponentInChildren<ParticleSystem>();
        }

        // Ensure particles don't play on start
        if (jumpStarParticles != null)
        {
            var main = jumpStarParticles.main;
            main.playOnAwake = false;
            jumpStarParticles.Stop();
            jumpStarParticles.Clear();
        }
    }

    void Update()
    {
        // Improved ground check - use both methods
        bool controllerGrounded = controller.isGrounded;
        bool sphereGrounded = false;

        if (groundCheck != null && groundMask != 0)
        {
            sphereGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        // Consider grounded if either method detects ground
        isGrounded = controllerGrounded || sphereGrounded;

        // Also check with a raycast from center as backup
        if (!isGrounded)
        {
            RaycastHit hit;
            float rayDistance = (controller.height / 2f) + 0.3f;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
            {
                isGrounded = true;
            }
        }

        // Reset velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Rotation input
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationInput = 1f;
        }

        // Apply rotation
        if (rotationInput != 0f)
        {
            transform.Rotate(0, rotationInput * rotationSpeed * Time.deltaTime, 0);
        }

        // Mouse look - horizontal rotates player, vertical tilts camera
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate player body left/right with mouse X
        transform.Rotate(0, mouseX * mouseSensitivity, 0);

        // Tilt camera up/down with mouse Y
        if (playerCamera != null)
        {
            cameraPitch -= mouseY * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            playerCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Running
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && isGrounded ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump input
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log($"Space pressed! Final isGrounded: {isGrounded}, controller: {controllerGrounded}, sphere: {sphereGrounded}");

            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                Debug.Log($"JUMP EXECUTED! velocity.y = {velocity.y}");
                PlayJumpEffect();
            }
            else
            {
                Debug.Log("JUMP BLOCKED - Not grounded!");
            }
        }

        // Apply gravity
        float oldVelocityY = velocity.y;
        velocity.y += gravity * Time.deltaTime;

        Vector3 moveAmount = velocity * Time.deltaTime;
        controller.Move(moveAmount);

        // Debug: Check if velocity was changed
        if (oldVelocityY > 5f) // Only log when we just jumped
        {
            Debug.Log($"After Move: velocity.y went from {oldVelocityY} to {velocity.y}, moved {moveAmount.y}m");
        }

        // Update animations
        UpdateAnimations(x, z, currentSpeed);

        // Track grounded state for next frame
        wasGrounded = isGrounded;
    }

    void UpdateAnimations(float inputX, float inputZ, float currentSpeed)
    {
        if (animator == null) return;

        // Calculate animation axis relative to character's local space
        Vector2 targetAnimAxis = new Vector2(inputX, inputZ);

        // Smooth the animation values for blend tree
        if (targetAnimAxis.sqrMagnitude > 0.01f)
        {
            Vector2 direction = (targetAnimAxis - currentAnimAxis).normalized;
            currentAnimAxis = Vector2.ClampMagnitude(
                currentAnimAxis + animationBlendSpeed * Time.deltaTime * direction,
                1f
            );
        }
        else
        {
            // Smoothly return to zero when no input
            currentAnimAxis = Vector2.ClampMagnitude(
                currentAnimAxis - animationBlendSpeed * Time.deltaTime * currentAnimAxis.normalized,
                Mathf.Max(0, currentAnimAxis.magnitude - animationBlendSpeed * Time.deltaTime)
            );
        }

        // Calculate state (0 = walk, 1 = run)
        float targetState = Input.GetKey(KeyCode.LeftShift) && isGrounded ? 1f : 0f;
        currentAnimState = Mathf.MoveTowards(currentAnimState, targetState, animationBlendSpeed * Time.deltaTime);

        // Set animator parameters
        animator.SetFloat(horizontalParam, currentAnimAxis.x);
        animator.SetFloat(verticalParam, currentAnimAxis.y);
        animator.SetFloat(stateParam, currentAnimState);
        animator.SetBool(jumpParam, !isGrounded);
    }

    void PlayJumpEffect()
    {
        if (jumpStarParticles == null)
        {
            Debug.LogError("jumpStarParticles is NULL! Assign it in Inspector or add a ParticleSystem as child.");
            return;
        }

        // Completely reset and restart the particle system
        jumpStarParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpStarParticles.Clear();
        jumpStarParticles.Simulate(0, true, true);
        jumpStarParticles.Play();

        // Debug info
        var main = jumpStarParticles.main;
        var emission = jumpStarParticles.emission;
        var renderer = jumpStarParticles.GetComponent<ParticleSystemRenderer>();

        Debug.Log($"Jump effect - Playing: {jumpStarParticles.isPlaying}, Emitting: {jumpStarParticles.isEmitting}, " +
                  $"MaxParticles: {main.maxParticles}, Duration: {main.duration}, " +
                  $"Material: {(renderer.material != null ? renderer.material.name : "NULL")}, " +
                  $"Emission enabled: {emission.enabled}");
    }

    // Draw ground check sphere in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
*/

/*

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float rotationSpeed = 180f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Effects")]
    public ParticleSystem jumpStarParticles;

    [Header("Pickup & Throw")]
    public Transform holdPoint;
    public float pickupRadius = 2.5f;
    public float throwSpeed = 15f;
    public float throwLoft = 4f;
    private const float ThrowMoveLockDuration = 0.3f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float cameraPitch = 0f;
    private Throwable heldObject;
    private float throwLockTimer;

    public bool IsJumping => !isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (jumpStarParticles == null)
            jumpStarParticles = GetComponentInChildren<ParticleSystem>();

        if (jumpStarParticles != null)
        {
            var main = jumpStarParticles.main;
            main.playOnAwake = false;
            jumpStarParticles.Stop();
            jumpStarParticles.Clear();
        }
    }

    void Update()
    {
        bool controllerGrounded = controller.isGrounded;
        bool sphereGrounded = false;

        if (groundCheck != null && groundMask != 0)
            sphereGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        isGrounded = controllerGrounded || sphereGrounded;

        if (!isGrounded)
        {
            RaycastHit hit;
            float rayDistance = (controller.height / 2f) + 0.3f;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
                isGrounded = true;
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Arrow key rotation
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotationInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) rotationInput = 1f;

        if (rotationInput != 0f)
            transform.Rotate(0, rotationInput * rotationSpeed * Time.deltaTime, 0);

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(0, mouseX * mouseSensitivity, 0);

        if (playerCamera != null)
        {
            cameraPitch -= mouseY * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            playerCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        // Count down throw movement lock
        if (throwLockTimer > 0f)
            throwLockTimer -= Time.deltaTime;

        // Movement (suppressed briefly after throwing)
        float x = throwLockTimer > 0f ? 0f : Input.GetAxis("Horizontal");
        float z = throwLockTimer > 0f ? 0f : Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && isGrounded ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump — blocked while holding an object so left-click throw doesn't also jump
        if (Input.GetButtonDown("Jump") && isGrounded && heldObject == null)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlayJumpEffect();
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Pickup
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject != null)
            {
                heldObject.Drop();
                heldObject = null;
            }
            else
            {
                Transform cam = playerCamera != null ? playerCamera : transform;
                Throwable found = null;

                // Look at an object and press E
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, pickupRadius))
                {
                    Debug.Log($"[Pickup] Raycast hit: {hit.collider.gameObject.name}");
                    found = hit.collider.GetComponentInParent<Throwable>();
                }

                // Fallback: anything close enough
                if (found == null)
                {
                    Collider[] nearby = Physics.OverlapSphere(transform.position, pickupRadius, ~0, QueryTriggerInteraction.Collide);
                    Debug.Log($"[Pickup] OverlapSphere found {nearby.Length} colliders within {pickupRadius}m");
                    foreach (var col in nearby)
                    {
                        Debug.Log($"[Pickup]   - {col.gameObject.name}");
                        Throwable t = col.GetComponentInParent<Throwable>();
                        if (t != null) { found = t; break; }
                    }
                }

                if (found != null)
                {
                    heldObject = found;
                    heldObject.PickUp(holdPoint != null ? holdPoint : transform);
                    Debug.Log($"[Pickup] Picked up {found.gameObject.name}");
                }
                else
                {
                    Debug.Log("[Pickup] Nothing to pick up.");
                }
            }
        }

        // Throw toward mouse cursor
        if (Input.GetMouseButtonDown(0) && heldObject != null)
        {
            Camera cam = null;
            if (playerCamera != null)
            {
                cam = playerCamera.GetComponent<Camera>();
                if (cam == null) cam = playerCamera.GetComponentInChildren<Camera>();
            }
            if (cam == null) cam = Camera.main;

            Vector3 direction = transform.forward;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                direction = Physics.Raycast(ray, out RaycastHit hit, 200f)
                    ? (hit.point - (holdPoint != null ? holdPoint.position : transform.position)).normalized
                    : ray.direction;
            }

            heldObject.Throw(direction, throwSpeed, throwLoft);
            heldObject = null;
            throwLockTimer = ThrowMoveLockDuration;
        }
    }

    void PlayJumpEffect()
    {
        if (jumpStarParticles == null) return;

        jumpStarParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpStarParticles.Clear();
        jumpStarParticles.Simulate(0, true, true);
        jumpStarParticles.Play();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}

*/


using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float rotationSpeed = 180f;
    
    [Header("2D Sprite & Animation")]
    public Animator spriteAnimator;
    public SpriteRenderer spriteRenderer;
    private Vector2 lastDirection = new Vector2(0, -1);

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Effects")]
    public ParticleSystem jumpStarParticles;

    [Header("Pickup & Throw")]
    public Transform holdPoint;
    public float pickupRadius = 2.5f;
    public float throwSpeed = 15f;
    public float throwLoft = 4f;
    private const float ThrowMoveLockDuration = 0.3f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float cameraPitch = 0f;
    private Throwable heldObject;
    private float throwLockTimer;

    public bool IsJumping => !isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (jumpStarParticles == null)
            jumpStarParticles = GetComponentInChildren<ParticleSystem>();

        if (jumpStarParticles != null)
        {
            var main = jumpStarParticles.main;
            main.playOnAwake = false;
            jumpStarParticles.Stop();
            jumpStarParticles.Clear();
        }
    }

    void Update()
    {
        bool controllerGrounded = controller.isGrounded;
        bool sphereGrounded = false;

        if (groundCheck != null && groundMask != 0)
            sphereGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        isGrounded = controllerGrounded || sphereGrounded;

        if (!isGrounded)
        {
            RaycastHit hit;
            float rayDistance = (controller.height / 2f) + 0.3f;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
                isGrounded = true;
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Arrow key rotation
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotationInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) rotationInput = 1f;

        if (rotationInput != 0f)
            transform.Rotate(0, rotationInput * rotationSpeed * Time.deltaTime, 0);

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(0, mouseX * mouseSensitivity, 0);

        if (playerCamera != null)
        {
            cameraPitch -= mouseY * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            playerCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        // Count down throw movement lock
        if (throwLockTimer > 0f)
            throwLockTimer -= Time.deltaTime;

        // Movement (suppressed briefly after throwing)
       // float x = throwLockTimer > 0f ? 0f : Input.GetAxis("Horizontal");
       // float z = throwLockTimer > 0f ? 0f : Input.GetAxis("Vertical");
        float x = throwLockTimer > 0f ? 0f : Input.GetAxisRaw("Horizontal");
        float z = throwLockTimer > 0f ? 0f : Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && isGrounded ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        UpdateSpriteAnimations(x, z);

        // Jump — blocked while holding an object so left-click throw doesn't also jump
        if (Input.GetButtonDown("Jump") && isGrounded && heldObject == null)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlayJumpEffect();
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Pickup
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject != null)
            {
                heldObject.Drop();
                heldObject = null;
            }
            else
            {
                Transform cam = playerCamera != null ? playerCamera : transform;
                Throwable found = null;

                // Look at an object and press E
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, pickupRadius))
                {
                    Debug.Log($"[Pickup] Raycast hit: {hit.collider.gameObject.name}");
                    found = hit.collider.GetComponentInParent<Throwable>();
                }

                // Fallback: anything close enough
                if (found == null)
                {
                    Collider[] nearby = Physics.OverlapSphere(transform.position, pickupRadius, ~0, QueryTriggerInteraction.Collide);
                    Debug.Log($"[Pickup] OverlapSphere found {nearby.Length} colliders within {pickupRadius}m");
                    foreach (var col in nearby)
                    {
                        Debug.Log($"[Pickup]   - {col.gameObject.name}");
                        Throwable t = col.GetComponentInParent<Throwable>();
                        if (t != null) { found = t; break; }
                    }
                }

                if (found != null)
                {
                    heldObject = found;
                    heldObject.PickUp(holdPoint != null ? holdPoint : transform);
                    Debug.Log($"[Pickup] Picked up {found.gameObject.name}");
                }
                else
                {
                    Debug.Log("[Pickup] Nothing to pick up.");
                }
            }
        }

        // Throw toward mouse cursor
        if (Input.GetMouseButtonDown(0) && heldObject != null)
        {
            Camera cam = null;
            if (playerCamera != null)
            {
                cam = playerCamera.GetComponent<Camera>();
                if (cam == null) cam = playerCamera.GetComponentInChildren<Camera>();
            }
            if (cam == null) cam = Camera.main;

            Vector3 direction = transform.forward;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                direction = Physics.Raycast(ray, out RaycastHit hit, 200f)
                    ? (hit.point - (holdPoint != null ? holdPoint.position : transform.position)).normalized
                    : ray.direction;
            }

            heldObject.Throw(direction, throwSpeed, throwLoft);
            heldObject = null;
            throwLockTimer = ThrowMoveLockDuration;
        }
    }

    void PlayJumpEffect()
    {
        if (jumpStarParticles == null) return;

        jumpStarParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpStarParticles.Clear();
        jumpStarParticles.Simulate(0, true, true);
        jumpStarParticles.Play();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    private void UpdateSpriteAnimations(float x, float z)
    {
        if (spriteAnimator == null)
        {
            return;
        }

        bool isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;
        if (isMoving)
        {
            lastDirection = new Vector2(x, z).normalized;
        }
        
        spriteAnimator.SetFloat("x", lastDirection.x);
        spriteAnimator.SetFloat("y", lastDirection.y);
        spriteAnimator.SetBool("isMoving", isMoving);
        spriteAnimator.SetBool("isGrounded", isGrounded);

    }
}
