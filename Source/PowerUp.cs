using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private int powerupType = 0;
    public Sprite powerUpSprite = null;

    public float maxCashValue = 1000.00f;
    public float minCashValue = 0.00f;

    public int minXPValue = 0;
    public int maxXPValue = 1000;

    public SpriteRenderer spriteRender = null;

    private float cashValue = 0;
    private int xpValue = 0;
    
    public void setPowerUpType(int powerupType) {
        this.powerupType = powerupType;
    }

    public int getPowerUpType() {
        return this.powerupType;
    }

    public void generateCashValue() {
        System.Random rnd = new System.Random(System.Guid.NewGuid().GetHashCode());
        cashValue = (float) rnd.NextDouble() * (maxCashValue - minCashValue) + minCashValue;
    }
    public void generateXPValue() {
        System.Random rnd = new System.Random(System.Guid.NewGuid().GetHashCode());
        xpValue = rnd.Next(minXPValue, maxXPValue);
    }

    public float getCashValue() {
        return cashValue;
    }

    public int getXPValue() {
        return xpValue;
    }

    // Start is called before the first frame update
    void Awake() {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
