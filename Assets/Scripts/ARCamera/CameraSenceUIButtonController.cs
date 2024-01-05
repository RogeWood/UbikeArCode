// utf-8 編碼
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSenceUIButtonController : MonoBehaviour
{
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private GameObject googleMapPanel;
    [SerializeField] private GameObject ubikeStationPanel;

    [SerializeField] private GameObject panelGroup;
    [SerializeField] private GoogleMapAPIControl googleMapAPIControl;

    private GameObject selectedLocationMark = null;
    private Vector3 openPostition = new Vector3(-7, 600, 0);
    private Vector3 closePostiton = new Vector3(-7, 30, 0);
    private bool isOpen = true;

    private void Start()
    {
        panelGroup.SetActive(false);
        openPostition = panelGroup.transform.position;
        // inactive/active plane
        // googleMapPanel.SetActive(false);
        // ubikeStationPanel.SetActive(false);
        // informationPanel.SetActive(true);
    }

    public void OnClickGoogleMapButton()
    {
        panelGroup.SetActive(true);
        informationPanel.SetActive(false);
        googleMapPanel.SetActive(true);
        ubikeStationPanel.SetActive(false);
    }

    public void OnClickInformationButton()
    {
        panelGroup.SetActive(true);
        informationPanel.SetActive(true);
        googleMapPanel.SetActive(false);
        ubikeStationPanel.SetActive(false);
    }

    public void OnClickSearchUbikeStation()
    {
        panelGroup.SetActive(true);
        informationPanel.SetActive(false);
        googleMapPanel.SetActive(false);
        ubikeStationPanel.SetActive(true);

        // delete origin location mark
        GameObject[] gos = GameObject.FindGameObjectsWithTag("LocationMark");
        foreach (GameObject go in gos)
            Destroy(go);
        // show location mark
        StartCoroutine(googleMapAPIControl.GetRequestNearUbikeStation());
    }


    public void onClikeLockLocationMark()
    {
        // get this station information
        Debug.Log("get this station information");
    }

    public void onClikeControlPanel()
    {
        panelGroup.SetActive(false);
        return;
        // Debug.Log(openPostition);
        // if (isOpen)
        // {
        //     panelGroup.transform.position = new Vector3(openPostition.x, openPostition.y-200, openPostition.z);
        //     isOpen = false;
        // }
        // else
        // {
        //     panelGroup.transform.position = openPostition;
        //     isOpen = true;
        // }
    }

    public void onClikeDeleteLocaitonMark()
    {
        if(selectedLocationMark)
        {
            Destroy(selectedLocationMark);
        }
    }
    public void setLocationMark(GameObject obj)
    {
        selectedLocationMark = obj;
    }
}
