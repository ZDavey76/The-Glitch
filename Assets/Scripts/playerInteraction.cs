using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public InputActionReference interactAction;
    public float interactRange = 3f;
    public Transform holdPosition; // Empty GameObject in front of camera
    public LayerMask interactableLayer;
    
    [Header("Hold Settings")]
    public float holdDistance = 2f;
    public float holdSmoothSpeed = 10f;
    public float throwForce = 10f;
    public InputActionReference throwAction;
    
    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private bool isHolding = false;
    
    [Header("UI Feedback")]
    public GameObject interactPrompt; // Optional: UI text "Press E to pick up"

    void Update()
    {
        if (!isHolding)
        {
            CheckForInteractable();
        }
        else
        {
            HoldObject();
            
            // Throw object
            if (throwAction.action.WasPressedThisFrame())
            {
                ThrowObject();
            }
        }
        
        // Pick up / Drop
        if (interactAction.action.WasPressedThisFrame())
        {
            if (!isHolding)
            {
                TryPickup();
            }
            else
            {
                DropObject();
            }
        }
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of screen
        
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                // Show prompt
                if (interactPrompt != null)
                    interactPrompt.SetActive(true);
            }
        }
        else
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    void TryPickup()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                PickupObject(hit.collider.gameObject);
            }
        }
    }

    void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldObjectRb = obj.GetComponent<Rigidbody>();
        
        if (heldObjectRb != null)
        {
            heldObjectRb.useGravity = false;
            heldObjectRb.linearDamping = 10f; // Dampen movement
            heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent spinning
        }
        
        isHolding = true;
        
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void HoldObject()
    {
        if (heldObject == null) return;
        
        // Calculate target position in front of camera
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
        
        // Smoothly move object to hold position
        Vector3 direction = targetPosition - heldObject.transform.position;
        
        if (heldObjectRb != null)
        {
            heldObjectRb.linearVelocity = direction * holdSmoothSpeed;
        }
    }

    void DropObject()
    {
        if (heldObject == null) return;
        
        if (heldObjectRb != null)
        {
            heldObjectRb.useGravity = true;
            heldObjectRb.linearDamping = 0f;
            heldObjectRb.constraints = RigidbodyConstraints.None;
        }
        
        heldObject = null;
        heldObjectRb = null;
        isHolding = false;
    }

    void ThrowObject()
    {
        if (heldObject == null) return;
        
        if (heldObjectRb != null)
        {
            heldObjectRb.useGravity = true;
            heldObjectRb.linearDamping = 0f;
            heldObjectRb.constraints = RigidbodyConstraints.None;
            
            // Add throw force
            Vector3 throwDirection = Camera.main.transform.forward;
            heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        }
        
        heldObject = null;
        heldObjectRb = null;
        isHolding = false;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize interact range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactRange);
    }
}