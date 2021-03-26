using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SolutionHandler : NetworkBehaviour {

    //public GameObject stats;
    GameObject networkMan;
    bool canUseCards;
    public Text solution;
    public Text category;

    [SyncVar]
    SolCard current;

    [SyncVar] string currentCat;
    [SyncVar] string currentSol;

    public class SolCard {

        public string category { get; set; }
        public string solution { get; set; }

        public SolCard(string sol, string cat) {
            solution = sol;
            category = cat;
        }

        public SolCard() {
            solution = "";
            category = "";
        }
    }

    List<SolCard> solutions = new List<SolCard>();

    void Start() {

        if (SceneManager.GetActiveScene().name.Equals("game")) {

            category = GameObject.Find("CategoryText").GetComponent<Text>();
            solution = GameObject.Find("SolutionText").GetComponent<Text>();

            networkMan = GameObject.Find("NetworkManager");
            canUseCards = networkMan.GetComponent<LobbyScene>().currentPlayer.canUseCards;

            if (isServer) {
                solutions.Add(new SolCard("statue of liberty", "object"));
                solutions.Add(new SolCard("bermuda triangle", "geography"));
                solutions.Add(new SolCard("surfing", "sports & hobbies"));
                solutions.Add(new SolCard("eiffel tower", "geography"));
                solutions.Add(new SolCard("guillotine", "object"));
                solutions.Add(new SolCard("fail an exam", "activity"));

                int index = Random.Range(0, solutions.Count);
                current = solutions[index];


                currentSol = current.solution;
                currentCat = current.category;
            }

            category.text = currentCat;
            if (isServer)
            {
                solution.text = currentSol;
                GameObject.Find("GuessPanel").SetActive(false);
            }
            else
            {
                solution.text = "";
            }
        }
    }

    public void LoseGame() {
        solution.text = currentSol;
        StartCoroutine(Wait());

        networkMan.GetComponent<LobbyScene>().ChangeScene("lobby2");
    }

    public void WinGame() {
        this.GetComponent<CountDown>().isEnabled = false;
        solution.text = currentSol;
        if (isServer)
            networkMan.GetComponent<LobbyScene>().currentPlayer.Score += 1;

        StartCoroutine(Wait());
        networkMan.GetComponent<LobbyScene>().ChangeScene("lobby2");
    }

    public void GuessInput(string answ) {
        string guess = answ.ToLower();
        if (!isServer) {
            CmdGuess(guess);
        }
        else {
            if (guess.Equals(current.solution)) {
                WinGame();
                RpcGuess();
            }
        }
    }

    [Command]
    public void CmdGuess(string guess) {
        if (guess.Equals(current.solution)) {
            WinGame();
            RpcGuess();
        }
    }

    [ClientRpc]
    public void RpcGuess() {
        WinGame();
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(3.0f);
    }
}
