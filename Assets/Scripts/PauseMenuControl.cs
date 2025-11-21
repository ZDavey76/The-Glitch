using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused = false;
    
    [Header("UI")]
    public GameObject pauseMenuUI;
    
    [Header("Input")]
    public InputActionReference pauseAction;
    
    private Rigidbody[] allRigidbodies;
    private Vector3[] savedVelocities;
    private Vector3[] savedAngularVelocities;
    private bool[] wasKinematic;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void OnEnable()
    {
        if (pauseAction != null)
            pauseAction.action.Enable();
    }

    void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.action.Disable();
    }

    void Update()
    {
        if (pauseAction != null && pauseAction.action.WasPressedThisFrame())
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("PAUSING - Physics frozen, player can still move!");
        
        // Find all rigidbodies
        allRigidbodies = FindObjectsOfType<Rigidbody>();
        savedVelocities = new Vector3[allRigidbodies.Length];
        savedAngularVelocities = new Vector3[allRigidbodies.Length];
        wasKinematic = new bool[allRigidbodies.Length];
        
        // Freeze all physics
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            savedVelocities[i] = allRigidbodies[i].linearVelocity;
            savedAngularVelocities[i] = allRigidbodies[i].angularVelocity;
            wasKinematic[i] = allRigidbodies[i].isKinematic;
            
            allRigidbodies[i].linearVelocity = Vector3.zero;
            allRigidbodies[i].angularVelocity = Vector3.zero;
            allRigidbodies[i].isKinematic = true;
        }
        
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        
        isPaused = true;
        
        // Show cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        Debug.Log("RESUMING - Physics restored!");
        
        // Restore all physics
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            if (allRigidbodies[i] != null)
            {
                allRigidbodies[i].isKinematic = wasKinematic[i];
                
                if (!wasKinematic[i])
                {
                    allRigidbodies[i].linearVelocity = savedVelocities[i];
                    allRigidbodies[i].angularVelocity = savedAngularVelocities[i];
                }
            }
        }
        
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        
        isPaused = false;
        
        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        // Change "MainMenu" to your actual main menu scene name
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu_Scene");
    }
}