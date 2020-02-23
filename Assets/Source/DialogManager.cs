using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Player thePlayer;
    private Queue<string> sentences;
    private Canvas dialogCanvas;

    // [0] is name of NPC, [1] is message
    private Text[] dialogText;

    private IDictionary<string, string> fileDialog;

    private string currentCharacter;
    private bool isBusy = false;

    private float defaultdialogTypeSpeed = 0.05f;
    private float currentdialogTypeSpeed = 0.05f;

    public AudioClip dialogTypeAudioClip;
    public AudioSource dialogManagerSource;

    /* Looks through the 'Assets/Dialog' directory and loads
     * every file with respect to their character name into memory.
     * The dialog then can be loaded by passing the character's name into 
     * the dictionary.
     */
    private void loadDialogFilesInMemory() {
        string[] files = System.IO.Directory.GetFiles("Assets/Dialog", "*.dat");

        foreach(string file in files) {
            string characterName = file.Replace("Assets/Dialog\\", "").Replace(".dat", "");
            fileDialog.Add(characterName, System.IO.File.ReadAllText(file));
        }
    }

    public void fetchDialog(string characterName) {
        if(!this.isBusy) { 
            // If the queue is empty...
            if(sentences.Count < 1) {
                // separate the dialog by new-line
                string[] lines = this.fileDialog[characterName].Split('\n');
                // queue the current dialog until we reach an empty line + 1.
                foreach (string line in lines) {
                    if (line != "\r") {
                        sentences.Enqueue(line);
                    } else {
                        sentences.Enqueue(line);
                        break;
                    }
                }            
            }

            // Dequeue the current sentence
            string currentline = this.sentences.Dequeue();

            // Display the sentence
            if (currentline != "\r") {
                string[] message = { characterName, currentline };
                this.showDialog(message);
            // If the sentence is a blank line then we know to close the dialog box.
            } else {
                this.hideDialog();
            }
        } else this.finishDialog();
    }

    public void hideDialog() {
        this.sentences.Clear();
        this.dialogCanvas.enabled = false;
        this.thePlayer.unpause();
    }
    
    // Start is called before the first frame update
    void Start() {
        // Dialog Manager disabled until further notice.
        /*this.sentences = new Queue<string>();
        this.fileDialog = new Dictionary<string, string>();


        this.dialogCanvas = this.GetComponentInChildren<Canvas>();
        this.dialogText = this.GetComponentsInChildren<Text>();

        this.dialogCanvas.enabled = false;

        this.dialogManagerSource.clip = this.dialogTypeAudioClip;

        this.loadDialogFilesInMemory();*/
    }

    public void showDialog(string[] message) {
        /*
        this.dialogText[0].text = message[0];
        this.dialogText[1].text = message[1];
        */
        if (!this.isBusy) {
            this.dialogCanvas.enabled = true;
            StartCoroutine(typeDialog(message));
        } else this.finishDialog();
    }

    IEnumerator typeDialog(string[] message) {
        this.dialogText[0].text = message[0];
        this.isBusy = true;
        for (int i = 0; i < message[1].Length; i++) {
            this.dialogText[1].text = message[1].Substring(0, i);
            this.dialogManagerSource.Play();
            yield return new WaitForSeconds(this.currentdialogTypeSpeed);
        }
        this.isBusy = false;
        this.currentdialogTypeSpeed = this.defaultdialogTypeSpeed;

    }

    public void finishDialog() {
        this.currentdialogTypeSpeed = 0.0005f;
    }

    // Update is called once per frame
    void Update() {
        // If there is dialog on the screen...
        if (this.dialogCanvas.enabled) {

        }
    }
}
