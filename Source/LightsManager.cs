using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour
{
    public GameObject[] arcadeMachineLights;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < arcadeMachineLights.Length; i++) {
            arcadeMachineLights[i].transform.Rotate(Vector3.forward);
        }
    }
}
