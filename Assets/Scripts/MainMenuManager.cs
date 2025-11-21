using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [Header("Setup")]
    [SerializeField] private string levelToLoad;
    [SerializeField] private bool debugging = false;
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject levelsUI;
    [SerializeField] GameObject settingsUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Instance already exists, destroying duplicate.");
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        mainMenuUI.SetActive(true);
        levelsUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    public void OnStartButton()
    {
        Debug.Log("Start button clicked");
        if (debugging)
            Debug.Log("Loading level: " + levelToLoad);

        SceneManager.LoadScene(levelToLoad);
    }
    public void OnLevelsButton()
    {
        mainMenuUI.SetActive(false);
        levelsUI.SetActive(true);
        Debug.Log("Levels button clicked");
    }
    public void OnSettingsButton()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        Debug.Log("Settings button clicked");
    }
    public void OnExitButton()
    {
        Debug.Log("Exit button clicked");
        QuitGame();
    }
    public void OnBackToMainMenuFromLevels()
    {
        levelsUI.SetActive(false);
        mainMenuUI.SetActive(true);
        Debug.Log("Back to Main Menu from Levels clicked");
    }
    public void OnBackToMainMenuFromSettings()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
        Debug.Log("Back to Main Menu from Settings clicked");
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
        if (debugging)
            Debug.Log("Quit Game called");
    }



}
