using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    public void OnClickButton()
    {   
        // swich scence to "Camera"
        SceneManager.LoadScene ("Camera");
        return;
    }
}


