using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeCabinet : MonoBehaviour
{
    public int UID = -1;
    public Player playerReference;
    public NotificationCenter notificationCenterReference;
    public UserInterface userInterfaceReference;
    public TrashManagement trashManagementReference;
    public string GameTitle;
    public int cabinetLevel = 1;
    public float priceToPlay = 0.25f;
    public int xpEarnedPerInterval = 10;
    public int xpEarnedFixedMultiplier = 100;
    public float priceToRepair = 6.00f;
    public bool machineBroken = false;


    private SpriteRenderer infoSpriteRend;
    //private Sprite brokenSprite;
    public Sprite brokenSprite;
    private SpriteRenderer selectSpriteRend;
    private Sprite selectSprite;
    private Text infoText;
    private Text levelText;

    public float getPriceToUpgrade() {
        float ret = 100;
        int i = 1;

        do {
            ret *= cabinetLevel;
            i++;
        } while (i < cabinetLevel);

        return ret;
    }

    public float getPriceToRepair() {
        return priceToRepair * cabinetLevel;
    }

    public void upgradeCabinet() {
        deductUpgradeMoney();

        this.cabinetLevel++;
        switch(cabinetLevel) {
            case 1:
                this.priceToPlay = 0.25f;
                break;
            case 2:
                this.priceToPlay = 0.50f;
                levelText.text = "2";
                break;
            case 3:
                this.priceToPlay = 0.75f;
                levelText.text = "3";
                break;
            case 4:
                this.priceToPlay = 1.00f;
                levelText.text = "4";
                break;
            default:
                break;
        }
        this.xpEarnedPerInterval *= this.cabinetLevel;
        this.printInfoMessage("");
    }

    private void deductUpgradeMoney() {
        this.playerReference.playerMoney = this.playerReference.playerMoney - this.getPriceToUpgrade();
    }

    public bool isMachineIsBroken(int seed) {
        System.Random rnd = new System.Random(seed);
        int machineBroke = rnd.Next(100);
        //Debug.Log("Break roll: " + machineBroke);

        if (machineBroke < 80 && machineBroke > 70) {
            this.machineBroken = true;
            this.infoSpriteRend.sprite = this.brokenSprite;
            return true;
        }
        return false;
    }

    public void breakMachine() {
        this.machineBroken = true;
        this.infoSpriteRend.sprite = this.brokenSprite;
    }

    public void fixMachine() {
        this.machineBroken = false;
        this.infoSpriteRend.sprite = null;

        this.playerReference.playerMoney = this.playerReference.playerMoney - getPriceToRepair();
        this.printInfoMessage("-" + getPriceToRepair().ToString("C"));

        playerReference.addPlayerXP(this.cabinetLevel * (xpEarnedFixedMultiplier));
    }

    public void showSelectIcon() {
        this.selectSpriteRend.sprite = selectSprite;
    }

    public void hideSelectIcon() {
        this.selectSpriteRend.sprite = null;
    }

    public void printInfoMessage(string msg) {
        try {
            this.infoText.text = msg;
            StartCoroutine(fadeInMessage());
        } catch (System.NullReferenceException e) {}
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

    private void loadInitialLevel() {
        levelText.text = cabinetLevel.ToString();
    }

    void Awake() {
        this.brokenSprite = Resources.Load<Sprite>("Sprites/wrench");
        this.selectSprite = Resources.Load<Sprite>("Sprites/arcade_select_icon");
        this.infoSpriteRend = this.transform.Find("Info").GetComponentInChildren<SpriteRenderer>();
        this.selectSpriteRend = this.transform.Find("SelectIcon").GetComponentInChildren<SpriteRenderer>();
        this.infoText = this.transform.Find("Canvas").GetComponentsInChildren<Text>()[0];
        this.levelText = this.transform.Find("Canvas").GetComponentsInChildren<Text>()[1];
    }

    // Start is called before the first frame update
    void Start() {
        this.infoText.text = "";
        this.infoText.color = Color.green;
        
    }

    void OnGUI() {
        this.loadInitialLevel();
    }

}
