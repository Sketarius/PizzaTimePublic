using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CabinetMenuManager : MonoBehaviour
{
    // Reference Menu UI
    private GameObject menuUIReference;
    public Player playerReference;
    public TrashManagement trashManagementReference;


    /* Arcade Cabinet Options */
    private bool cabinetOptionsDisplayed = false;
    private Text[] cabinetInteractionOptions;
    private int cabinetIntOptionSelected = 2;

    private const int CABINET_GAME_TITLE = 0;
    private const int CABINET_INFO_TEXT = 1;
    private const int UPGRADE_REPAIR_CABINET_OPTION = 2;
    private const int CABINET_CANCEL_OPTION = 3;
    private ArcadeCabinet arcadeCabinetSelected = null;

    /* Sound section */

    public AudioSource menuUIAudioSource;
    public AudioClip menuUIShowClip;
    public AudioClip menuUIScrollClip;
    public AudioClip menuUISelectClip;
    private ArcadeCabinet selectedCabinet;

    private bool menuCanInteract = true;


    // Start is called before the first frame update
    void Start() {
        // Let's get the gameobject we're on top of!
        this.menuUIReference = this.gameObject;
        // Set the audio source to play the scroll sound at first.
        this.menuUIAudioSource.clip = this.menuUIScrollClip;
        // Hide us!
        this.menuUIReference.SetActive(false);

        // Hide Mouse Cursor
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        if (!this.menuUIReference.activeSelf) {
            // Change to open menu open sound & play
            this.menuUIAudioSource.clip = this.menuUIShowClip;
            this.menuUIAudioSource.Play();
            
        }

        if (this.cabinetOptionsDisplayed) {
            this.handleInteractionMenu();
        } //else Time.timeScale = 1;

    }

    public void triggerCabinetInteraction(GameObject arcadeObject) {

        ArcadeCabinet ac = arcadeObject.GetComponentInParent<ArcadeCabinet>();

        cabinetInteractionOptions = this.menuUIReference.GetComponentInChildren<Canvas>().GetComponentsInChildren<Text>();
        cabinetInteractionOptions[CABINET_GAME_TITLE].text = ac.GameTitle;
        cabinetInteractionOptions[CABINET_INFO_TEXT].text = "Lvl   " + ac.cabinetLevel + "    Profit: " + ac.priceToPlay.ToString("C");
        if (!ac.machineBroken) {
            cabinetInteractionOptions[UPGRADE_REPAIR_CABINET_OPTION].text = "Upgrade Cabinet";
        } else {
            cabinetInteractionOptions[UPGRADE_REPAIR_CABINET_OPTION].text = "Repair Cabinet";
        }
        cabinetInteractionOptions[UPGRADE_REPAIR_CABINET_OPTION].color = Color.white;

        cabinetInteractionOptions[CABINET_CANCEL_OPTION].text = "Cancel";
        cabinetInteractionOptions[CABINET_CANCEL_OPTION].color = Color.white;

        this.cabinetIntOptionSelected = UPGRADE_REPAIR_CABINET_OPTION;

        
        cabinetInteractionOptions[UPGRADE_REPAIR_CABINET_OPTION].color = (cabinetIntOptionSelected == UPGRADE_REPAIR_CABINET_OPTION) ? Color.yellow : Color.white;
        
        if(playerReference.playerMoney < ac.getPriceToUpgrade()) {
            cabinetInteractionOptions[UPGRADE_REPAIR_CABINET_OPTION].color = Color.gray;
            if(!ac.machineBroken) cabinetIntOptionSelected++;
        } else {
            cabinetInteractionOptions[UPGRADE_REPAIR_CABINET_OPTION].color = (cabinetIntOptionSelected == UPGRADE_REPAIR_CABINET_OPTION) ? Color.yellow : Color.white;
        }

        cabinetInteractionOptions[cabinetIntOptionSelected].color = Color.yellow;

        cabinetOptionsDisplayed = true;
        arcadeCabinetSelected = ac;

        this.gameObject.SetActive(true);
        this.playerReference.disablePlayerMovement();
    }

    IEnumerator MenuChange(int option) {
        selectOptionOnCabinetMenu(option);
        yield return new WaitForSeconds(0.3f);
        menuCanInteract = true;
    }

    private void handleInteractionMenu() {
        if (gameObject.activeSelf && cabinetOptionsDisplayed) {
            //Time.timeScale = 0;
            bool activatePressed = Input.GetButtonDown("Activate");
            if (activatePressed) {
                activatePressed = false;

                switch (this.cabinetIntOptionSelected) {
                    case UPGRADE_REPAIR_CABINET_OPTION:
                        if (arcadeCabinetSelected.machineBroken) {
                            hideInteractionMenu(menuUIShowClip);
                            this.arcadeCabinetSelected.fixMachine();
                            menuCanInteract = true;
                        } else {
                            this.arcadeCabinetSelected.upgradeCabinet();
                            hideInteractionMenu(menuUIShowClip);
                            menuCanInteract = true;
                        }
                        break;
                    case CABINET_CANCEL_OPTION:
                        hideInteractionMenu(menuUISelectClip);
                        menuCanInteract = true;
                        break;
                }
            } else if (Input.GetKeyDown(KeyCode.W) || (Input.GetAxis("Vertical") > 0) && menuCanInteract) {
                menuCanInteract = false;
                StartCoroutine(MenuChange(this.cabinetIntOptionSelected - 1));
            } else if (Input.GetKeyDown(KeyCode.S) || (Input.GetAxis("Vertical") < 0) && menuCanInteract) {
                menuCanInteract = false;
                StartCoroutine(MenuChange(this.cabinetIntOptionSelected + 1));
            } else if (Input.GetButtonDown("Cancel")) {
                hideInteractionMenu(menuUISelectClip);
            }
        }
    }

    private void selectOptionOnCabinetMenu(int selectedOption) {

        if (selectedOption < UPGRADE_REPAIR_CABINET_OPTION) {
            selectedOption = CABINET_CANCEL_OPTION;
        }

        // Reset to first option.
        if (selectedOption > CABINET_CANCEL_OPTION) {
            selectedOption = UPGRADE_REPAIR_CABINET_OPTION;
        }

        
        for(int i = UPGRADE_REPAIR_CABINET_OPTION; i < cabinetInteractionOptions.Length; i++) {
            if (selectedOption == i) {
                if(cabinetInteractionOptions[selectedOption].color == Color.gray) {
                    selectedOption++;
                    continue;
                } else {
                    cabinetInteractionOptions[selectedOption].color = Color.yellow;
                    this.cabinetIntOptionSelected = selectedOption;
                }
            } else {
                if(!(cabinetInteractionOptions[i].color == Color.gray)) {
                    cabinetInteractionOptions[i].color = Color.white;
                }
            }
        }

        
    }

    private void hideInteractionMenu(AudioClip soundToPlay) {
        playerReference.enablePlayerMovement();
        this.menuUIAudioSource.clip = soundToPlay;
        this.menuUIAudioSource.Play();
        this.cabinetInteractionOptions[cabinetIntOptionSelected].color = Color.white;
        this.cabinetOptionsDisplayed = false;
        this.menuUIReference.SetActive(false);
        this.playerReference.unpause();
    }

    public bool interactionMenuDisplayed() {
        return this.cabinetOptionsDisplayed;
    }
}
