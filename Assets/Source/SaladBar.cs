using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaladBar : MonoBehaviour
{
    public Player thePlayer;
    public Accountant accountantReference;
    public NotificationCenter notificationCenterReference;
    public BuffBarManager buffBarReference;
    public TrashManagement trashManagementReference;
    public int earnedXPMultiplier = 100;

    private SpriteRenderer saladBarRenderer;
    private Sprite saladBarFull;
    private Sprite saladBarEmpty;
    private int saladBarInterval = 25;
    private bool saladBarIsEmpty = false;

    private float pricePerReplenish = 10.00f;

    private Text infoText;

    public AudioSource audioSourceReference;
    public AudioClip saladBarEmptyClip;
    public AudioClip saladBarFullClip;

    void Awake() {
        this.saladBarRenderer = this.GetComponent<SpriteRenderer>();
        this.saladBarFull = Resources.Load<Sprite>("Sprites/salad_bar_full");
        this.saladBarEmpty = Resources.Load<Sprite>("Sprites/salad_bar_empty");
        this.saladBarRenderer.sprite = saladBarFull;
    }

    // Start is called before the first frame update
    void Start() {
        this.infoText = this.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        StartCoroutine(saladBarDepleteTick());
    }

    public void replenishSaladBar() {
        if (this.saladBarIsEmpty) {
            this.saladBarRenderer.sprite = saladBarFull;
            this.infoText.text = "+ " + pricePerReplenish.ToString("C");
            thePlayer.playerMoney = thePlayer.playerMoney + pricePerReplenish;
            this.saladBarIsEmpty = false;
            buffBarReference.removeFromBuffBar(BuffBarManager.EMPTY_SALADBAR);
            this.audioSourceReference.clip = saladBarFullClip;
            this.audioSourceReference.Play();
            thePlayer.addPlayerXP(earnedXPMultiplier * thePlayer.getPlayerLevel());
            StartCoroutine(fadeInMessage());
        }
    }

    public void setSaladBarFull() {
        this.saladBarIsEmpty = false;
        this.saladBarRenderer.sprite = saladBarFull;
    }

    public void setSaladBarEmpty() {
        this.saladBarIsEmpty = true;
        this.saladBarRenderer.sprite = saladBarEmpty;
    }

    public bool saladBarIsFull() {
        return !(this.saladBarIsEmpty);
    }

    IEnumerator saladBarDepleteTick() {
        System.Random rnd = new System.Random(System.Guid.NewGuid().GetHashCode());
        while (true) {
            if (!thePlayer.playerIsPaused()) {
                this.saladBarInterval = rnd.Next(15, 25);
                yield return new WaitForSeconds(this.saladBarInterval);
                if (!saladBarIsEmpty && accountantReference.arcadeIsOpen()) {
                    this.saladBarRenderer.sprite = saladBarEmpty;
                    this.saladBarIsEmpty = true;
                    buffBarReference.addToBuffBar(BuffBarManager.EMPTY_SALADBAR);
                    this.audioSourceReference.clip = saladBarEmptyClip;
                    this.audioSourceReference.Play();
                }
                while (saladBarIsEmpty) {
                    yield return null;
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
}
