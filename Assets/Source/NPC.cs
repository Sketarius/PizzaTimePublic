using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Player thePlayer;
    
    private Animator animator;
    public float speed;
    public int startingPoint;

    private float waitTime;
    public float startWaitTime;

    public Transform[] moveSpots;

    public Text infoText;

    public int npcType = (int) NPCManager.NPCType.ROAMER;

    string[] karen_cycles = {
                            "Manager, NOW!",
                            "!@#$",
                            "My husband is coming!",
                            "I deserve this!",
                            "I'll sue!",
                            "My pizza was cold!",
                            "The bathrooms are dirty!",
                            "You've just lost a customer!",
                            "BUT I'M STAYING!"
    };

    public GameObject speechBubble;
    private bool karenIsTalking = false;


    public void Start() {
        waitTime = startWaitTime;
        animator = GetComponent<Animator>();
        

        // This be a Karen!
        if (npcType == (int)NPCManager.NPCType.KAREN) {
            speechBubble.SetActive(false);
        }
    }

    public void Update() {
        walkPath();
    }

    public void handleSpeech() {
        if (!karenIsTalking) {
            StartCoroutine(karenSpeechCycle());
            karenIsTalking = true;
        }
    }

    IEnumerator karenSpeechCycle() {
        int current = 0;
        Text bubbbleText = speechBubble.GetComponentInChildren<Text>();
        while (true) {            
            bubbbleText.text = karen_cycles[current];
            speechBubble.SetActive(true);
            yield return new WaitForSeconds(1f);
            speechBubble.SetActive(false);
            yield return new WaitForSeconds(2f);
            if (current == karen_cycles.Length -1) {
                current = 0;
            } else {
                current++;
            }
        }
    }

    private void getDirection(Vector3 origin, Vector3 destination) {
        float Xdiff;
        float Ydiff;

        Xdiff = (origin.x > destination.x) ? origin.x - destination.x : destination.x - origin.x;
        Ydiff = (origin.y > destination.y) ? origin.y - destination.y : destination.y - origin.y;

        if ((Xdiff != 0.0f) || (Ydiff != 0.0f)) {
            this.animator.enabled = true;
            if (Xdiff > Ydiff) {
                animator.SetFloat("MoveY", 0f);
                if (origin.x < destination.x) {
                    this.animator.SetFloat("MoveX", 0.6f);
                } else {
                    this.animator.SetFloat("MoveX", -0.6f);
                }
            } else {
                animator.SetFloat("MoveX", 0f);
                if (origin.y > destination.y) {
                    this.animator.SetFloat("MoveY", -0.6f);
                } else {
                    this.animator.SetFloat("MoveY", 0.6f);
                }
            }
        } else {
            this.animator.enabled = false;
        }
    }

    public void hide() {
        this.transform.gameObject.SetActive(false);
    }

    public void visitArcade() {
        this.transform.gameObject.SetActive(true);
    }
    
    public int getNPCType() {
        return npcType;
    }

    private void walkPath() {
        if (!thePlayer.playerIsPaused()) {
            if (moveSpots.Length > 0) {
                // Get direction and animate accordingly.
                getDirection(transform.position, moveSpots[startingPoint].position);

                // Move position to destination.
                transform.position = Vector2.MoveTowards(transform.position, moveSpots[startingPoint].position, speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, moveSpots[startingPoint].position) < 0.2f) {
                    if (waitTime <= 0) {
                        //randomSpot = Random.Range(0, moveSpots.Length);
                        if ((startingPoint + 1) == moveSpots.Length) {
                            startingPoint = 0;
                        } else {
                            startingPoint++;
                        }

                        waitTime = startWaitTime;
                    } else {
                        waitTime -= Time.deltaTime;
                    }
                }
            }
        }
    }

    public Vector3 getHomePosition() {
        return this.moveSpots[0].transform.position;
    }
}

