using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CountDown : NetworkBehaviour {
    [SyncVar]
    public float timeStart = 120.0F;
    public Text textBox;
    Scene sceneLoaded;

    public bool isEnabled { get; set; }

    void Start() {
        sceneLoaded = SceneManager.GetActiveScene();
      if (sceneLoaded.name.Equals("game")) { 
            textBox = GameObject.Find("CountDownText").GetComponent<Text>();
        textBox.text = timeStart.ToString("F1");
        isEnabled = true;
        }
    }

    void Update() {
        if (sceneLoaded.name.Equals("game")) {
            if (isEnabled && timeStart > 0.0F) {
                timeStart -= Time.deltaTime;
                textBox.text = timeStart.ToString("F1");
            }
            if (isEnabled && timeStart <= 0.0F) {
                this.GetComponent<SolutionHandler>().LoseGame();
            }
        }
    }
}