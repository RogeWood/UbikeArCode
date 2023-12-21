// utf-8 編碼
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class CompassManage : MonoBehaviour
{
    private Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;

        // check if user has location service enable
        if (!Input.location.isEnabledByUser)
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // enable compass
        Input.location.Start();
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, cam.eulerAngles.y - Input.compass.trueHeading, 0);
    }
}
