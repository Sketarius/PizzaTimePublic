using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManagement : MonoBehaviour
{
    public Accountant theAccountant;
    public Player thePlayer;
    public BuffBarManager buffBarManager;
    public EventMessageManager eventMessageManager;
    
    public SpriteRenderer playerTrashSpriteRenderer;
    public Sprite trashEmptySprite;
    public Sprite trashFullSprite;

    public Sprite trashBagSprite;

    public int xpForTrashDropOff = 300;
    public int trashInfestationCountdown = 10;
    public int percentChanceOfInfestation = 25;
    private bool arcadeInfested = false;

    private SpriteRenderer trashSpriteRenderer;

    private System.Random rnd;
    private bool trashIsFull = false;
    private bool playerHoldingTrash = false;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(startTrashTick());
        rnd = new System.Random(Guid.NewGuid().GetHashCode());
        trashSpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private bool rollForInfestation() {
        bool ret = false;
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());

        int randomNum = rnd.Next(100);
        if (randomNum > (100 - percentChanceOfInfestation)) {
            ret = true;
        }

        return ret;
    }

    IEnumerator startTrashTick() {
        while (true) {
            if(theAccountant.arcadeIsOpen()) {
                if (!trashIsFull) {
                    // 15-31 seconds
                    yield return new WaitForSeconds((int)rnd.Next(15, 30 + 1));
                    trashSpriteRenderer.sprite = trashFullSprite;
                    trashIsFull = true;
                    buffBarManager.addToBuffBar(BuffBarManager.TRASH);

                    
                    // Allow player time to pick up trash.
                    yield return new WaitForSeconds(trashInfestationCountdown);

                    // If time has passed then infest arcade.
                    if (trashIsFull && !playerHoldingTrash) {
                        // Roll for infestation
                        if (!arcadeInfested) {
                            //arcadeInfested = rollForInfestation();
                            arcadeInfested = true;

                            if (arcadeInfested) {
                                eventMessageManager.showInfestedMessage();
                            }
                        }
                    }
                }

            }
            yield return null;
        }
    }

    /*
     *  trashIsFull remains true until player drops off the trash at the dump 
     */
    public void pickUpTrash() {
        if (!playerHoldingTrash) {
            if (trashIsFull) {
                trashSpriteRenderer.sprite = trashEmptySprite;
                playerTrashSpriteRenderer.sprite = trashBagSprite;
                playerHoldingTrash = true;
            }
        }
    }

    public bool playerIsHoldingTrash() {
        return playerHoldingTrash;
    }

    public void dumpTrash() {
        if (playerHoldingTrash && trashIsFull) {
            trashIsFull = false;
            playerTrashSpriteRenderer.sprite = null;
            thePlayer.addPlayerXP(xpForTrashDropOff);
            playerHoldingTrash = false;
            buffBarManager.removeFromBuffBar(BuffBarManager.TRASH);
        }
    }

    public bool arcadeTrashIsFull() {
        return trashIsFull;
    }

    public bool arcadeIsInfested() {
        return arcadeInfested;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
