using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public Player thePlayer;
    public Slider playerXPBar;

    private Text accountFundsText;
    private SpriteRenderer openCloseSpriteRend;
    private Sprite openSprite;
    private Sprite closeSprite;
    private bool accountIsNegative = false;

    private Text xpAdd;
    private bool xpAddDisplayisBeingUsed = false;

    // Start is called before the first frame update
    void Start()
    {
        this.accountFundsText = GameObject.Find("/Main Camera/User Interface/Canvas/Account Funds").GetComponent<Text>();
        this.accountFundsText.text = thePlayer.getPlayerMoney().ToString();
        
        this.openSprite = Resources.Load<Sprite>("Sprites/open_sprite");
        this.closeSprite = Resources.Load<Sprite>("Sprites/close_sprite");
        this.openCloseSpriteRend = GameObject.Find("/Main Camera/User Interface/Canvas/StoreStatusSprite").GetComponent<SpriteRenderer>();
        this.xpAdd = GameObject.Find("/Main Camera/User Interface/Canvas/XPAddSub").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        this.displayPlayerMoney();

        if(thePlayer.playerMoney < 0) {
            this.accountIsNegative = true;
        } else this.accountIsNegative = false;

        playerXPBar.value = (float) thePlayer.getPlayerXP() / thePlayer.getXPForNextLevel(thePlayer.getPlayerLevel());
    }

    public void displayOpenSign() {
        this.openCloseSpriteRend.sprite = openSprite;
    }

    public void displayCloseSign() {
        this.openCloseSpriteRend.sprite = closeSprite;
    }

    private void displayPlayerMoney() {
        this.accountFundsText.text = thePlayer.getPlayerMoney().ToString("C");
        if (accountIsNegative) {
            this.accountFundsText.color = Color.red;
        } else this.accountFundsText.color = Color.white;
    }

    public void updateFunds(float begin, float end) {
        StartCoroutine(animateFunds(begin, end));
    }


    public void animateXP(int xp) {
        //if (!xpAddDisplayisBeingUsed) {
            //xpAddDisplayisBeingUsed = true;
            StartCoroutine(startAnimateAddXP(xp));
        //}
    }
    
    IEnumerator startAnimateAddXP(int xp) {
        float alpha = 0;

        while(xpAddDisplayisBeingUsed) {
            yield return null;
        }

        xpAddDisplayisBeingUsed = true;

        xpAdd.text = "+ " + xp.ToString() + " XP";
        xpAdd.color = new Color(xpAdd.color.r, xpAdd.color.g, xpAdd.color.b, alpha);
        while (xpAdd.color.a < 1.0f) {
            alpha = alpha + 0.09f;
            xpAdd.color = new Color(xpAdd.color.r, xpAdd.color.g, xpAdd.color.b, alpha);
            yield return new WaitForSeconds(0.09f);
        }

        yield return new WaitForSeconds(0.02f);

        while (xpAdd.color.a > 0.0f) {
            alpha = alpha - 0.09f;
            xpAdd.color = new Color(xpAdd.color.r, xpAdd.color.g, xpAdd.color.b, alpha);
            yield return new WaitForSeconds(0.09f);
        }
        xpAdd.text = "";
        xpAddDisplayisBeingUsed = false;
    }

    IEnumerator animateFunds(float begin, float end) {
        float current = begin;
        this.accountFundsText.text = current.ToString("C");
        if (begin < end) {
            while (current < end) {
                if ((end - current) < 0.50f) {
                    current = current + 0.01f;
                } else {
                    current = current + 0.25f;
                }
                yield return null;
                this.accountFundsText.text = current.ToString("C");
            }
        } else {
            while (current > end) {
                if ((current - end) < 0.50f) {
                    current = current - 0.01f;
                } else {
                    current = current - 1.25f;
                }
                yield return null;
                this.accountFundsText.text = current.ToString("C");
            }
        }
    }
}