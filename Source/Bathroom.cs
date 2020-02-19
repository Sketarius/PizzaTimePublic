using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bathroom : MonoBehaviour
{
    public Player thePlayer;
    public Accountant accountantReference;
    public AudioSource audioSourceReference;
    public BuffBarManager buffBarReference;
    public TrashManagement trashManageReference;

    public int earnedXPMultiplier = 100;

    private bool bathroomClean = true;
    private int bathroomMessInterval = 8;

    private SpriteRenderer toiletDirtSpriteRend;
    private SpriteRenderer sinkDirtSpriteRend;

    private Sprite toiletDirt;
    private Sprite sinkDirt;

    public AudioClip cleanSound;
    public AudioClip dirtySound;

    void Awake() {
        toiletDirtSpriteRend = GameObject.Find("/Arcade/Bathroom/BathroomDirtToilet").GetComponent<SpriteRenderer>();
        sinkDirtSpriteRend = GameObject.Find("/Arcade/Bathroom/BathroomDirtSink").GetComponent<SpriteRenderer>();

        toiletDirt = Resources.Load<Sprite>("Sprites/bathroom_dirt_1");
        sinkDirt = Resources.Load<Sprite>("Sprites/bathroom_dirt_2");

        toiletDirtSpriteRend.sprite = null;
        sinkDirtSpriteRend.sprite = null;
    }

    void Start() {
        StartCoroutine(bathroomTick());
    }

    public bool isBathroomClean() {
        return this.bathroomClean;
    }

    public void cleanBathroom() {
        if (!this.bathroomClean) {
            toiletDirtSpriteRend.sprite = null;
            sinkDirtSpriteRend.sprite = null;
            this.bathroomClean = true;
            buffBarReference.removeFromBuffBar(BuffBarManager.DIRTY_BATHROOM);
            audioSourceReference.clip = cleanSound;
            audioSourceReference.Play();
            thePlayer.addPlayerXP(earnedXPMultiplier * thePlayer.getPlayerLevel());
        }
    }

    public void setBathroomDIrty() {
        toiletDirtSpriteRend.sprite = toiletDirt;
        sinkDirtSpriteRend.sprite = sinkDirt;
        this.bathroomClean = false;
    }

    IEnumerator bathroomTick() {
        System.Random rnd = new System.Random(System.Guid.NewGuid().GetHashCode());
        while (true) {
            if (!thePlayer.playerIsPaused()) {
                this.bathroomMessInterval = rnd.Next(10, 20);
                yield return new WaitForSeconds(bathroomMessInterval);
                if (accountantReference.arcadeIsOpen()) {
                    toiletDirtSpriteRend.sprite = toiletDirt;
                    sinkDirtSpriteRend.sprite = sinkDirt;
                    this.bathroomClean = false;
                    buffBarReference.addToBuffBar(BuffBarManager.DIRTY_BATHROOM);
                    audioSourceReference.clip = dirtySound;
                    audioSourceReference.Play();
                }

                while (!bathroomClean) {
                    yield return null;
                }
            } else yield return null;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
