using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BedManager : MonoBehaviour
{
    public Player thePlayer;
    public Accountant accountantReference;
    public AudioManager audioManReference;
    public NotificationCenter notificationCenter;
    public SecurityManager securityManager;
    public TutorialSpeechManager tutorialSpeechManager;

    public int percentChanceOfRobbery;
    public int percentChanceofDamagedCabinets;

    private SpriteRenderer arrowRend;
    public Sprite bedArrowSprite;
    private Canvas fadeOutCanvas;
    public int floatingArrowSpeed = 1;
    private float yArrowStartingPosition;

    public bool bedAlreadyUsed = true;

    System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());

    System.Random rndf = new System.Random(Guid.NewGuid().GetHashCode());

    public void goToSleep() {
        DateTime now = accountantReference.getGameTime();
        float moneyLost;

        if (!accountantReference.arcadeIsOpen() && !bedAlreadyUsed) {
            int currentTimeVal;
           
            if (now.Hour > 21 && now.Hour < 24) {
                // Add a day and hours.
                currentTimeVal = 8 + ((now.Hour == 22) ? 2 : 1);
            } else {
                // Add hours..
                currentTimeVal = 8 - now.Hour;
            }
            StartCoroutine(fadeToNextDay());
            accountantReference.setGameTime(now.AddHours(currentTimeVal));

            // If no security hired, then there's a chance of being robbed.
            if (!securityManager.getSecurityHired()) {
                moneyLost = chanceOfRobbery();
                thePlayer.setPlayerMoney(thePlayer.getPlayerMoney() - moneyLost);

                if (moneyLost > 0) {
                    notificationCenter.printMessage("Your arcade was robbed overnight.\nYou lost " + moneyLost.ToString("C") + "\nHire security to prevent this!");
                    tutorialSpeechManager.displaySpeech((int) TutorialSpeechManager.SpeechType.ROBBED);
                }
            }
        }

        bedAlreadyUsed = true;
    }

    public bool ableToUseBed() {
        bool ret = false;
        TimeSpan start = new TimeSpan(22, 0, 0);
        TimeSpan end = new TimeSpan(8, 0, 0);
        TimeSpan now = accountantReference.getGameTime().TimeOfDay;

        if ((now > start) || (now < end)) {
            ret = true;
        }

        return ret;
    }

    public bool bedIsAvailable() {
        return !bedAlreadyUsed;
    }

    public void setBedAvailable() {
        bedAlreadyUsed = false;
    }

    private float RandomFloatBetween(float minValue, float maxValue) {
        var next = (float) rndf.NextDouble();

        return minValue + (next * (maxValue - minValue));
    }

    private float chanceOfRobbery() {
        
        int randomNum = rnd.Next(100);
        float moneyLost = 0.00f;

        Debug.Log("Robbery: " + randomNum);
        // 5% chance of robbery.
        if (randomNum > (100 - percentChanceOfRobbery)) {
            ArcadeCabinet[] cabs = accountantReference.getArcadeCabinets();
            for(int i = 0; i < cabs.Length; i++) {
                randomNum = rnd.Next(100);
                // 25% chance the vandals hurt an arcade cabinet
                if (randomNum > (100 - percentChanceofDamagedCabinets)) {
                    cabs[i].breakMachine();
                }
            }

            // 1-90 % of money can be lost
            moneyLost = thePlayer.getPlayerMoney() * RandomFloatBetween(0.01f, 0.9f);
            return moneyLost;
        } else {
            return 0.00f;
        }
    }

    IEnumerator fadeToNextDay() {
        float alpha = 0.00f;
        this.fadeOutCanvas.enabled = true;
        UnityEngine.UI.Image blackPanel = this.fadeOutCanvas.GetComponentInChildren<UnityEngine.UI.Image>();
        Text gameOverMessage = fadeOutCanvas.GetComponentInChildren<Text>();
        
        audioManReference.playerSleep();
        accountantReference.goToNextDay();
        gameOverMessage.text = "Day " + accountantReference.getDayNumber();

        while (blackPanel.color.a < 1.0f) {
            blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, alpha);
            alpha = alpha + 0.09f;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while (blackPanel.color.a > 0.0f) {
            blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, alpha);
            gameOverMessage.color = new Color(gameOverMessage.color.r, gameOverMessage.color.g, gameOverMessage.color.b, alpha);
            alpha = alpha - 0.09f;
            yield return null;
        }
        gameOverMessage.text = "";
        audioManReference.playLevel(accountantReference.getDayNumber());

        arrowRend.sprite = null;
    }

    IEnumerator arrowHover() {
        Vector2 destinationHigh = new Vector2(arrowRend.transform.position.x, arrowRend.transform.position.y + 0.5f);
        Vector2 destinationLow = new Vector2(arrowRend.transform.position.x, arrowRend.transform.position.y - 0.5f);

        while (true) {
            while (arrowRend.transform.position.y != destinationHigh.y) {
                arrowRend.transform.position = Vector2.MoveTowards(arrowRend.transform.position, destinationHigh, floatingArrowSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            while (arrowRend.transform.position.y != destinationLow.y) {
                arrowRend.transform.position = Vector2.MoveTowards(arrowRend.transform.position, destinationLow, floatingArrowSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        fadeOutCanvas = accountantReference.getFadeOutCanvas();
        arrowRend = transform.Find("Arrow").GetComponentInChildren<SpriteRenderer>();
        yArrowStartingPosition = arrowRend.transform.position.y;
        StartCoroutine(arrowHover());
    }

    // Update is called once per frame
    void Update() {
        if (!accountantReference.arcadeIsOpen() && ableToUseBed()) {
            arrowRend.sprite = bedArrowSprite;
        } else arrowRend.sprite = null;
    }
}
