using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerMenuManager : MonoBehaviour
{
    public Player thePlayer;
    public Accountant accountantReference;
    public SecurityManager securityManager;

    public float advertisePrice;
    // Security price now set in SecurityManager
    //public float securityPrice;
    public float hireCookPrice;

    /* Computer Options */
    private Text[] computerInteractionOptions;
    private int computerIntOptionSelected = 0;
    private bool computerOptionsDisplayed = false;

    private Text advertisementOption;
    private Text hireSecurityOption;
    private Text hireCookOption;

    private const int COMPUTER_ADVERTISE = 0;
    private const int COMPUTER_HIRE_SECURITY = 1;
    private const int COMPUTER_HIRE_COOK = 2;
    private const int COMPUTER_CANCEL_OPTION = 3;

    bool menuCanInteract = true;

    public void triggerComputerInteraction(GameObject computerObject) {
        float money = thePlayer.getPlayerMoney();
        computerIntOptionSelected = COMPUTER_ADVERTISE;

        for(int i = 0; i < computerInteractionOptions.Length; i++) {
            computerInteractionOptions[i].color = Color.black;
        }

        if ((money < advertisePrice) && (computerIntOptionSelected == COMPUTER_ADVERTISE)) {
            computerInteractionOptions[COMPUTER_ADVERTISE].color = Color.grey;
            computerIntOptionSelected = COMPUTER_HIRE_SECURITY;
        }

        if ((money < securityManager.securityPrice) && (computerIntOptionSelected == COMPUTER_HIRE_SECURITY)) {
            computerInteractionOptions[COMPUTER_HIRE_SECURITY].color = Color.grey;
            computerIntOptionSelected = COMPUTER_HIRE_COOK;
        }

        if ((money < hireCookPrice) && (computerIntOptionSelected == COMPUTER_HIRE_COOK)) {
            computerInteractionOptions[COMPUTER_HIRE_COOK].color = Color.grey;
            computerIntOptionSelected = COMPUTER_CANCEL_OPTION;
        }

        if(computerIntOptionSelected == COMPUTER_ADVERTISE) {
            computerInteractionOptions[COMPUTER_ADVERTISE].color = Color.yellow;
        }

        if(computerIntOptionSelected == COMPUTER_HIRE_SECURITY) {
            computerInteractionOptions[COMPUTER_HIRE_SECURITY].color = Color.yellow;
        }

        if (computerIntOptionSelected == COMPUTER_HIRE_COOK) {
            computerInteractionOptions[COMPUTER_HIRE_COOK].color = Color.yellow;
        }

        if (computerIntOptionSelected == COMPUTER_CANCEL_OPTION) {
            computerInteractionOptions[COMPUTER_CANCEL_OPTION].color = Color.yellow;
        }

        this.gameObject.SetActive(true);
        computerOptionsDisplayed = true;
        this.thePlayer.disablePlayerMovement();
    }

    private void handleInteractionMenu() {
        if (gameObject.activeSelf && computerOptionsDisplayed) {
            bool activatePressed = Input.GetButtonDown("Activate");
            if (activatePressed) {
                activatePressed = false;

                switch (this.computerIntOptionSelected) {
                    case COMPUTER_ADVERTISE:
                        accountantReference.advertiseArcade();
                        hideInteractionMenu();
                        break;
                    case COMPUTER_HIRE_SECURITY:
                        securityManager.purchaseSecurity();
                        hideInteractionMenu();
                        break;
                    case COMPUTER_HIRE_COOK:
                        break;
                    case COMPUTER_CANCEL_OPTION:
                        hideInteractionMenu();
                        break;
                }
            } else if (Input.GetKeyDown(KeyCode.W) || (Input.GetAxis("Vertical") > 0) && menuCanInteract) {
                menuCanInteract = false;
                StartCoroutine(MenuChange(this.computerIntOptionSelected - 1));
            } else if (Input.GetKeyDown(KeyCode.S) || (Input.GetAxis("Vertical") < 0) && menuCanInteract) {
                menuCanInteract = false;
                StartCoroutine(MenuChange(this.computerIntOptionSelected + 1));
            }
        }
    }

    IEnumerator MenuChange(int option) {
        selectOptionOnComputerMenu(option);
        yield return new WaitForSeconds(0.3f);
        menuCanInteract = true;
    }

    public void selectOptionOnComputerMenu(int option) {
        if(option < COMPUTER_ADVERTISE) {
            option = COMPUTER_CANCEL_OPTION;
        }

        if(option > COMPUTER_CANCEL_OPTION) {
            option = COMPUTER_ADVERTISE;
        }

        for(int i = 0; i < computerInteractionOptions.Length; i++) {
            if(option == i) {
                if(computerInteractionOptions[option].color == Color.gray) {
                    option++;
                    continue;
                } else {
                    computerInteractionOptions[option].color = Color.yellow;
                    this.computerIntOptionSelected = option;
                }
            } else {
                if (!(computerInteractionOptions[i].color == Color.gray)) {
                    computerInteractionOptions[i].color = Color.black;
                }
            }
        }
    }

    private void hideInteractionMenu() {
        thePlayer.enablePlayerMovement();
        this.computerOptionsDisplayed = false;
        this.gameObject.SetActive(false);
    }

    public bool interactionMenuDisplayed() {
        return this.computerOptionsDisplayed;
    }

    // Start is called before the first frame update
    void Start() {
        computerInteractionOptions = this.gameObject.transform.Find("Canvas").transform.Find("Panel").transform.Find("Options").GetComponentsInChildren<Text>();
        computerInteractionOptions[COMPUTER_ADVERTISE].text = "Advertise                  " + advertisePrice.ToString("C");
        computerInteractionOptions[COMPUTER_HIRE_SECURITY].text = "Hire Security             " + securityManager.securityPrice.ToString("C");
        computerInteractionOptions[COMPUTER_HIRE_COOK].text = "Hire Additional Cooks    " + hireCookPrice.ToString("C");
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        handleInteractionMenu();
    }
}
