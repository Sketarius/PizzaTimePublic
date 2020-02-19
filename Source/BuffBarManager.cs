using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBarManager : MonoBehaviour
{
    public SpriteRenderer[] buffBarSlots;
    public Sprite emptyPizza;
    public Sprite emptySaladBar;
    public Sprite dirtyBathroom;
    public Sprite energyDrink;
    public Sprite trash;
    public Sprite advertising;

    public const int PIZZA_READY = 0;
    public const int EMPTY_SALADBAR = 1;
    public const int DIRTY_BATHROOM = 2;
    public const int ENERGY_DRINK = 3;
    public const int TRASH = 4;
    public const int ADVERTISING = 5;

    void Start() {
        
    }

    void Update() {
        
    }

    public void addToBuffBar(int buffBarItem) {
        // Add buff to earliest empty slot
        for (int i = 0; i < buffBarSlots.Length; i++) {
            if (buffBarSlots[i].sprite == null) {
                switch (buffBarItem) {
                    case PIZZA_READY:
                        buffBarSlots[i].sprite = emptyPizza;
                        break;
                    case EMPTY_SALADBAR:
                        buffBarSlots[i].sprite = emptySaladBar;
                        break;
                    case DIRTY_BATHROOM:
                        buffBarSlots[i].sprite = dirtyBathroom;
                        break;
                    case ENERGY_DRINK:
                        buffBarSlots[i].sprite = energyDrink;
                        break;
                    case TRASH:
                        buffBarSlots[i].sprite = trash;
                        break;
                    case ADVERTISING:
                        buffBarSlots[i].sprite = advertising;
                        break;
                }
                break;
            }
        }
        cleanUpIcons();
    }

    public void removeFromBuffBar(int buffBarItem) {
        for (int i = 0; i < buffBarSlots.Length; i++) {
            if ((buffBarSlots[i].sprite == emptyPizza) && (buffBarItem == PIZZA_READY)) {
                buffBarSlots[i].sprite = null;
            } else if ((buffBarSlots[i].sprite == emptySaladBar) && (buffBarItem == EMPTY_SALADBAR)) {
                buffBarSlots[i].sprite = null;
            } else if ((buffBarSlots[i].sprite == dirtyBathroom) && (buffBarItem == DIRTY_BATHROOM)) {
                buffBarSlots[i].sprite = null;
            } else if ((buffBarSlots[i].sprite == energyDrink) && (buffBarItem == ENERGY_DRINK)) {
                buffBarSlots[i].sprite = null;
            } else if ((buffBarSlots[i].sprite == advertising) && (buffBarItem == ADVERTISING)) {
                buffBarSlots[i].sprite = null;
            } else if ((buffBarSlots[i].sprite == trash) && (buffBarItem == TRASH)) {
                buffBarSlots[i].sprite = null;
            }
        }
        cleanUpIcons();
    }

    private void cleanUpIcons() {
        for (int i = 0; i < buffBarSlots.Length; i++) {
            // This slot is null and not the end element
            if ((buffBarSlots[i].sprite == null) && (i < (buffBarSlots.Length - 1))) {
                // If there is a buff icon to the right
                if (buffBarSlots[i+1].sprite != null) {
                    buffBarSlots[i].sprite = buffBarSlots[i + 1].sprite;
                    buffBarSlots[i + 1].sprite = null;
                }
            }
        }
    }
}
