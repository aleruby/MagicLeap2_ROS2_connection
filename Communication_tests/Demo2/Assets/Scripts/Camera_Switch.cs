using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Switch : MonoBehaviour
{

    public GameObject First_Person_Camera;
    public GameObject Third_Person_Camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            bool is1PActive = First_Person_Camera.activeSelf;
            First_Person_Camera.SetActive(!is1PActive);
            Third_Person_Camera.SetActive(is1PActive);
        }
    }
}
