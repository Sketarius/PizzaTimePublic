using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityManager : MonoBehaviour
{
    public Player thePlayer;
    public NPCManager npcManager;
    public float securityPrice;

    private bool securityHired = false;
    
    void Start() {
        
    }

    public void purchaseSecurity() {
        thePlayer.setPlayerMoney(thePlayer.getPlayerMoney() - securityPrice);
        npcManager.activateNPCType((int) NPCManager.NPCType.SECURITY);
        securityHired = true;
    }

    public bool getSecurityHired() {
        return securityHired;
    }

    void Update() {
        
    }
}
