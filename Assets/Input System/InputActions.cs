using UnityEngine;
using UnityEngine.InputSystem;

public class TestingInputSystem : MonoBehaviour
{
    private Rigidbody capsuleRigidbody;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private float speed;

    private void Awake()
    {
        capsuleRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();


        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;

    }

    private void Update()
    {
        /*Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        float speed = 5f;
        capsuleRigidbody.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * speed, ForceMode.Force);*/
        Vector2 direction = playerInputActions.Player.Movement.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * 5f;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if (context.performed)
        {

            Debug.Log("Jump!" + context.phase);
            capsuleRigidbody.AddForce(Vector3.up * 7.5f, ForceMode.Impulse);
        }
    }
}