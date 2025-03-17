using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class networkManagerUI : MonoBehaviour
{
    private UIDocument _document;
    private Button _server;
    private Button _client;
    private Button _host;
    private List<Button> _serverUIButtons = new List<Button>();

    private void Awake()
    {
        
        _document = GetComponent<UIDocument>();
        
        _server = _document.rootVisualElement.Q("Server") as Button;
        _server.RegisterCallback<ClickEvent>(onServerClick);
        _host = _document.rootVisualElement.Q("Host") as Button;
        _host.RegisterCallback<ClickEvent>(onServerClick);
        _client = _document.rootVisualElement.Q("Client") as Button;
        _client.RegisterCallback<ClickEvent>(onServerClick);

        // _serverUIButtons = _document.rootVisualElement.Query<Button>().ToList();
        // for (int i = 0; i < _serverUIButtons.Count; i++)
        // {
        //     _serverUIButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClick);
        //     if(i == 0) { }
        // }

        
    }
    private void OnDisable()
    {
        _server.UnregisterCallback<ClickEvent>(onServerClick);

        for (int i = 0; i < _serverUIButtons.Count; i++)
        {
            _serverUIButtons[i].UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void onServerClick(ClickEvent evt)
    {
        if (evt.currentTarget.Equals(_server))
        {
            NetworkManager.Singleton.StartServer();
        };

        if (evt.currentTarget.Equals(_client))
        {
            NetworkManager.Singleton.StartClient();
        };

        if (evt.currentTarget.Equals(_host))
        {
            NetworkManager.Singleton.StartHost();
        };
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {

    }
    
}
