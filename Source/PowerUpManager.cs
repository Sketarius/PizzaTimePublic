using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public Player thePlayer;
    public Accountant accountantReference;
    public int percentageOfAppearing;
    public PowerUp[] thePowerUps;
    public EnergyDrinkMachine energyDrinkMachine;

    public float secondsPowerUpRoll;
    public float powerUpWaitTime;


    public AudioSource powerUpAudioSource;
    public AudioClip powerUpAppearSfx;
    public AudioClip powerUpAcquireSfx;
    
    // All possible sprites
    public Sprite xpPowerSprite;
    public Sprite cashPowerSprite;
    public Sprite speedPowerSprite;
    public Sprite equipmentEnvincSprite;

    public enum powerupTypes { XP_POWERUP, CASH_POWERUP, SPEED_POWERUP, EQUIPMENT_INVINCIBILITY };

    private int powerUpInstances;
    private bool isPowerUpVisible = false;

    IEnumerator powerUpManagerTick() {
        System.Random rnd = new System.Random(System.Guid.NewGuid().GetHashCode());
        int powerUpCoin = 0;
        int powerUpLocation = -1;
        int powerUpType = -1;
        Sprite powerUpSprite = null;
        while (true) {
            if (accountantReference.arcadeIsOpen()) {
                // While no powerups are visible and arcade is open.
                while (!isPowerUpVisible) {
                    yield return new WaitForSeconds(secondsPowerUpRoll);
                    powerUpCoin = rnd.Next(1, 100);
                    Debug.Log("Power up rolled: " + powerUpCoin);
                    if (powerUpCoin > (100 - percentageOfAppearing)) {
                        powerUpLocation = rnd.Next(0, powerUpInstances);
                        powerUpType = rnd.Next(0, System.Enum.GetNames(typeof(powerupTypes)).Length);
                        thePowerUps[powerUpLocation].setPowerUpType(powerUpType);
                        Debug.Log("Power up should appear!");
                        switch (powerUpType) {
                            case (int)powerupTypes.XP_POWERUP:
                                powerUpSprite = xpPowerSprite;
                                thePowerUps[powerUpLocation].generateXPValue();
                                break;
                            case (int)powerupTypes.CASH_POWERUP:
                                powerUpSprite = cashPowerSprite;
                                thePowerUps[powerUpLocation].generateCashValue();
                                break;
                            case (int)powerupTypes.SPEED_POWERUP:
                                powerUpSprite = speedPowerSprite;
                                break;
                            case (int)powerupTypes.EQUIPMENT_INVINCIBILITY:
                                powerUpSprite = equipmentEnvincSprite;
                                break;
                        }
                        isPowerUpVisible = true;
                    }
                }

                while (isPowerUpVisible) {
                    float spriteAlpha = 0f;

                    thePowerUps[powerUpLocation].spriteRender.sprite = powerUpSprite;
                    thePowerUps[powerUpLocation].spriteRender.color = new Color(thePowerUps[powerUpLocation].spriteRender.color.r, thePowerUps[powerUpLocation].spriteRender.color.g, thePowerUps[powerUpLocation].spriteRender.color.b, spriteAlpha);
                    powerUpAudioSource.clip = powerUpAppearSfx;
                    powerUpAudioSource.Play();

                    while (spriteAlpha <= 1f) {
                        thePowerUps[powerUpLocation].spriteRender.color = new Color(thePowerUps[powerUpLocation].spriteRender.color.r, thePowerUps[powerUpLocation].spriteRender.color.g, thePowerUps[powerUpLocation].spriteRender.color.b, spriteAlpha);
                        spriteAlpha = spriteAlpha + 0.05f;
                        yield return null;
                    }

                    yield return new WaitForSeconds(powerUpWaitTime);

                    // Player could have acquired the powerup, so fading out isn't necessary.
                    if (isPowerUpVisible) {
                        while (spriteAlpha > 0f) {
                            thePowerUps[powerUpLocation].spriteRender.color = new Color(thePowerUps[powerUpLocation].spriteRender.color.r, thePowerUps[powerUpLocation].spriteRender.color.g, thePowerUps[powerUpLocation].spriteRender.color.b, spriteAlpha);
                            spriteAlpha = spriteAlpha - 0.05f;
                            yield return null;
                        }
                        // Reset Alpha on returning.
                        thePowerUps[powerUpLocation].spriteRender.sprite = null;
                        thePowerUps[powerUpLocation].spriteRender.color = new Color(thePowerUps[powerUpLocation].spriteRender.color.r, thePowerUps[powerUpLocation].spriteRender.color.g, thePowerUps[powerUpLocation].spriteRender.color.b, 0.0f);
                        isPowerUpVisible = false;
                    }
                }
            } else {
                yield return null;
            }
        } 
    }

    public void setPowerUpVisible(bool visible) {
        isPowerUpVisible = visible;
    }

    public void powerUpAcquire(PowerUp p) {

        switch(p.getPowerUpType()) {
            // Retrieve cash value for player.
            case (int)powerupTypes.CASH_POWERUP:
                thePlayer.setPlayerMoney(thePlayer.getPlayerMoney() + p.getCashValue());
                break;
            // Retrieve XP value for player.
            case (int)powerupTypes.XP_POWERUP:
                thePlayer.setPlayerXP(thePlayer.getPlayerXP() + p.getXPValue());
                break;
            case (int)powerupTypes.SPEED_POWERUP:
                energyDrinkMachine.giveEnergyDrink();
                break;
        }


        isPowerUpVisible = false;
        powerUpAudioSource.clip = powerUpAcquireSfx;
        powerUpAudioSource.Play();
    }

    void Awake() {
        powerUpInstances = thePowerUps.Length;
    }
    
    // Start is called before the first frame update
    void Start() {        
        StartCoroutine(powerUpManagerTick());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
