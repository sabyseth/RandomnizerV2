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

    private bool _isMenuOpen = false;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        
        _resumeButton = _menuRoot.Q<Button>("ResumeButton");
        _settingsButton = _menuRoot.Q<Button>("SettingsButton");
        _quitButton = _menuRoot.Q<Button>("QuitButton");

        _resumeButton.clicked += ToggleMenu;
        _settingsButton.clicked += OnSettingsClicked;
        _quitButton.clicked += OnQuitClicked;

        _menuRoot.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        _isMenuOpen = !_isMenuOpen;
        _menuRoot.style.display = _isMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;
        Time.timeScale = _isMenuOpen ? 0f : 1f;
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
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
