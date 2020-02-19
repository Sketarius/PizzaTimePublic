using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Accountant : MonoBehaviour
{
    private DateTime gameTime;
    private int dayNumber = 1;

    // Public references
    public GameObject userInterface;
    public NotificationCenter notificationCenterReference;
    public SaladBar saladBarReference;
    public PizzaManager pizzaManagerReference;
    public Bathroom bathroomReference;
    public BedManager bedReference;
    public BuffBarManager buffBarReference;
    public AudioManager audioManReference;
    public SecurityManager securityManager;
    public TutorialSpeechManager tutorialSpeechManager;
    public EventMessageManager eventMessageManager;
    public TrashManagement trashManagementReference;

    public int startHour;
    public int startMinute;

    public SpriteRenderer customerMoodIndicator;
    public Sprite customerMoodGood;
    public Sprite customerMoodNeutral;
    public Sprite customerMoodBad;

    private enum customerMoods {
        CUST_MOOD_GOOD = 0, 
        CUST_MOOD_NEUTRAL = 1, 
        CUST_MOOD_BAD = 3 
    };

    private customerMoods currentMood = 0;

    private Text[] timeText;

    private Text moneyAddSub;

    private Text storeStatus;
    private Text gameTimeText;
    private Text playerLevel;
    private Text playerXP;

    private Coroutine machineTickSub;
    private int machineTickInterval = 5;

    private ArcadeCabinet[] arcadeCabs;
    public Player thePlayer;

    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip arcadeOpenSound;
    private bool openSoundPlayed = false;
    private bool closedSoundPlayed = false;
    private System.Random rnd;

    private int numOfBrokenCabinets = 0;

    private bool arcadeIsAdvertising = false;

    private bool gameIsOver = false;
    private Canvas fadeOutCanvas;
    public bool open = false;


    // Start is called before the first frame update
    void Start() {
        // Game starts January 1 [Current Year] 9:00am
        this.gameTime = new DateTime(DateTime.Now.Year, 1, 1, startHour, startMinute, 0);
        this.gameTimeText = userInterface.GetComponentsInChildren<Text>()[1];
        this.storeStatus = userInterface.GetComponentsInChildren<Text>()[2];
        this.playerLevel = userInterface.GetComponentsInChildren<Text>()[3];
        this.playerXP = userInterface.GetComponentsInChildren<Text>()[4];

        this.getAllArcadeCabinets();

        this.audioSource.clip = this.coinSound;
        this.rnd = new System.Random(Guid.NewGuid().GetHashCode());

        this.customerMoodIndicator = GameObject.Find("/Main Camera/User Interface/Canvas/Customer Mood").GetComponent<SpriteRenderer>();

        this.customerMoodIndicator.sprite = customerMoodGood;

        this.moneyAddSub = GameObject.Find("/Main Camera/User Interface/Canvas/MoneyAddSub").GetComponent<Text>();

        // Game Over Canvas Object
        this.fadeOutCanvas = GameObject.Find("/Main Camera/FadeCanvas").GetComponent<Canvas>();
        this.fadeOutCanvas.enabled = false;

        StartCoroutine(timeTick());
        machineTickSub = StartCoroutine(machineTick());
        StartCoroutine(customerMoodTick());

        // If there has been data loaded from a save && 
        // the amount of loaded cabinets are equal to the existing cabinets.
        if (PlayerState.playerDataLoaded && (PlayerState.cabinets.Count == arcadeCabs.Length)) {
            thePlayer.playerMoney = PlayerState.playerMoney;
            thePlayer.setPlayerXP(PlayerState.playerXP);
            thePlayer.setPlayerLevel(PlayerState.playerLevel);
            gameTime = PlayerState.gameTime;
            this.dayNumber = PlayerState.dayNumber;
            audioManReference.playLevel(this.dayNumber);
            // Load saladbar values.
            if (PlayerState.saladBarIsFull) {
                saladBarReference.setSaladBarFull();
            } else saladBarReference.setSaladBarEmpty();

            // If bathroom is dirty on save load
            if (!PlayerState.bathroomIsClean) {
                bathroomReference.setBathroomDIrty();
            }

            tutorialSpeechManager.loadTutorialSettings(PlayerState.tutorialsExplained);

            // Match up UIDs for arcade cabinets and set their properties.
            for (int i = 0; i < PlayerState.cabinets.Count; i++) {
                for (int j = 0; j < arcadeCabs.Length; j++) {
                    if (PlayerState.cabinets[i].UID == arcadeCabs[j].UID) {
                        arcadeCabs[j].cabinetLevel = PlayerState.cabinets[i].cabinetLevel;
                        arcadeCabs[j].GameTitle = PlayerState.cabinets[i].GameTitle;
                        if (PlayerState.cabinets[i].machineBroken) {
                            arcadeCabs[j].breakMachine();
                            this.numOfBrokenCabinets = numOfBrokenCabinets + 1;
                        }
                        arcadeCabs[j].priceToPlay = PlayerState.cabinets[i].priceToPlay;
                        arcadeCabs[j].priceToRepair = PlayerState.cabinets[i].priceToRepair;
                    }
                }
            }

        }
        tutorialSpeechManager.setTutorialsReady();
    }

    private void getAllArcadeCabinets() {
        GameObject go = GameObject.Find("/Arcade/Arcade Machines");
        this.arcadeCabs = (ArcadeCabinet[])go.GetComponentsInChildren<ArcadeCabinet>();
    }

    // Update is called once per frame
    void Update() {
        this.gameTimeText.text = this.gameTime.ToString("ddd h:mm tt");
        // Arcade is Open
        if (this.arcadeIsOpen()) {
            this.storeStatus.text = "";
            this.userInterface.GetComponent<UserInterface>().displayOpenSign();
            this.storeStatus.color = Color.green;
            // Play open sound.
            if (!this.openSoundPlayed) {
                this.audioSource.clip = this.arcadeOpenSound;
                this.audioSource.Play();
                this.openSoundPlayed = true;
                this.notificationCenterReference.printMessage("Day " + dayNumber + ":\nThe arcade is now open.");
                eventMessageManager.showStartMessage();
            }

            if (this.closedSoundPlayed) {
                this.closedSoundPlayed = false;
            }
        // Arcade is Closed.
        } else {
            this.storeStatus.text = "";
            this.storeStatus.color = Color.red;
            this.userInterface.GetComponent<UserInterface>().displayCloseSign();

            if (!this.closedSoundPlayed) {
                closedSoundPlayed = true;                
            }

            if (this.openSoundPlayed) {
                this.openSoundPlayed = false;
            }

            if (!bedReference.bedIsAvailable() && bedReference.ableToUseBed()) {
                string[] sleepTutorial = { "The arcade is closed, and it's now time to go home and sleep!" };
                this.notificationCenterReference.printMessage("The arcade is now closed.\nYou may now head to bed!");
                tutorialSpeechManager.displaySpeech(sleepTutorial);
                bedReference.setBedAvailable();
            }
        }

        this.playerLevel.text = "Level " + this.thePlayer.getPlayerLevel();
        this.playerXP.text = this.thePlayer.getPlayerXP() + "/" + this.thePlayer.getXPForNextLevel(this.thePlayer.getPlayerLevel());

        // Game Over check.
        if (thePlayer.getPlayerMoney() < 0.00f && !thePlayer.gameIsOver()) {
            thePlayer.setGameOver();
            displayGameOver();
        }
    }

    public void displayGameOver() {
        StopAllCoroutines();
        thePlayer.disablePlayerMovement();
        StartCoroutine(fadeToGameOver());
        thePlayer.disableAnimator();
    }

    public IEnumerator fadeToGameOver() {
        float alpha = 0.00f;
        this.fadeOutCanvas.enabled = true;
        Image blackPanel = this.fadeOutCanvas.GetComponentInChildren<Image>();
        Text gameOverMessage = fadeOutCanvas.GetComponentInChildren<Text>();
        gameOverMessage.text = "BANKRUPT";

        while (blackPanel.color.a < 0.5f) {
            blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, alpha);
            alpha = alpha + 0.09f;
            yield return null;
        }    
    }

    public void advertiseArcade() {
        this.userInterface.GetComponent<UserInterface>().updateFunds(this.thePlayer.getPlayerMoney(), this.thePlayer.getPlayerMoney() - 100.00f);
        this.thePlayer.playerMoney = thePlayer.playerMoney - 100.00f;
        this.arcadeIsAdvertising = true;
        buffBarReference.addToBuffBar(BuffBarManager.ADVERTISING);
        StopCoroutine(this.machineTickSub);
        machineTickSub = StartCoroutine(machineTick());
    }

    IEnumerator customerMoodTick() {
        int saladBarPoints = 12;
        int bathroomPoints = 12;
        int trashPoints = 24;
        int totalArcadeCabs = arcadeCabs.Length;
        int totalPoints = totalArcadeCabs + saladBarPoints + bathroomPoints + trashPoints;
        Debug.Log("Total Points: " + totalPoints);
        while(true) {
            if (!thePlayer.playerIsPaused()) {
                int moodPoints = 0;
                float moodResult = 0f;

                // Mood Points @ Max!
                moodPoints = totalPoints;

                // If salad bar is not full, we lose points
                if (!this.saladBarReference.saladBarIsFull()) {
                    moodPoints = moodPoints - saladBarPoints;
                }

                // If the bathroom is not clean, we lose points
                if (!this.bathroomReference.isBathroomClean()) {
                    moodPoints = moodPoints - bathroomPoints;
                }

                // Trash full? Lose 'dem points!
                if (this.trashManagementReference.arcadeTrashIsFull()) {
                    moodPoints = moodPoints - trashPoints;
                }

                // For every broken arcade cabinet we lose points.
                moodPoints = moodPoints - numOfBrokenCabinets;

                // Calculate the percentage of customer mood.
                moodResult = (float) moodPoints / totalPoints;


                //Good mood
                if (moodResult > 0.75f) {
                    this.customerMoodIndicator.sprite = customerMoodGood;
                    this.currentMood = customerMoods.CUST_MOOD_GOOD;
                } else if (moodResult < 0.75f && moodResult > 0.50f) {
                    this.customerMoodIndicator.sprite = customerMoodNeutral;
                    this.currentMood = customerMoods.CUST_MOOD_NEUTRAL;
                } else if (moodResult < 0.50f) {
                    this.customerMoodIndicator.sprite = customerMoodBad;
                    this.currentMood = customerMoods.CUST_MOOD_BAD;
                }

                //yield return new WaitForSeconds(1);
                yield return null;
            } else yield return null;
        }
    }

    // Ticking for arcade cabinets.
    IEnumerator machineTick() {
        int originalNonAdvertisementInterval = machineTickInterval;
        int advertisementTick = 0;
        while (true) {
            if (!thePlayer.playerIsPaused()) {
                float totalMade = 0f;
                int totalXPEarned = 0;

                if (this.arcadeIsAdvertising && advertisementTick == 0) {
                    this.machineTickInterval = (int)this.machineTickInterval / 2;                    
                }

                // Only accumulate when store is Open
                if (this.arcadeIsOpen()) {
                    // Broken random chance.
                    int randomNum;
                    numOfBrokenCabinets = 0;
                    for (int i = 0; i < this.arcadeCabs.Length; i++) {

                        if (!arcadeCabs[i].machineBroken) {
                            totalMade += this.arcadeCabs[i].priceToPlay;

                            // Neutral penalizes half of arcadeCost
                            if (currentMood == customerMoods.CUST_MOOD_NEUTRAL) {
                                totalMade -= (float)(this.arcadeCabs[i].priceToPlay / 2);
                                // Bad penalizes (playerLevel * 2) * arcadeCost
                            } else if (currentMood == customerMoods.CUST_MOOD_BAD) {
                                totalMade -= (float)(this.thePlayer.getPlayerLevel() * (this.arcadeCabs[i].priceToPlay / 2));
                            }

                            totalXPEarned += this.arcadeCabs[i].xpEarnedPerInterval;
                            randomNum = rnd.Next(100);
                            if (randomNum < 2) {
                                try {
                                    arcadeCabs[i].breakMachine();
                                    this.numOfBrokenCabinets = numOfBrokenCabinets + 1;
                                } catch (NullReferenceException nre) { }
                            }

                            arcadeCabs[i].printInfoMessage("+ " + this.arcadeCabs[i].priceToPlay.ToString("C"));
                        } else {
                            this.numOfBrokenCabinets = numOfBrokenCabinets + 1;
                        }
                    }
                    this.printMoneyMade(totalMade.ToString("C"), (totalMade > 0.00f));
                    this.userInterface.GetComponent<UserInterface>().updateFunds(this.thePlayer.getPlayerMoney(), this.thePlayer.getPlayerMoney() + totalMade);
                    this.thePlayer.setPlayerMoney(this.thePlayer.getPlayerMoney() + totalMade);
                    this.thePlayer.setPlayerXP(this.thePlayer.getPlayerXP() + totalXPEarned);
                    this.audioSource.clip = this.coinSound;
                    this.audioSource.Play();
                }
                yield return new WaitForSeconds(machineTickInterval);

                // If we are advertising this keeps tracks of max intervals before we go back to the normal interval.
                if (arcadeIsAdvertising) {
                    advertisementTick++;

                    if(advertisementTick > 10) {
                        arcadeIsAdvertising = false;
                        buffBarReference.removeFromBuffBar(BuffBarManager.ADVERTISING);
                        machineTickInterval = originalNonAdvertisementInterval;
                    }
                }
            } else yield return null;
        }
    }

    private void printMoneyMade(string msg, bool gain) {
        this.moneyAddSub.text = ((gain)?"+ ":"- ") + msg;

        StartCoroutine(moneyMadeIndicator(gain));
    }

    private IEnumerator moneyMadeIndicator(bool gain) {
        float alpha = 0;
        this.moneyAddSub.color = new Color((gain)?0:255, (gain)?255:0, 0, alpha);
        while (this.moneyAddSub.color.a < 1.0f) {
            this.moneyAddSub.color = new Color((gain)?0:255, (gain)?255:0, 0, alpha);
            alpha = alpha + 0.09f;
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(1f);

        while (this.moneyAddSub.color.a > 0.0f) {
            this.moneyAddSub.color = new Color((gain)?0:255, (gain)?255:0, 0, alpha);
            alpha = alpha - 0.09f;
            yield return new WaitForSeconds(0.08f);
        }
    }

    // The game time.
    IEnumerator timeTick() {
        while (true) {
            if (!thePlayer.playerIsPaused()) {
                this.gameTime = gameTime.AddMinutes(1);
                yield return new WaitForSeconds(0.4f);
            } else yield return null;
        }
    }

    public bool arcadeIsOpen() {
        bool ret = false;
        TimeSpan open = new TimeSpan(9, 0, 0);
        TimeSpan closed = new TimeSpan(22, 0, 0);
        TimeSpan now = this.gameTime.TimeOfDay;

        if ((now > open) && (now < closed)) {
            ret = true;
        }

        return ret;
    }

    public DateTime getGameTime() {
        return this.gameTime;
    }

    public void setGameTime(DateTime newTime) {
        this.gameTime = newTime;
    }

    public SaladBar getSaladBar() {
        return this.saladBarReference;
    }

    public void setDayNumber(int dayNum) {
        this.dayNumber = dayNum;
    }

    public Canvas getFadeOutCanvas() {
        return this.fadeOutCanvas;
    }

    public int getDayNumber() {
        return dayNumber;
    }

    public ArcadeCabinet[] getArcadeCabinets() {
        return this.arcadeCabs;
    }

    public void goToNextDay() {
        this.dayNumber = dayNumber + 1;
    }
}
