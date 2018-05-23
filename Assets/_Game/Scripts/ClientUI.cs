using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientUI : MonoBehaviour {

    public Client clientController;
    public GameObject StartScreen;
    public GameObject EscapeMenu;
    public GameObject HUD;
    public Text escapeMenuStatusText;
    public GameManager gameManager;

    private bool started = false;
    private float stopConnectionTime;

    public void Start() {
        stopConnectionTime = Time.time + 10f;

        EscapeMenu.SetActive(false);
        HUD.SetActive(false);
        StartScreen.SetActive(true);

    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (EscapeMenu.activeSelf) {
                EscapeMenu.SetActive(false);
            }
            else {
                EscapeMenu.SetActive(true);
            }
        }
        
        if (started == false) {
            if (clientController.PlayerAvatarCreated == false) {
                if (Time.time > stopConnectionTime) {
                    StartScreen.SetActive(false);
                    escapeMenuStatusText.text = "Connection Timed Out. Please Quit And Try Again.";
                    EscapeMenu.SetActive(true);
                }
            }
            else {
                HUD.SetActive(true);
                StartScreen.SetActive(false);
                started = true;
                /*
                GameObject localPlayerShipObj = GameObject.Find("LocalPlayerShip");
                if (localPlayerShipObj == null) {
                    Debug.LogError("couldn't find LocalPlayerShip for the game manager");
                }
                gameManager.localPlayerShip = localPlayerShipObj.GetComponent<LocalPlayerShip>();
                */
            }
        }
        
        
    }


    public void QuitGame() {
        // do some stuff before closing client instance

        SceneManager.LoadScene(0);
    }
}
