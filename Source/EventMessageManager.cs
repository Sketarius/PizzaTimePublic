using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMessageManager : MonoBehaviour
{
    // Shadow and Front
    public SpriteRenderer startMessage;
    public SpriteRenderer levelUpMessage;
    public SpriteRenderer infestedMessage;

    // Start is called before the first frame update
    void Start() {
        
    }

    IEnumerator startShowMessage() {
        float alpha = 0;
        Transform messageTransform = startMessage.transform;
        float reset_x = messageTransform.localScale.x;
        float reset_y = messageTransform.localScale.y;
        while (alpha  < 1f) {            
                startMessage.color = new Color(startMessage.color.r, startMessage.color.g, startMessage.color.b, alpha);
                alpha += 0.01f;            
            yield return new WaitForSeconds(0.00001f);
        }
        yield return new WaitForSeconds(0.5f);
        while (alpha > 0f) {            
                startMessage.color = new Color(startMessage.color.r, startMessage.color.g, startMessage.color.b, alpha);
                messageTransform.localScale = new Vector3(messageTransform.localScale.x + 1, messageTransform.localScale.y+1);
                alpha -= 0.01f;
            
            yield return new WaitForSeconds(0.00001f);
        }

        // Reset scale.
        messageTransform.localScale = new Vector3(reset_x, reset_y); ;
    }

    IEnumerator startInfestedMessage() {
        float alpha = 0;
        Transform messageTransform = infestedMessage.transform;
        float reset_x = messageTransform.localScale.x;
        float reset_y = messageTransform.localScale.y;
        while (alpha < 1f) {
            infestedMessage.color = new Color(infestedMessage.color.r, infestedMessage.color.g, infestedMessage.color.b, alpha);
            alpha += 0.01f;
            yield return new WaitForSeconds(0.00001f);
        }
        //yield return new WaitForSeconds(0.5f);
        while (alpha > 0f) {
            infestedMessage.color = new Color(infestedMessage.color.r, infestedMessage.color.g, infestedMessage.color.b, alpha);
            messageTransform.localScale = new Vector3(messageTransform.localScale.x + 1, messageTransform.localScale.y + 1);
            alpha -= 0.01f;

            yield return new WaitForSeconds(0.00001f);
        }

        // Reset scale.
        messageTransform.localScale = new Vector3(reset_x, reset_y); ;
    }

    IEnumerator startLevelUpMessage() {
        float alpha = 1;
        Transform messageTransform = levelUpMessage.transform;
        float pos_reset_x = messageTransform.localPosition.x;
        float pos_reset_y = messageTransform.localPosition.y;
        float scale_reset_x = messageTransform.localScale.x;
        float scale_reset_y = messageTransform.localScale.y;
        float rot_reset_z = messageTransform.transform.rotation.z;

        // Set Alpha 1.
        levelUpMessage.color = new Color(levelUpMessage.color.r, levelUpMessage.color.g, levelUpMessage.color.b, alpha);

        while (levelUpMessage.transform.localPosition.y < 0) {
            levelUpMessage.transform.localPosition = new Vector3(levelUpMessage.transform.localPosition.z, levelUpMessage.transform.localPosition.y + 1);               
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);
        while (alpha > 0f) {
            levelUpMessage.color = new Color(levelUpMessage.color.r, levelUpMessage.color.g, levelUpMessage.color.b, alpha);
            messageTransform.localScale = new Vector3(messageTransform.localScale.x + 1, messageTransform.localScale.y + 1);
            messageTransform.rotation = new Quaternion(messageTransform.rotation.x, messageTransform.rotation.y, messageTransform.rotation.z + 0.04f, messageTransform.rotation.w);
            alpha -= 0.01f;

            yield return new WaitForSeconds(0.01f);
        }
        messageTransform.localScale = new Vector3(scale_reset_x, scale_reset_y);
        messageTransform.localPosition = new Vector3(pos_reset_x, pos_reset_y);
        messageTransform.rotation = new Quaternion(messageTransform.rotation.x, messageTransform.rotation.y, rot_reset_z, messageTransform.rotation.w);
    }

    public void showLevelUpMessage() {
        StartCoroutine(startLevelUpMessage());
    }

    public void showStartMessage() {
        StartCoroutine(startShowMessage());
    }

    public void showInfestedMessage() {
        StartCoroutine(startInfestedMessage());
    }

    // Update is called once per frame
    void Update() {
        
    }
}
