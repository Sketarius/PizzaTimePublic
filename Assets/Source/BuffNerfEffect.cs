using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffNerfEffect : MonoBehaviour
{
    private SpriteRenderer buffNerfEffectSR;
    private Animator animator;
    // Start is called before the first frame update
    void Start() {
        this.buffNerfEffectSR = this.GetComponentInChildren<SpriteRenderer>();
        this.buffNerfEffectSR.enabled = false;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void playDrinkEnergy() {
        this.buffNerfEffectSR.enabled = true;
        this.animator.enabled = true;
        //this.animator.Play("DrinkEnergy");
        this.animator.Play("DrinkEnergy", -1, 0f);
        //animator.GetComponentInChildren<Animation>().Rewind();
    }

    public void playHitAnimation() {
        this.buffNerfEffectSR.enabled = true;
        this.animator.enabled = true;
        //this.animator.Play("DrinkEnergy");
        this.animator.Play("Hit_Animation", -1, 0f);
        //animator.GetComponentInChildren<Animation>().Rewind();
    }

    public void hideAnimationSprite() {
        this.buffNerfEffectSR.enabled = false;
    }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Animation ended!");
    }
}
