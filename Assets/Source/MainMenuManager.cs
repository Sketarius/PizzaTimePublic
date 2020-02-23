using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public SpriteRenderer companyLogo;
    public SpriteRenderer gameLogo;
    public SpriteRenderer brickBk;

    public Canvas fadeOutCanvas;
    public Canvas gameMenuCanvas;

    public AudioSource audioSource;
    public AudioClip menuSelectSound;
    public AudioClip menuChooseSound;

    private Text[] mainMenuOptions;
    private int mainMenuOptionSelected = 0;
    private const int NEW_GAME_OPTION = 0;
    private const int LOAD_GAME_OPTION = 1;
    private const int QUIT_GAME_OPTION = 2;

    bool readyToStart = false;
    bool menuCanInteract = true;

    void Awake() {

    }

    // Start is called before the first frame update
    void Start() {
        mainMenuOptions = gameMenuCanvas.GetComponentsInChildren<Text>();

        // Set menu select sound as default.
        audioSource.clip = menuSelectSound;
        StartCoroutine(fadeLogos());
    }

    // Update is called once per frame
    void Update() {
        if(readyToStart) {
            /*if(Input.GetButtonDown("Submit")) {
                //StopAllCoroutines();
                StartCoroutine(fadeToBlack());                
            }*/
            // Handle being able to select options from main menu, duh.
            handleInteractionMenu();
        }
    }

    private void handleInteractionMenu() {

        foreach (Text option in mainMenuOptions) {
            option.color = Color.white;
        }

        if (mainMenuOptionSelected == NEW_GAME_OPTION) {
            mainMenuOptions[NEW_GAME_OPTION].color = Color.yellow;
        }

        if (mainMenuOptionSelected == LOAD_GAME_OPTION) {
            mainMenuOptions[LOAD_GAME_OPTION].color = Color.yellow;
        }

        if (mainMenuOptionSelected == QUIT_GAME_OPTION) {
            mainMenuOptions[QUIT_GAME_OPTION].color = Color.yellow;
        }

        bool activatePressed = (Input.GetButtonDown("Activate") || Input.GetButtonDown("Submit"));
        if (activatePressed) {
            activatePressed = false;

            audioSource.clip = menuChooseSound;
            audioSource.Play();

            switch (mainMenuOptionSelected) {
                case LOAD_GAME_OPTION:
                    loadLastSave();
                    break;
                case QUIT_GAME_OPTION:
                    Steamworks.SteamAPI.Shutdown();
                    Application.Quit();
                    break;
            }

           StartCoroutine(fadeToBlack());

        } else if ((Input.GetKeyDown(KeyCode.W) || (Input.GetAxis("Vertical") > 0)) && menuCanInteract) {
            this.menuCanInteract = false;
            StartCoroutine(MenuChange(mainMenuOptionSelected - 1));
        } else if ((Input.GetKeyDown(KeyCode.S) || (Input.GetAxis("Vertical") < 0)) && menuCanInteract) {
            this.menuCanInteract = false;
            StartCoroutine(MenuChange(mainMenuOptionSelected + 1));
        }
    }

    IEnumerator MenuChange(int option) {
        selectOptionOnMenu(option);
        yield return new WaitForSeconds(0.3f);
        this.menuCanInteract = true;
    }

    private void playMenuSelectSound() {
        audioSource.Play();
    }

    public void selectOptionOnMenu(int option) {
        if (option < NEW_GAME_OPTION) {
            option = QUIT_GAME_OPTION;
        }

        if (option > QUIT_GAME_OPTION) {
            option = NEW_GAME_OPTION;
        }

        for (int i = 0; i < mainMenuOptions.Length; i++) {
            if (option == i) {
                mainMenuOptions[option].color = Color.yellow;
                mainMenuOptionSelected = option;
            } else {
                mainMenuOptions[i].color = Color.white;
            }
        }
        // Play sound.
        playMenuSelectSound();
    }

    IEnumerator fadeToBlack() {
        UnityEngine.UI.Image blackPanel = fadeOutCanvas.GetComponentInChildren<UnityEngine.UI.Image>();
        float alpha = 0.0f;

        while (blackPanel.color.a < 1.1f) {
            blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, alpha);
            alpha = alpha + 0.5f;
            yield return new WaitForSeconds(0.1f);
        }

        SceneManager.LoadScene("Overworld");
    }

    private void loadLastSave() {
        if (System.IO.File.Exists("save.dat")) {
            PlayerState.loadSave();
            Debug.Log("Save loaded!");
        }
    }

    IEnumerator fadeLogos() {
        float companyAlpha = 0.00f;
        float gameAlpha = 0.00f;
        float brickAlpha = 0.00f;
        float gameLogoY = gameLogo.transform.position.y;
        float pressStartAlpha = 0.00f;

        // Fade in Company Logo
        while (companyLogo.color.a < 1.0f) {
            companyLogo.color = new Color(companyLogo.color.r, companyLogo.color.g, companyLogo.color.b, companyAlpha);
            companyAlpha = companyAlpha + 0.09f;
            yield return null;
        }
        // Wait 2 seconds
        yield return new WaitForSeconds(2);
        // Fade out Company Logo
        while (companyLogo.color.a > 0.0f) {
            companyLogo.color = new Color(companyLogo.color.r, companyLogo.color.g, companyLogo.color.b, companyAlpha);
            companyAlpha = companyAlpha - 0.09f;
            yield return null;
        }
        // Wait 1 second
        yield return new WaitForSeconds(1);

        // Fade in Game Logo
        while (gameLogo.color.a < 1.0f) {
            gameLogo.color = new Color(gameLogo.color.r, gameLogo.color.g, gameLogo.color.b, gameAlpha);
            gameAlpha = gameAlpha + 0.09f;
            yield return null;
        }
        // Wait 2 seconds
        yield return new WaitForSeconds(1);

        while(gameLogo.transform.position.y < 2.5f) {
            gameLogoY = gameLogoY + 0.03f;
            gameLogo.transform.position = new Vector3(gameLogo.transform.position.x, gameLogoY, gameLogo.transform.position.z);
            yield return null;
        }

        while (brickAlpha < 1) {
            brickAlpha = brickAlpha + 0.09f;
            brickBk.color = new Color(brickBk.color.r, brickBk.color.g, brickBk.color.b, brickAlpha);
            yield return null;
        }
        
        readyToStart = true;
        
        while(pressStartAlpha < 1) {
            foreach (Text option in mainMenuOptions) {
                option.color = new Color(option.color.r, option.color.g, option.color.b, pressStartAlpha);
                pressStartAlpha += 0.1f;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
