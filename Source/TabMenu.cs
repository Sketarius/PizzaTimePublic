using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    public Player thePlayer;

    public Text[] tabOptions;
    public GameObject[] tabMenus;
    private int tabOptionSelected = 0;
    private enum TabMenuOptions { PLAYER_MENU, HIRE_MENU, BUY_MENU };
    private bool tabMenuDisplaying = false;
    private bool menuItemSelectable = true;
    private Coroutine menuItemSelectCoRoutine = null;

    public void displayPlayerAttributes() {
        tabOptionSelected = 0;
        this.gameObject.SetActive(true);

        quickhighlightSelect(0);

        tabMenuDisplaying = true;
    }

    public void hidePlayerAttributes() {
        tabMenuDisplaying = false;


        // potential bugfix
        // setting inactive while a coroutine is 
        // still running and is expected to return to a non-existant object - not good.
        StopCoroutine(menuItemSelectCoRoutine);
        this.gameObject.SetActive(false);
        menuItemSelectable = true;
    }

    public bool playerAttributesDisplayed() {
        return this.tabMenuDisplaying;
    }

    private void quickhighlightSelect(int option) {
        for (int i = 0; i < tabOptions.Length; i++) {
            tabOptions[i].color = Color.white;
            tabMenus[i].SetActive(false);
        }
        tabMenus[option].SetActive(true);
        tabOptions[option].color = Color.yellow;
    }

    IEnumerator highlightSelectedOption(int option) {                
        for (int i = 0; i < tabOptions.Length; i++) {
            tabOptions[i].color = Color.white;
            tabMenus[i].SetActive(false);
        }
        tabMenus[option].SetActive(true);
        tabOptions[option].color = Color.yellow;
        
        yield return new WaitForSeconds(0.3f);
        this.menuItemSelectable = true;
    }

    // Start is called before the first frame update
    void Start() {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(tabMenuDisplaying) {
            // Selection to the right.
            if(Input.GetAxis("Horizontal") > 0 || Input.GetKeyUp(KeyCode.D)) {
                if (menuItemSelectable) {
                    if (tabOptionSelected == tabOptions.Length - 1) {
                        tabOptionSelected = 0;
                    } else {
                        tabOptionSelected++;
                    }
                    menuItemSelectable = false;
                    menuItemSelectCoRoutine = StartCoroutine(highlightSelectedOption(tabOptionSelected));
                }
                // Selection to the left.
            } else if (Input.GetAxis("Horizontal") < 0 || Input.GetKeyUp(KeyCode.A)) {
                if (menuItemSelectable) {
                    if (tabOptionSelected == 0) {
                        tabOptionSelected = tabOptions.Length - 1;
                    } else {
                        tabOptionSelected--;
                    }
                    menuItemSelectable = false;
                    menuItemSelectCoRoutine = StartCoroutine(highlightSelectedOption(tabOptionSelected));
                }
            }
        }
    }
}
