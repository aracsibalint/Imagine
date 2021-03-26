using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LobbyScene;
using UnityEngine.Networking;

public class LobbyPanel : NetworkBehaviour {

    Scene sceneLoaded;
    List<Player> joined = new List<Player>();
    Player current;

    string playersPanelText = "";

    public void DisableStartBtn() {
        GameObject.Find("ButtonStartHost").GetComponent<Button>().enabled = false;
    }

    private void Start() {
        
        this.transform.SetParent(GameObject.Find("Canvas").transform);
        
            
        
        sceneLoaded = SceneManager.GetActiveScene();
        var solutionHandlerObject = GameObject.Find("SolutionHandlerObject");

        if (sceneLoaded.name.Equals("lobby2"))
        {
            GameObject.Find("JoinedPlayersPanel").transform.SetParent(this.transform);
            solutionHandlerObject.SetActive(false);
        }
            
        else if (sceneLoaded.name.Equals("game"))
        {
            Screen.SetResolution(962, 536, false);
            solutionHandlerObject.SetActive(true);
            this.GetComponent<Image>().enabled = false;
            GameObject.Find("ButtonStartHost").SetActive(false);
            GameObject.Find("ButtonBack").SetActive(false);
            GameObject.Find("Title").SetActive(false);
            //GameObject.Find("PlayersText").SetActive(false);
        }        
      
    }

    public override void OnStartLocalPlayer() { 

        current = GameObject.Find("NetworkManager").GetComponent<LobbyScene>().currentPlayer;
        if (!isServer) {
            CmdAddPlayer(current);
        }
        else {
            GameObject.Find("NetworkManager").GetComponent<LobbyScene>().joinedPlayers.Add(current);
            joined.Add(current);
            playersPanelText += string.Format("{0} : {1} \n", current.name, current.Score);
            RpcAddPlayer(joined.ToArray(), playersPanelText);
        }

    }

    [Command]
    public void CmdAddPlayer(Player p) {
        if (!joined.Contains(p)) {
            joined.Add(p);
            GameObject.Find("NetworkManager").GetComponent<LobbyScene>().joinedPlayers.Add(p);
            playersPanelText += string.Format("{0} : {1} \n", p.name, p.Score);
            RpcAddPlayer(joined.ToArray(), playersPanelText);
        }
    }

    [ClientRpc]
    public void RpcAddPlayer(Player[] p, string str) {
        joined.Clear();
        joined.AddRange(p);
        GameObject.Find("NetworkManager").GetComponent<LobbyScene>().joinedPlayers.Clear();
        GameObject.Find("NetworkManager").GetComponent<LobbyScene>().joinedPlayers.AddRange(p);
        playersPanelText = str;
    }


    public void StartGame() {

        if (isServer) {
            GameObject networkMan = GameObject.Find("NetworkManager");
            networkMan.GetComponent<LobbyScene>().SetRoles();

            joined.Clear();
            joined.AddRange(networkMan.GetComponent<LobbyScene>().joinedPlayers);

            GameObject.Find("NetworkManager").GetComponent<LobbyScene>().ChangeScene("game");
        }
    }

    [ClientRpc]
    public void RpcUpdateSelected(Player p) {
        var networkJoined = GameObject.Find("NetworkManager").GetComponent<LobbyScene>().joinedPlayers;
        foreach (var item in networkJoined) {
            if (item.name.Equals(p.name))
                item.canUseCards = true;
            else
                item.canUseCards = false;
        }

        joined.Clear();
        joined.AddRange(networkJoined);

    }

    public void Disconnect() {
        if (isServer)
            GameObject.Find("NetworkManager").GetComponent<LobbyScene>().DisconnectHost();
        else
            GameObject.Find("NetworkManager").GetComponent<LobbyScene>().DisconnectClient();
    }

    private void Update() {
        if(sceneLoaded.name.Equals("lobby2"))
        GameObject.Find("PlayersText").GetComponent<Text>().text = playersPanelText;
    }

}
