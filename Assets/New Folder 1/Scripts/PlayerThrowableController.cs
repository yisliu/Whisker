using TMPro;
using UnityEngine;

public class PlayerThrowableController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private TextMeshProUGUI pickupPrompt;

    [Header("Settings")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float throwLoft = 5f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    private Throwable heldObject;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main?.transform;
    }

    void Update()
    {
        if (heldObject == null)
            HandlePickupDetection();
        else
            HandleHeldObject();
    }

    private void HandlePickupDetection()
    {
        if (playerCamera == null) return;

        Throwable throwable = null;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, pickupRange))
        {
            throwable = hit.collider.GetComponent<Throwable>()
                     ?? hit.collider.GetComponentInParent<Throwable>()
                     ?? hit.collider.GetComponentInChildren<Throwable>();

            Debug.Log($"[Pickup] Hit: {hit.collider.name}, Throwable found: {throwable != null}");
        }

        bool lookingAtThrowable = throwable != null;

        if (pickupPrompt != null)
            pickupPrompt.gameObject.SetActive(lookingAtThrowable);

        if (lookingAtThrowable && Input.GetKeyDown(pickupKey))
        {
            heldObject = throwable;
            heldObject.PickUp(holdPoint);
            if (pickupPrompt != null) pickupPrompt.gameObject.SetActive(false);
        }
    }

    private void HandleHeldObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            heldObject.Throw(playerCamera.forward, throwSpeed, throwLoft);
            heldObject = null;
        }
        else if (Input.GetKeyDown(pickupKey))
        {
            heldObject.Drop();
            heldObject = null;
        }
    }
}
