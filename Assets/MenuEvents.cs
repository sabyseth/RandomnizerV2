using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class MenuEvents : MonoBehaviour
{
    private UIDocument _document;

    private Button _button;

    private void Awake(){
    _document = GetComponent<UIDocument>();

    _button = _document.rootVisualElement.Q("StartGameButton") as Button;
    _button.RegisterCallback<ClickEvent>(OnplayGameClick);
    }

    private void OnDisable(){
        _button.UnregisterCallback<ClickEvent>(OnplayGameClick);
    }

    private void OnplayGameClick(ClickEvent evt){
        Debug.Log("Start");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
