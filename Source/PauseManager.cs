using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public Player thePlayer;
    public Accountant theAccountant;
    public NotificationCenter notificationCenter;
    public AudioSource audioSource;
    public TutorialSpeechManager tutorialSpeechManager;

    public AudioClip menuSelectSound;
    public AudioClip menuChooseSound;

    private Canvas pauseCanvas;

    private const int SAVE_GAME_OPTION = 0;
    private const int LOAD_GAME_OPTION = 1;
    private const int QUIT_GAME_OPTION = 2;

    /* Computer Options */
    private Text[] pauseInteractionOptions;
    private int pauseIntOptionSelected = 0;
    private bool pauseOptionsDisplayed = false;

    bool menuCanInteract = true;

    void Awake() {
        audioSource.clip = menuSelectSound;
    }

    public void triggerComputerInteraction() {
        displayPauseMenu();
        float money = thePlayer.getPlayerMoney();
        pauseIntOptionSelected = SAVE_GAME_OPTION;

        for (int i = 0; i < pauseInteractionOptions.Length; i++) {
            pauseInteractionOptions[i].color = Color.white;
        }

        if (pauseIntOptionSelected == SAVE_GAME_OPTION) {
            pauseInteractionOptions[SAVE_GAME_OPTION].color = Color.yellow;
        }

        if (pauseIntOptionSelected == LOAD_GAME_OPTION) {
            pauseInteractionOptions[LOAD_GAME_OPTION].color = Color.yellow;
        }

        if (pauseIntOptionSelected == QUIT_GAME_OPTION) {
            pauseInteractionOptions[QUIT_GAME_OPTION].color = Color.yellow;
        }

        //this.gameObject.SetActive(true);
        pauseOptionsDisplayed = true;
        this.thePlayer.disablePlayerMovement();
    }

    private void handleInteractionMenu() {
        if (gameObject.activeSelf && pauseOptionsDisplayed) {
            bool activatePressed = Input.GetButtonDown("Activate");
            if (activatePressed) {
                activatePressed = false;
               
                switch (this.pauseIntOptionSelected) {
                    case SAVE_GAME_OPTION:
                        PlayerState.writeSaveFile(thePlayer, theAccountant, tutorialSpeechManager);
                        hidePauseMenu();
                        thePlayer.unfreezePlayer();
                        thePlayer.unpause();
                        pauseOptionsDisplayed = false;
                        this.thePlayer.enablePlayerMovement();
                        notificationCenter.printMessage("Game saved.");
                        break;
                    case LOAD_GAME_OPTION:
                        if (System.IO.File.Exists("save.dat")) {
                            PlayerState.loadSave();
                            SceneManager.LoadScene("Overworld");
                        }
                        break;
                    case QUIT_GAME_OPTION:
                        //Steamworks.SteamAPI.Shutdown();
                        //Application.Quit();
                        SceneManager.LoadScene("GameIntro");
                        break;
                }
                audioSource.clip = menuChooseSound;
                audioSource.Play();
                audioSource.clip = menuSelectSound;
            } else if ((Input.GetKeyDown(KeyCode.W) || (Input.GetAxis("Vertical") > 0)) && menuCanInteract) {
                this.menuCanInteract = false;
                StartCoroutine(MenuChange(this.pauseIntOptionSelected - 1));
            } else if ((Input.GetKeyDown(KeyCode.S) || (Input.GetAxis("Vertical") < 0)) && menuCanInteract) {
                this.menuCanInteract = false;
                StartCoroutine(MenuChange(this.pauseIntOptionSelected + 1));
            }
        }
    }

    IEnumerator MenuChange(int option) {
        selectOptionOnPauseMenu(option);        
        yield return new WaitForSeconds(0.3f);
        this.menuCanInteract = true;
    }

    public void selectOptionOnPauseMenu(int option) {
        if (option < SAVE_GAME_OPTION) {
            option = QUIT_GAME_OPTION;
        }

        if (option > QUIT_GAME_OPTION) {
            option = SAVE_GAME_OPTION;
        }

        for (int i = 0; i < pauseInteractionOptions.Length; i++) {
            if (option == i) {
                pauseInteractionOptions[option].color = Color.yellow;
                this.pauseIntOptionSelected = option;
            } else {
                pauseInteractionOptions[i].color = Color.white;
            }
        }

        audioSource.Play();
    }

    void OnDestroy() {
        Debug.Log("Destroyed");
    }

    // Start is called before the first frame update
    void Start() {
        // Pause Menu Canvas
        pauseInteractionOptions = this.gameObject.transform.Find("Canvas").transform.Find("PauseMenuOptions").GetComponentsInChildren<Text>();

        pauseCanvas = GameObject.Find("/Main Camera/PauseMenu").GetComponentInChildren<Canvas>();
        pauseCanvas.enabled = false;
    }

    public void displayPauseMenu() {
        pauseCanvas.enabled = true;
    }

    public void hidePauseMenu() {
        pauseCanvas.enabled = false;
        pauseOptionsDisplayed = false;
    }

    // Update is called once per frame
    void Update() {
        handleInteractionMenu();
    }
}
