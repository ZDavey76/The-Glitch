using UnityEngine;
using System.Collections;

public class BouncingBall : MonoBehaviour
{
    private Rigidbody sphereRigidbody;
    public GameObject player;
    public GameObject ball;
    public float thrust; 
    bool enter = false;
    bool holding = false;
    bool canHoldBall = false; 
  


    void Start()
    {
        sphereRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canHoldBall == true)
        {
            holding = true; 
            //Makes the GameObject "newParent" the parent of the GameObject "player".
            ball.transform.parent = player.transform;

            //Display the parent's name in the console.
            Debug.Log("Player's Parent: " + ball.transform.parent.name);

            // Check if the new parent has a parent GameObject.
            if (player.transform.parent != null)
            {
                //Display the name of the grand parent of the player.
                Debug.Log("Player's Grand parent: " + ball.transform.parent.parent.name);
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && holding == true)
        {
            transform.parent = null; // drops the object
            sphereRigidbody.AddRelativeForce(Vector3.forward * thrust); // meant to throw the object 
        }
    } 
    void OnGUI()
    {
        if (enter)
        {
            // lets the player know they can pick it up
            GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height - 100, 155, 30), "Press 'F' to hold the ball.");
            canHoldBall = true;
        }
    }

    public void DetachFromParent()
    {
        // Detaches the transform from its parent.
        transform.parent = null;
    }

    // Activate the Main function when Player enter the trigger area
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enter = true;
        }
    }

    // Deactivate the Main function when Player exit the trigger area
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enter = false;
            canHoldBall = false; 
            transform.parent = null;
        }
    }
}