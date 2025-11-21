/*
***********************The GliTcH******************************

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController myController;
    public float speed;
    public InputActionReference moveAction;

    private Vector3 currentMovement;

    public InputActionReference lookAction;
    private Vector2 rotStore;
    public float lookSpeed;
    public Camera theCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        //Debug.Log("Move Input: " + moveInput);

        Vector3 moveForward = transform.forward * moveInput.y;
        Vector3 moveSideways = transform.right * moveInput.x;
        currentMovement = (moveForward + moveSideways) * speed;
        myController.Move(currentMovement * Time.deltaTime);

        //looking
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        lookInput.y = -lookInput.y;

        rotStore = rotStore + (lookInput * lookSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, rotStore.x, 0f);
        theCam.transform.localRotation = Quaternion.Euler(rotStore.y, 0f, 0f);
    }
}
*/