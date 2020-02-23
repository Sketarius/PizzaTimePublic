using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public Player thePlayer;
    public NotificationCenter notificationCenter;

    public int showRoomActivationLevel = 4;
    private Light[] showRoomLights;
    private bool showRoomActivated = false;

    // Start is called before the first frame update
    void Start() {
        showRoomLights = transform.Find("Showroom").GetComponentsInChildren<Light>();
    }

    // Update is called once per frame
    void Update() {
        if(!showRoomActivated && thePlayer.getPlayerLevel() >= showRoomActivationLevel) {
            for(int i = 0; i < showRoomLights.Length; i++) {
                showRoomLights[i].intensity = 3.75f;
            }
            notificationCenter.printMessage("Showroom has been unlocked!");
            showRoomActivated = true;
        }
    }
}
