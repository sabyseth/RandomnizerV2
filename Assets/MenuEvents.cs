using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuEvents : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GameScene"; // Set this in the Inspector

    private UIDocument _document;

    private Button _startButton;
    private Button _settingsButton;
    private Button _quitButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _startButton = _document.rootVisualElement.Q<Button>("StartGameButton");
        _settingsButton = _document.rootVisualElement.Q<Button>("SettingsButton");
        _quitButton = _document.rootVisualElement.Q<Button>("QuitButton");

        if (_startButton != null)
            _startButton.RegisterCallback<ClickEvent>(OnStartGameClick);
        else
            Debug.LogError("StartGameButton not found");

        if (_settingsButton != null)
            _settingsButton.RegisterCallback<ClickEvent>(OnSettingsClick);
        else
            Debug.LogError("SettingsButton not found");

        if (_quitButton != null)
            _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);
        else
            Debug.LogError("QuitButton not found");
    }

    private void OnDisable()
    {
        if (_startButton != null)
            _startButton.UnregisterCallback<ClickEvent>(OnStartGameClick);

        if (_settingsButton != null)
            _settingsButton.UnregisterCallback<ClickEvent>(OnSettingsClick);

        if (_quitButton != null)
            _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);
    }

    private void OnStartGameClick(ClickEvent evt)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name not set!");
        }
    }

    private void OnSettingsClick(ClickEvent evt)
    {
        Debug.Log("Settings clicked");
        // Add settings logic here
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("Quit clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
