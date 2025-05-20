using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Mono.Cecil.Cil;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class networkManagerUI : NetworkBehaviour
{
    private UIDocument _document;
    private Button _server;
    private Button _client;
    private Button _host;
    public TextField networkRole;
    public String role;
    private List<Button> _serverUIButtons = new List<Button>();
    
    public Label roleText;
    public Material playerColor;
    public Color roleColor;


    private void debugMessage()
{
    Debug.Log($"IsServer: {NetworkManager.Singleton.IsServer}");
    Debug.Log($"IsHost: {NetworkManager.Singleton.IsHost}");
    Debug.Log($"IsClient: {NetworkManager.Singleton.IsClient}");

    if (IsServer) // only server/host can access ConnectedClients
    {
        Debug.Log("Running on server.");
        Debug.Log($"OwnerClientId: {OwnerClientId}");
        Debug.Log($"Client in ConnectedClients? {NetworkManager.Singleton.ConnectedClients.ContainsKey(OwnerClientId)}");
    }
    else
    {
        Debug.Log("Running on client. Skipping server-only checks.");
    }

    roleTextUpdate();
}

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


    }

    public void roleTextUpdate()
    {
        roleText.text = role;
    }

    private void OnDisable()
    {
        _server.UnregisterCallback<ClickEvent>(onServerClick);
        NetworkManager.Singleton.Shutdown();

    }

    private void onServerClick(ClickEvent evt)
    {
        if (evt.currentTarget.Equals(_server))
        {
            NetworkManager.Singleton.StartServer();
            role = "client";
        }
        ;

        if (evt.currentTarget.Equals(_client))
        {
            NetworkManager.Singleton.StartClient();
            role = "Client";
            roleColor = Color.green;
            playerColor.color = roleColor;
        }
        ;

        if (evt.currentTarget.Equals(_host))
        {
            NetworkManager.Singleton.StartHost();
            role = "Host";
            roleColor = Color.red;
            playerColor.color = roleColor;
        };
        InvokeRepeating("debugMessage", 0f, 5f);

    }

}
