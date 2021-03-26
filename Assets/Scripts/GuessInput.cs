using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuessInput : MonoBehaviour
{
    public InputField input;

    public void OnChanged() {
        var playerobject = GameObject.Find("NetworkManager").GetComponent<LobbyScene>().GetPlayerObject();
        playerobject.GetComponent<SolutionHandler>().CmdGuess(input.text);
    }

  
}
