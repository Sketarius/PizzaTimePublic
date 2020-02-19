using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Player thePlayer;
    public AudioSource audioManagerSource;
    public AudioClip level1Clip;
    public AudioClip level1FailClip;
    public AudioClip level2Clip;
    public AudioClip playerSleepClip;

    private bool playerFailed = false;

    public void playerSleep() {
        this.audioManagerSource.clip = playerSleepClip;
        this.audioManagerSource.Play();
    }

    public void playLevel(int day) {
        switch (day) {
            case 2:
                this.audioManagerSource.clip = level2Clip;
                break;
            default:
                this.audioManagerSource.clip = level1Clip;
                break;
        }
        this.audioManagerSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.audioManagerSource.clip = this.level1Clip;
        this.audioManagerSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(thePlayer.gameIsOver() && !playerFailed) {
            this.audioManagerSource.loop = false;
            this.audioManagerSource.clip = this.level1FailClip;
            this.audioManagerSource.Play();
            playerFailed = true;
        }
    }
}
