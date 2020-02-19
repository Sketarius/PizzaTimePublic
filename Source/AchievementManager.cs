using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchievementManager : MonoBehaviour
{
    public Accountant theAccountant;
    public Player thePlayer;

    public bool first_hundred_1 = false;

    // Start is called before the first frame update
    void Start()
    {
        SteamUserStats.ResetAllStats(true);
    }

    // Update is called once per frame
    void Update() {
        if (!first_hundred_1) {
            if (thePlayer.getPlayerMoney() > 100.00f) {
                Debug.Log("Got 1st achievement!");
                SteamUserStats.SetAchievement("1_FIRST_HUNDRED");
                SteamUserStats.StoreStats();
                first_hundred_1 = true;
            }
        }
    }
}
