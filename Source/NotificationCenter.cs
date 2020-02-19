using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationCenter : MonoBehaviour
{
    private Text textComponent;
    private bool notificationInProgress = false;

    // Start is called before the first frame update
    void Start() {
        this.textComponent = this.GetComponentInChildren<Text>();
        this.textComponent.color = new Color(255, 255, 255, 0);
        //this.printMessage("Testing fade in and out!");
    }

    public void printMessage(string msg) { 
        StartCoroutine(fadeInMessage(msg));
    }

    private IEnumerator fadeInMessage(string msg) {
        float alpha = 0;

        while (notificationInProgress) {
            yield return null;
        }

        this.textComponent.text = msg;

        notificationInProgress = true;

        while (this.textComponent.color.a < 1.0f)
        {
            this.textComponent.color = new Color(255, 255, 255, alpha);
            alpha = alpha + 0.01f;
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(3f);

        while (this.textComponent.color.a > 0.0f)
        {
            this.textComponent.color = new Color(255, 255, 255, alpha);
            alpha = alpha - 0.01f;
            yield return new WaitForSeconds(0.03f);
        }

        notificationInProgress = false;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
