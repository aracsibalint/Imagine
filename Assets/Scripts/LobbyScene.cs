using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyScene : NetworkManager {
    public class Player {

        public bool canUseCards = true;

        public string name { get; set; }

        private int score = 0;
        public int Score { get => score; set { if (value >= 0) score = value; } }

        public Player(string n) {
            name = n;
        }

        public Player() {
            name = "";
        }
    }

    protected int currentPlayers;

    public List<Player> joinedPlayers = new List<Player>();

    public Player currentPlayer;

    public void StartupHost() {
        SetPort();
        string name = GameObject.Find("InputFieldName").transform.Find("Text").GetComponent<Text>().text;
        currentPlayer = new Player(name);
        joinedPlayers.Add(currentPlayer);
        Debug.Log("NetworkManager name is: " + name);
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame() {
        SetIPAddress();
        SetPort();
        string name = GameObject.Find("InputFieldName").transform.Find("Text").GetComponent<Text>().text;
        currentPlayer = new Player(name);
        NetworkManager.singleton.StartClient();
    }

    void SetIPAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        if (ipAddress.Equals(""))
            ipAddress = "localhost";
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    // Update is called once per frame
    void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void SetRoles() {
        int index = Random.Range(0, joinedPlayers.Count);
        var selected = joinedPlayers[index];

        foreach (var item in joinedPlayers) {
            if (item.name.Equals(selected.name))
                item.canUseCards = true;
            else
                item.canUseCards = false;
        }


    }

    public void DisconnectHost() {
        NetworkManager.singleton.StopHost();
    }

    public void DisconnectClient() {
        NetworkManager.singleton.StopClient();
    }

    public void ChangeScene(string sceneName) {
        NetworkManager.singleton.ServerChangeScene(sceneName);
    }

    public GameObject GetPlayerObject() {
        NetworkManager networkManager = NetworkManager.singleton;

        List<PlayerController> pc = networkManager.client.connection.playerControllers;

        for (int i = 0; i < pc.Count; i++) {

            if (pc[i].IsValid) {
                if (pc[i].gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
                    return pc[i].gameObject;
            }

        }
        return null;
    }


}
