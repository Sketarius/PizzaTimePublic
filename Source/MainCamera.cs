using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Turn off v-sync
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }
}
