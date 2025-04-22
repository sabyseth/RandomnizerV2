using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class networkManagerUI : NetworkBehaviour
{
    //[SerializeField] private String Role;

    private UIDocument _document;
    private Button _server;
    private Button _client;
    private Button _host;
    public TextField networkRole;
    public String role;
    private List<Button> _serverUIButtons = new List<Button>();
    
    public Label roleText;

    private void Awake()
    {
        
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        roleText = root.Q<Label>("NetworkRole");
        roleText = root.Q("NetworkRole") as Label;

        _server = root.Q("Server") as Button;
        _server.RegisterCallback<ClickEvent>(onServerClick);
        _host = root.Q("Host") as Button;
        _host.RegisterCallback<ClickEvent>(onServerClick);
        _client = root.Q("Client") as Button;
        _client.RegisterCallback<ClickEvent>(onServerClick);

        // _serverUIButtons = root.Query<Button>().ToList();
        // for (int i = 0; i < _serverUIButtons.Count; i++)
        // {
        //     _serverUIButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClick);
        //     if(i == 0) { }
        // }
        InvokeRepeating("SendDebugMessage", 0f, 5f);
        
    }
        
    public void SendDebugMessage()
    {
        Debug.Log(role);
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
            Debug.Log("Server Clicked");
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
    // void Update()
    // {

    // }
}
