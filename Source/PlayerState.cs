using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static long playerXP;
    public static float playerMoney;
    public static int playerLevel;
    public static System.DateTime gameTime;
    public static int dayNumber;
    public static bool saladBarIsFull;
    public static bool bathroomIsClean;
    public static string tutorialsExplained;
    public static List<ArcadeCabinet> cabinets = new List<ArcadeCabinet>();
    public static bool playerDataLoaded = false;

    public static void writeSaveFile(Player thePlayer, Accountant theAccountant, TutorialSpeechManager tutorialSpeechManager) {
        long playerXP = thePlayer.getPlayerXP();
        float playerMoney = thePlayer.getPlayerMoney();
        int playerLevel = thePlayer.getPlayerLevel();
        System.DateTime gameTime = theAccountant.getGameTime();
        ArcadeCabinet[] cabinets = GameObject.FindObjectsOfType<ArcadeCabinet>();

        using (System.IO.StreamWriter file = new System.IO.StreamWriter("save.dat")) {
            file.WriteLine(playerXP.ToString());
            file.WriteLine(playerMoney.ToString());
            file.WriteLine(playerLevel.ToString());
            file.WriteLine(gameTime.ToString());
            file.WriteLine(theAccountant.getDayNumber().ToString());
            file.WriteLine(theAccountant.getSaladBar().saladBarIsFull().ToString());
            file.WriteLine(theAccountant.bathroomReference.isBathroomClean().ToString());
            file.WriteLine(tutorialSpeechManager.generateSaveString());

            foreach (ArcadeCabinet cab in cabinets) {
                file.WriteLine(cab.UID + "," + cab.cabinetLevel + "," + cab.GameTitle + "," + cab.priceToPlay + "," + cab.priceToRepair + "," + cab.machineBroken);
            }
        }
    }

    public static void loadSave() {
        string[] lines = System.IO.File.ReadAllLines("save.dat");
        cabinets.Clear();

        for (int i = 0; i < lines.Length; i++) {
            switch(i) {
                // Player XP
                case 0:
                    playerXP = long.Parse(lines[i]);
                    break;
                // Player Money
                case 1:
                    playerMoney = float.Parse(lines[i]);
                    break;
                // Player Level
                case 2:
                    playerLevel = int.Parse(lines[i]);
                    break;
                // Game Time
                case 3:
                    gameTime = System.DateTime.Parse(lines[i]); 
                    break;
                // Day Number
                case 4:
                    dayNumber = int.Parse(lines[i]);
                    break;
                // Salad Bar
                case 5:
                    saladBarIsFull = bool.Parse(lines[i]);
                    break;
                // Bathroom
                case 6:
                    bathroomIsClean = bool.Parse(lines[i]);
                    break;
                // Tutorials
                case 7:
                    tutorialsExplained = lines[i];
                    break;
                // Arcade Machines
                default:
                    string[] tokens = lines[i].Split(',');
                    ArcadeCabinet cab = new ArcadeCabinet();
                    for (int j = 0; j < tokens.Length; j++) {
                        switch (j) {
                            // UID
                            case 0:
                                cab.UID = int.Parse(tokens[j]);
                                break;
                            // Cabinet Level
                            case 1:
                                cab.cabinetLevel = int.Parse(tokens[j]);
                                break;
                            // Game Title
                            case 2:
                                cab.GameTitle = tokens[j];
                                break;
                            // Cost to Play
                            case 3:
                                cab.priceToPlay = float.Parse(tokens[j]);
                                break;
                            // Cost to Repair
                            case 4:
                                cab.priceToRepair = float.Parse(tokens[j]);
                                break;
                            // Cabinet is broken
                            case 5:
                                cab.machineBroken = bool.Parse(tokens[j]);
                                break;
                        }
                    }
                    cabinets.Add(cab);
                    break;
            }
        }

        playerDataLoaded = true;
    }
}
