using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzaManager : MonoBehaviour
{
    public Player thePlayer;
    public Accountant accountantReference;
    public NotificationCenter notificationCenterReference;
    public BuffBarManager buffBarReference;
    public TutorialSpeechManager tutSpeechManager;
    public TrashManagement trashManagement;
    public int percentChanceOfOvenBreak;

    public int xpForPizzaServe;
    public int xpForOvenFix;

    //private int numberOfCooks = 1;
    private int pizzaCookInterval = 10; // 10 seconds a pizza is cooked.
    private bool pizzaIsReady = false;
    private float employeeWage = 6.15f;
    private float pricePerPizza = 12.00f;
    private SpriteRenderer pizzaPanSpriteRenderer;
    private Sprite pizzaPanEmptySprite;
    private Sprite pizzaPanReadySprite;

    private bool pizzaGuyPayTutorialSpoke = false;

    public AudioSource audioSourceReference;
    public AudioSource audioSourceFireAlarm;
    public AudioClip pizzaFetchClip;
    public AudioClip pizzaReadyClip;
    public AudioClip ovenOnFireClip;
    public AudioClip fireAlarmClip;
    public AudioClip fireExtinguisherClip;

    private Text infoText;

    private bool pizzaOvenIsBroken = false;
    public SpriteRenderer pizzaOvenInfoRenderer;
    private Sprite brokenOvenIcon;
    private System.Random rnd;

    private string[] pizzaguy_oven_fire = { "The pizza oven is on fire! Put it out!" };

    // Start is called before the first frame update
    void Start() {
        this.rnd = new System.Random(Guid.NewGuid().GetHashCode());
        //this.pizzaPanSpriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        this.pizzaPanSpriteRenderer = GameObject.Find("/PizzaManager/PizzaPan").GetComponent<SpriteRenderer>();
        this.infoText = this.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();

        this.pizzaPanEmptySprite = Resources.Load<Sprite>("Sprites/pizza_pannish");
        this.pizzaPanReadySprite = Resources.Load<Sprite>("Sprites/pizza_pannish_ready");

        this.pizzaPanSpriteRenderer.sprite = this.pizzaPanEmptySprite;

        //this.pizzaOvenInfoRenderer = GameObject.Find("/PizzaManager/Oven/Fire").GetComponent<SpriteRenderer>();
        //this.brokenOvenIcon = Resources.Load<Sprite>("Sprites/wrench");

        this.pizzaOvenInfoRenderer.color = new Color(255, 255, 255, 0);

        StartCoroutine(pizzaWageTick());
        StartCoroutine(pizzaCookTick());

        this.infoText.text = "";
        this.infoText.color = Color.green;
    }

    public void pickUpPizza() {
        if (pizzaIsReady) {
            this.pizzaPanSpriteRenderer.sprite = this.pizzaPanEmptySprite;
            this.thePlayer.playerMoney = this.thePlayer.playerMoney + pricePerPizza;
            //Debug.Log("You served a pizza!");
            buffBarReference.removeFromBuffBar(BuffBarManager.PIZZA_READY);
            this.infoText.text = "+ " + pricePerPizza.ToString("C");
            StartCoroutine(fadeInMessage());
            this.audioSourceReference.clip = pizzaFetchClip;
            this.audioSourceReference.Play();
            pizzaIsReady = false;

            // The XP earned for serving pizza.
            thePlayer.addPlayerXP(xpForPizzaServe);
        }
    }

    public void fixOven() {
        if(pizzaOvenIsBroken) {
            pizzaOvenIsBroken = false;
            this.audioSourceReference.clip = fireExtinguisherClip;
            this.audioSourceReference.Play();
            this.pizzaOvenInfoRenderer.color = new Color(255,255,255, 0);
            thePlayer.addPlayerXP(xpForOvenFix);
            
        }
    }

    // The game time.
    IEnumerator pizzaCookTick() {
        int brokeOvenCoin;
        while (true) {
            if (!thePlayer.playerIsPaused()) {
                yield return new WaitForSeconds(this.pizzaCookInterval);
                if (!pizzaIsReady && accountantReference.arcadeIsOpen() && !pizzaOvenIsBroken) {
                    this.pizzaPanSpriteRenderer.sprite = this.pizzaPanReadySprite;
                    this.pizzaIsReady = true;
                    buffBarReference.addToBuffBar(BuffBarManager.PIZZA_READY);
                    this.audioSourceReference.clip = pizzaReadyClip;
                    this.audioSourceReference.Play();

                    brokeOvenCoin = rnd.Next(1, 100);

                    // 5% chance of broken oven this turn
                    // randomNum > (100 - percentChanceOfRobbery)
                    if (brokeOvenCoin > (100 - percentChanceOfOvenBreak)) {
                        pizzaOvenIsBroken = true;
                        this.pizzaOvenInfoRenderer.color = new Color(255, 255, 255, 255);
                        this.audioSourceReference.clip = ovenOnFireClip;
                        this.audioSourceReference.Play();
                        this.audioSourceFireAlarm.clip = fireAlarmClip;
                        this.audioSourceFireAlarm.Play();
                        if (!tutSpeechManager.ovenFireExplained) {
                            tutSpeechManager.displaySpeech(pizzaguy_oven_fire);
                            tutSpeechManager.ovenFireExplained = true;
                        }
                        Debug.Log("Oven rolled: " + brokeOvenCoin);
                    } else {
                        Debug.Log("Oven rolled: " + brokeOvenCoin);
                    }
                    
                    while (pizzaIsReady) {
                        yield return null;
                    }

                    while (pizzaOvenIsBroken) {
                        yield return null;
                    }
                }
            } else yield return null;
        }
    }

    IEnumerator pizzaWageTick() {
        while (true) {
            if (!thePlayer.playerIsPaused()) {
                yield return new WaitForSeconds(60 * 0.4f);

                if (accountantReference.arcadeIsOpen() && !thePlayer.playerIsPaused()) {
                    thePlayer.playerMoney = thePlayer.playerMoney - this.employeeWage;
                    
                    // Pizza guy hourly wage notification.
                    this.notificationCenterReference.printMessage("We paid " + this.employeeWage.ToString("C") + " to the pizza cook for his hourly wage.");
                    
                    // If user sets tutorial speech on and we haven't learned about paying pizza guy every hour.
                    if (!tutSpeechManager.chefPaidExplained) {
                        string[] pizzaguy_wage_tutorial = { "Every hour, the Pizza Chef(s) get paid an hourly wage of " + this.employeeWage.ToString("C") + ".",
                                                            "They have to eat too! ;)" };
                        tutSpeechManager.displaySpeech(pizzaguy_wage_tutorial);
                        tutSpeechManager.chefPaidExplained = true;
                    }
                }
            } else yield return null;
        }
    }

    private IEnumerator fadeInMessage() {
        float alpha = 0;
        this.infoText.color = new Color(0, 255, 0, alpha);
        while (this.infoText.color.a < 1.0f) {
            this.infoText.color = new Color(0, 255, 0, alpha);
            alpha = alpha + 0.09f;
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(1f);

        while (this.infoText.color.a > 0.0f) {
            this.infoText.color = new Color(0, 255, 0, alpha);
            alpha = alpha - 0.09f;
            yield return new WaitForSeconds(0.08f);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
