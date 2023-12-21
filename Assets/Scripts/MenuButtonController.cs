using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void OnClickButton()
    {   
        // swich scence to "Camera"
        SceneManager.LoadScene ("Camera");
        return;
    }
}


