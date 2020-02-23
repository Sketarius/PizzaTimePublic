using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Player Level
    private int playerLevel = 1;
    // Player XP
    private long playerXP = 0;
    // Money
    public float playerMoney;
    public float runSpeed = 1.2f;

    private TalentTreeNode talentTree;

    // A reference to the Dialog System
    public GameObject dialogObject;
    // Reference to MenuUI
    public GameObject menUIReference;
    // Reference to Computer Menu UI
    public GameObject computerMenuReference;

    public UserInterface userInterfaceReference;

    public PauseManager pauseManagerReference;

    public TabMenu tabMenu;

    public PowerUpManager powerupmanagerReference;
    
    public BuffNerfEffect buffNerfEffectRef;

    public TrashManagement trashManagementReference;

    public EventMessageManager eventMessageManager;

    public GameObject playerLevelUpFx;

    private float horizontal;
    private float vertical;   
    
    // Player rigidbody and camera
    private Rigidbody2D rb2d;
    private Animator animator;
    private Animation theAnimation;
    private Camera mainCamera;

    public float mainCameraZ = -10f;

    // An array of coroutines that run when the player collides with an interactable object.
    private Dictionary<Collider2D, Queue<Coroutine>> inputCoroutineDict = new Dictionary<Collider2D, Queue<Coroutine>>();

    private bool playerPaused = false;
    private bool playerFrozen = false;
    private bool gameOver = false;

    private bool tabMenuDisplayed = false;

    /*
     *  Player based audio
     */
    public AudioSource audioSource;
    public AudioClip levelUpSound;
    public AudioClip karenHitSound;

    private bool playerMovementIsDisabled = false;

    private bool playerCanInteract = false;

    [Range(0.0f, 1.0f)]
    public float minTimeBetweenActions = 0.1f;
    private System.DateTime lastInteractionTime = System.DateTime.Now;

    // First floor level Arcade reference
    GameObject firstFloorArcade;

    // When Karen hits you
    private bool playerIsDamaged = false;


    private void handlePlayerControls() {
        if (!playerMovementIsDisabled) {
            this.animator.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
            this.animator.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        }

        if(gameOver) {
            if(Input.GetButtonDown("Activate")) {
                SceneManager.LoadScene("Overworld");
            }
        }

        if (Input.GetButtonDown("Pause")) {
            if (!this.playerIsPaused()) {
                this.freezePlayer();
                this.pause();
            } else {
                this.unpause();
                this.unfreezePlayer();
            }
            
        } else if (Input.GetButtonDown("Player Attributes")) {
            if (!tabMenuDisplayed) {
                tabMenuDisplayed = true;
                tabMenu.displayPlayerAttributes();
                this.freezePlayer();
            } else {
                tabMenuDisplayed = false;
                tabMenu.hidePlayerAttributes();
                this.unpause();
            }
        }
    }

    private void camOnPlayer() {
       this.mainCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, mainCameraZ);
    }

    public int getPlayerLevel() {
        return this.playerLevel;
    }

    public long getPlayerXP() {
        return this.playerXP;
    }

    public void setPlayerLevel(int level) {
        this.playerLevel = level;
    }

    public bool gameIsOver() {
        return gameOver;
    }

    public void setGameOver() {
        gameOver = true;
    }

    public void setPlayerXP(long XP) {
        this.playerXP = XP;
    }

    public void addPlayerXP(int XP) {
        this.playerXP += XP;
        userInterfaceReference.animateXP(XP);
    }

    public void disableAnimator() {
        this.animator.enabled = false;
    }

    public void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        theAnimation = GetComponent<Animation>();
        this.mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        animator.enabled = true;

        firstFloorArcade = GameObject.Find("Arcade");

        // Code to experiment with multiple building levels.
        //go.transform.localScale = new Vector3(0f, 0f, 0f);

        //initiateTalentTree();
    }

    public void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        this.handlePlayerControls();
        this.camOnPlayer();

        if (!playerMovementIsDisabled && !this.playerIsPaused()) {
            if (!((Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.DownArrow)))) {
                rb2d.velocity = new Vector2(horizontal / 4 * runSpeed, vertical / 4 * runSpeed);
            }
        }

        if(playerFrozen) {
            rb2d.velocity = new Vector2(0, 0);
        }

        //displayMouseStats();
    }
    /*
    public void displayMouseStats() {
        if (Input.GetMouseButtonDown(0)) {
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }*/

    IEnumerator triggerInputListen(Collider2D col) {
        PizzaManager pm = col.gameObject.GetComponentInParent<PizzaManager>();
        CabinetMenuManager mm = menUIReference.GetComponentInChildren<CabinetMenuManager>();
        ComputerMenuManager cmm = computerMenuReference.GetComponent<ComputerMenuManager>();

        while (!mm.interactionMenuDisplayed() && !cmm.interactionMenuDisplayed() && !this.gameOver) {
            // Talking to NPCs
            if (Input.GetButtonDown("Activate") && !playerPaused) {
                var currentGameTime = System.DateTime.Now;
                if ((currentGameTime - lastInteractionTime).TotalSeconds >= minTimeBetweenActions) {
                    yield return null;
                } else {
                    lastInteractionTime = currentGameTime;
                }

                // Arcade Cab interaction
                if (col.gameObject.name == "ArcadeCabinet") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        this.freezePlayer();

                        // Trigger Arcade Cabinet Interaction
                        mm.triggerCabinetInteraction(col.gameObject);
                    }
                    // Let's serve a pizza!
                } else if (col.gameObject.name == "PizzaPan") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        // Pizza Manager script is on the PizzaManager object. Therefore the parent.                    
                        pm.pickUpPizza();
                    }
                // Salad Bar!
                } else if (col.gameObject.name == "SaladBarStation") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        SaladBar sb = col.gameObject.GetComponent<SaladBar>();
                        sb.replenishSaladBar();
                    }
                    // Bathroom junk!
                } else if (col.gameObject.name == "BathroomTrigger") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        Bathroom bm = col.gameObject.GetComponentInParent<Bathroom>();
                        bm.cleanBathroom();
                    }
                } else if (col.gameObject.name == "Oven") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        pm.fixOven();
                    }
                } else if (col.gameObject.name == "Computer") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        cmm.triggerComputerInteraction(col.gameObject);
                    }
                } else if (col.gameObject.name == "EnergyDrinkMachine") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        EnergyDrinkMachine edm = col.gameObject.GetComponent<EnergyDrinkMachine>();
                        edm.buyEnergyDrink();
                    }
                } else if (col.gameObject.name == "Bed") {
                    if (!trashManagementReference.playerIsHoldingTrash()) {
                        BedManager bm = col.gameObject.GetComponent<BedManager>();
                        bm.goToSleep();
                    }
                } else if (col.gameObject.name == "TrashPickup") {
                    trashManagementReference.pickUpTrash();
                } else if (col.gameObject.name == "TrashDropoff") {
                    trashManagementReference.dumpTrash();
                } else {
                    //this.freezePlayer();
                    //dialogObject.SendMessage("fetchDialog", col.gameObject.name);
                }
            // Object collisions requiring no input (activation).
            } else {
                if (col.gameObject.name == "Powerup") {
                    PowerUp pu = col.gameObject.GetComponent<PowerUp>();
                    if (pu.spriteRender.color.a > 0.0f) {
                        pu.spriteRender.color = new Color(pu.spriteRender.color.r, pu.spriteRender.color.g, pu.spriteRender.color.b, 0f);
                        powerupmanagerReference.powerUpAcquire(pu);
                    }
                } else if (col.gameObject.name == "NPC_Karen") {
                    if (!playerIsDamaged) {
                        showDamageAnimation();
                    }
                }
            }
            yield return null;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D col) {
        playerCanInteract = true;

        if (!inputCoroutineDict.ContainsKey(col)) {
            inputCoroutineDict[col] = new Queue<Coroutine>();
        }

        inputCoroutineDict[col].Enqueue(StartCoroutine(triggerInputListen(col)));

        if (col.gameObject.name == "ArcadeCabinet") {
            col.gameObject.GetComponent<ArcadeCabinet>().showSelectIcon();
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log(collision.gameObject.name);
    }*/

    void OnTriggerStay2D(Collider2D col) {
        if (playerCanInteract && !menUIReference.GetComponentInChildren<CabinetMenuManager>().interactionMenuDisplayed()) {
            inputCoroutineDict[col].Enqueue(StartCoroutine(triggerInputListen(col)));
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        playerCanInteract = false;
        while (inputCoroutineDict[col].Count > 0) {
            var coroutineToStop = inputCoroutineDict[col].Dequeue();
            if (coroutineToStop != null) {
                StopCoroutine(coroutineToStop);
            }
        }
        
        if(col.gameObject.name == "ArcadeCabinet") { 
            col.gameObject.GetComponent<ArcadeCabinet>().hideSelectIcon();
        }
    }

    private void initiateTalentTree() {
        // Create root node.
        TalentTreeNode root;
        talentTree = new TalentTreeNode(true, null);
        root = talentTree;

        // New Skill added to first Left Branch
        Skill moneyIntervalLevel1 = new Skill(Skill.MONEY_INTERVAL_EARN_SKILL, "attribute_money_icon");
        moneyIntervalLevel1.setSkillDescription((moneyIntervalLevel1.getSkillLevel() * 5) + "% increase in cash earned every gain interval");
        talentTree.addTalentNode(moneyIntervalLevel1);

        // Return to root
        talentTree = root;

        // New Skill added to center branch
        Skill xpIntervalLevel1 = new Skill(Skill.XP_INTERVAL_EARN_SKILL);
        xpIntervalLevel1.setSkillDescription((xpIntervalLevel1.getSkillLevel() * 5) + "% increase in XP earned every gain interval");
        talentTree.addTalentNode(xpIntervalLevel1);

        // Return to root
        talentTree = root;

        // New Skill added to first right branch
        Skill speedLevel1 = new Skill(Skill.PLAYER_SPEED_SKILL);
        speedLevel1.setSkillDescription((speedLevel1.getSkillLevel() * 5) + "% increase in player speed");
        talentTree.addTalentNode(speedLevel1);
    }

    IEnumerator startDamageAnimation() {
        int blink_times = 12;
        int current = 0;
        SpriteRenderer playerSR = this.GetComponent<SpriteRenderer>();
        buffNerfEffectRef.playHitAnimation();
        this.audioSource.clip = this.karenHitSound;
        this.audioSource.Play();
        while (current < blink_times) {
            playerSR.color = new Color(playerSR.color.r, playerSR.color.g, playerSR.color.b, 0);
            yield return new WaitForSeconds(0.05f);
            playerSR.color = new Color(playerSR.color.r, playerSR.color.g, playerSR.color.b, 1);
            yield return new WaitForSeconds(0.05f);
            current++;
        }
        playerIsDamaged = false;
    }

    public void showDamageAnimation() {
        playerIsDamaged = true;
        StartCoroutine(startDamageAnimation());
    }

    public void setPlayerMoney(float playerMoney) {
        this.playerMoney = playerMoney;
    }

    public void setPlayerMovementSpeed(float speed) {
        this.runSpeed = speed;
    }

    public float getPlayerMovementSpeed() {
        return this.runSpeed;
    }

    public void enablePlayerMovement() {
        this.playerMovementIsDisabled = false;
    }

    public void disablePlayerMovement() {
        this.playerMovementIsDisabled = true;
    }

    public float getPlayerMoney()
    {
        return this.playerMoney;
    }

    public bool playerIsPaused() {
        return this.playerPaused;
    }

    public void pause() {
        //Time.timeScale = 0;
        //pauseManagerReference.displayPauseMenu();
        pauseManagerReference.triggerComputerInteraction();
        this.playerMovementIsDisabled = true;
        this.playerPaused = true;
    }

    public void freezePlayer() {
        this.playerPaused = true;
        this.playerMovementIsDisabled = true;
        this.playerFrozen = true;
    }

    public void unfreezePlayer() {
        this.playerMovementIsDisabled = false;
        this.playerFrozen = false;
    }

    public void unpause() {
        Time.timeScale = 1;
        pauseManagerReference.hidePauseMenu();
        this.playerMovementIsDisabled = false;
        this.playerPaused = false;
        this.playerFrozen = false;
    }

    public long getXPForNextLevel(int level) {
        long ret = 0;
        ret = 1000;
        int i = 1;

        do {
            ret *= level;   
            i++;
        } while (i < level);

        if (this.playerXP > ret) {
            this.playerLevel = this.playerLevel + 1;
            this.audioSource.clip = this.levelUpSound;
            this.audioSource.Play();
            playerLevelUpFx.GetComponent<Animator>().Play("level_up_fx", -1, 0f);
            eventMessageManager.showLevelUpMessage();
        }
        return ret;
    }

    public TalentTreeNode getTalentTree() {
        return talentTree;
    }
}
