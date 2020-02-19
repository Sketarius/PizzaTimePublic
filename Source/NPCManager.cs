using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public Accountant accountantReference;
    public Player thePlayer;
    public TrashManagement trashManagement;
    public NPC[] npcs;

    public enum NPCType { ROAMER=0, PARTY=1, KAREN=2, SECURITY=3, CHEF = 4, MOUSE=5 };

    private bool roamerNPCsAreActive = false;
    private bool partyNPCsAreActive = false;
    private bool karenNPCsAreActive = true;
    private bool securityNPCsAreActive = false;
    private bool chefNPCsAreActive = false;
    private bool mouseNPCsAreActive = false;

    void Start() {
        for(int i = 0; i < npcs.Length; i++) {
            npcs[i].hide();
        }
    }

    void Update() {
        if (!thePlayer.playerIsPaused()) {
            // If arcade is open then NPCs should walk right in.
            if (accountantReference.arcadeIsOpen() && !roamerNPCsAreActive) {
                roamerNPCsAreActive = true;
                for (int i = 0; i < npcs.Length; i++) {
                    // Only roamers will visit when arcade first opens.
                    if (npcs[i].getNPCType() == (int)NPCType.ROAMER) {
                        npcs[i].visitArcade();
                    }
                }
            }

            // If arcade is closed, then NPCs need to go home.
            if (!accountantReference.arcadeIsOpen() && roamerNPCsAreActive) {
                roamerNPCsAreActive = false;
                partyNPCsAreActive = false;
                for (int i = 0; i < npcs.Length; i++) {
                    npcs[i].hide();
                }
                resetNPCPositions();
            }

            // At level 4, showroom opens. Party NPCs appear.
            if (accountantReference.arcadeIsOpen() && !partyNPCsAreActive && thePlayer.getPlayerLevel() > 3) {
                partyNPCsAreActive = true;
                for (int i = 0; i < npcs.Length; i++) {
                    if (npcs[i].npcType == (int)NPCType.PARTY) {
                        npcs[i].visitArcade();
                    }
                }
            }

            // Karen handler
            if (accountantReference.arcadeIsOpen() && karenNPCsAreActive) {
                for (int i = 0; i < npcs.Length; i++) {
                    if (npcs[i].npcType == (int)NPCType.KAREN) {
                        npcs[i].visitArcade();
                        npcs[i].handleSpeech();
                    }
                }
            }

            // Security handler
            if (securityNPCsAreActive) {
                for (int i = 0; i < npcs.Length; i++) {
                    if (npcs[i].npcType == (int)NPCType.SECURITY) {
                        npcs[i].visitArcade();
                    }
                }
            }

            if (accountantReference.arcadeIsOpen() && !chefNPCsAreActive) {
                chefNPCsAreActive = true;
                for (int i = 0; i < npcs.Length; i++) {
                    // Only roamers will visit when arcade first opens.
                    if (npcs[i].getNPCType() == (int)NPCType.CHEF) {
                        npcs[i].visitArcade();
                    }
                }
            }

            if (!mouseNPCsAreActive && trashManagement.arcadeIsInfested()) {
                mouseNPCsAreActive = true;
                for (int i = 0; i < npcs.Length; i++) {
                    if (npcs[i].getNPCType() == (int)NPCType.MOUSE) {
                        npcs[i].visitArcade();
                    }
                }
            }
        }
    }

    public void activateNPCType(int npcType) {
        for(int i = 0; i < npcs.Length; i++) {
            if (npcs[i].npcType == npcType) {
                npcs[i].visitArcade();
            }
        }
    }

    public void resetNPCPositions() {
        for(int i = 0; i < npcs.Length; i++) {
            npcs[i].transform.position = npcs[i].getHomePosition();
        }
    }
}
