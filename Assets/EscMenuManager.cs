using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EscMenuManager : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Set this in Inspector

    private UIDocument _document;
    private VisualElement _menuRoot;
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;
    private PlayerInputActions _inputActions;
    public MonoBehaviour[] scriptsToDisable; // Set these in the Inspector


    private bool _isMenuOpen = false;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument not found on this GameObject!");
            return;
        }

        _menuRoot = _document.rootVisualElement.Q<VisualElement>("EscMenu");
        if (_menuRoot == null)
        {
            Debug.LogError("Could not find VisualElement named 'EscMenu' in UXML!");
            return;
        }

        _resumeButton = _menuRoot.Q<Button>("ResumeButton");
        _settingsButton = _menuRoot.Q<Button>("SettingsButton");
        _quitButton = _menuRoot.Q<Button>("QuitButton");
    
        if (_resumeButton == null) Debug.LogError("ResumeButton not found");
        if (_settingsButton == null) Debug.LogError("SettingsButton not found");
        if (_quitButton == null) Debug.LogError("QuitButton not found");

        _resumeButton.clicked += ToggleMenu;
        _settingsButton.clicked += OnSettingsClicked;
        _quitButton.clicked += OnQuitClicked;

        _menuRoot.style.display = DisplayStyle.None;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("esc pressed");
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        _isMenuOpen = !_isMenuOpen;
        _menuRoot.style.display = _isMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;

        // Lock/unlock the cursor
        UnityEngine.Cursor.lockState = _isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = _isMenuOpen;


        // Disable or enable the scripts
        foreach (var script in scriptsToDisable)
        {
            script.enabled = !_isMenuOpen;
        }
    }


    private void OnSettingsClicked()
    {
        Debug.Log("Settings clicked in ESC menu");
        // Open settings panel here
    }

    private void OnQuitClicked()
    {
        Debug.Log("Returning to main menu...");
        Time.timeScale = 1f;
        _inputActions.Disable();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        _inputActions?.Dispose();
    }

}
