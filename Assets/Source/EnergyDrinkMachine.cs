using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyDrinkMachine : MonoBehaviour
{
    public Player thePlayer;
    public float priceToBuy = 50.00f;
    private int secondsOfBoost = 10;

    public BuffBarManager buffBarReference;

    public AudioSource edmAudioSource;
    public AudioClip edmDrinkClip;
    public AudioClip edmEndClip;

    private BuffNerfEffect buffNerfEffectRef;
    private Text costInfo;

    private bool playerAlreadyCaffeinated = false;

    public void buyEnergyDrink() {
        if ((thePlayer.getPlayerMoney() >= priceToBuy) && !playerAlreadyCaffeinated) {
            playerAlreadyCaffeinated = true;
            buffBarReference.addToBuffBar(BuffBarManager.ENERGY_DRINK);
            StartCoroutine(drinkEnergyDrink(false));
        }
    }

    public void giveEnergyDrink() {
        if (!playerAlreadyCaffeinated) {
            playerAlreadyCaffeinated = true;
            buffBarReference.addToBuffBar(BuffBarManager.ENERGY_DRINK);
            StartCoroutine(drinkEnergyDrink(true));
        }
    }

    IEnumerator drinkEnergyDrink(bool drinkIsFree) {        
        // Get normal player speed.
        float normalMoveSpeed = thePlayer.getPlayerMovementSpeed();
        // Set player speed * 0.5
        thePlayer.setPlayerMovementSpeed(normalMoveSpeed * 1.3f);
        
        // If drink is not free. (Not a Power up)
        if (!drinkIsFree) {
            thePlayer.setPlayerMoney(thePlayer.getPlayerMoney() - priceToBuy);
        }

        edmAudioSource.clip = edmDrinkClip;
        edmAudioSource.Play();

        this.buffNerfEffectRef.playDrinkEnergy();

        // Let the effect take secondsOfBoost time
        yield return new WaitForSeconds(secondsOfBoost);
        // Return to normal speed.
        thePlayer.setPlayerMovementSpeed(normalMoveSpeed);
        playerAlreadyCaffeinated = false;
        buffBarReference.removeFromBuffBar(BuffBarManager.ENERGY_DRINK);
        edmAudioSource.clip = edmEndClip;
        edmAudioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.buffNerfEffectRef = thePlayer.GetComponentInChildren<BuffNerfEffect>();
        this.costInfo = this.transform.Find("CostCanvas").GetComponentInChildren<Text>();

        this.costInfo.text = this.priceToBuy.ToString("C");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
