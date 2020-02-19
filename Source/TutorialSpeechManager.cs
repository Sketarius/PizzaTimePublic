using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSpeechManager : MonoBehaviour
{
    public Accountant theAccountant;
    public Player thePlayer;

    public enum SpeechType { INTRO = 1, ROBBED = 2, PAID_CHEF = 3 };
    public GameObject tutorialCanvas;
    public AudioSource audioSource;
    public AudioClip dialogClick;

    public static bool setTutorialOn = true;
    private bool tutorialsReady = false;

    public bool introSpeechShown = false;
    public bool chefPaidExplained = false;
    public bool ovenFireExplained = false;
    public bool showRoomExplained = false;
    public bool goToSleepExplained = false;

    private string[] intro_text = { "Welcome to Pizza Time Arcade!", 
                                    "Our job is to keep this arcade running smoothly and efficiently.",
                                    "That includes making money!",
                                    "Hold on to that money, I can't afford to be working at McDoogles again!"
    };

    private string[] robbed_text = { "Oh no, we've been robbed!", 
                                     "Maybe we should hire a security guard.",
                                     "You can hire one by accessing the desk computer!" 
    };

    void Start() {
        audioSource.clip = dialogClick;
    }

    public void setTutorialsReady() {
        tutorialsReady = true;
    }

    public bool shouldTutorialsShow() {
        int dayNum = theAccountant.getDayNumber();

        if (dayNum > 1 && PlayerState.playerDataLoaded && (theAccountant.getGameTime().Hour > 8)) {
            return false;
        }
        return true;
    }

    public void displaySpeech(string[] speechText) {
        StartCoroutine(displayTutorialText(speechText));
    }

    public void displaySpeech(int speechType) {
        switch(speechType) {
            case (int) SpeechType.INTRO:
                StartCoroutine(displayTutorialText(intro_text));
                break;
            case (int)SpeechType.ROBBED:
                StartCoroutine(displayTutorialText(robbed_text));
                break;
        }
    }

    IEnumerator displayTutorialText(string[] speech_text) {
        // Wait until unpaused.
        while (thePlayer.playerIsPaused()) {
            yield return null;
        }

        tutorialCanvas.SetActive(true);
        Text tutorialText = tutorialCanvas.GetComponentInChildren<Text>();
        for (int i = 0; i < speech_text.Length; i++) {

            for (int j = 0; j < speech_text[i].Length; j++) {
                // Wait until unpaused.
                while (thePlayer.playerIsPaused()) {
                    yield return null;
                }
                tutorialText.text += speech_text[i][j];
                audioSource.Play();
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1f);
            //yield return StartCoroutine(waitForKeyDown(KeyCode.E));
            tutorialText.text = "";
        }
        tutorialCanvas.SetActive(false);

        yield return null;
    }

    IEnumerator waitForKeyDown(KeyCode keyCode) {
        while (!Input.GetKeyDown(keyCode)) {
            yield return null;
        }
    }

    // When loading a save, reload the tutorials already seen, so we don't see them anymore.
    public void loadTutorialSettings(string saveStr) {
        for (int i = 0; i < saveStr.Length; i++) {
            switch(i) {
                // Intro speech shown
                case 0:
                    introSpeechShown = (saveStr[i] == 'I') ? true : false;
                    break;
                // chef pay explained
                case 1:
                    chefPaidExplained = (saveStr[i] == 'C') ? true : false;
                    break;
                // Oven fire explained
                case 2:
                    ovenFireExplained = (saveStr[i] == 'O') ? true : false;
                    break;
                // showroom explained
                case 3:
                    showRoomExplained = (saveStr[i] == 'S') ? true : false;
                    break;
                // Go to sleep explained
                case 4:
                    goToSleepExplained = (saveStr[i] == 'G') ? true : false;
                    break;
            }
        }
    }

    // Used when saving a game.
    public string generateSaveString() {
        string saveStr = "";
        saveStr += (introSpeechShown) ? "I" : "X";
        saveStr += (chefPaidExplained) ? "C" : "X";
        saveStr += (ovenFireExplained) ? "O" : "X";
        saveStr += (showRoomExplained) ? "S" : "X";
        saveStr += (goToSleepExplained) ? "G" : "X";
        return saveStr;
    }

    void Update() {
        if (tutorialsReady) {
            if (!introSpeechShown) {
                displaySpeech((int)SpeechType.INTRO);
                introSpeechShown = true;
            }
            tutorialsReady = false;
        }
    }

}
