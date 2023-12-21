// utf-8 編碼
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Android;

public class GPSLocation : MonoBehaviour
{
    public TextMeshProUGUI GPSStatus;
    public TextMeshProUGUI latitudeValue;
    public TextMeshProUGUI longitudeValue;
    public TextMeshProUGUI altitudeValue;
    public TextMeshProUGUI horizontaAccuracylValue;
    public TextMeshProUGUI timestampValue;

    private void Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        Input.compass.enabled = true;
        StartCoroutine(GPSLoc());
    }

    IEnumerator GPSLoc()
    {
        // start service before quering location
        Input.location.Start();

        // wait until service initalize
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // service didn't init in 20 second
        if (maxWait < 1)
        {
            Debug.Log("gps open time out");
            yield break;
        }

        // connection failed
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to connect gps");
            yield break;
        }
        else
        {
            // Access granted
            if(GPSStatus != null) GPSStatus.text = "Running";
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
    }// end of GPSLoc

    private void UpdateGPSData()
    {
        if(GPSStatus != null)
        {
            if(Input.location.status == LocationServiceStatus.Running)
            {
                // Access granted to GPS values and it has been init
                GPSStatus.text = "Running";
                latitudeValue.text = "latitude" + Input.location.lastData.latitude.ToString();
                longitudeValue.text = "longitude" + Input.location.lastData.longitude.ToString();
                altitudeValue.text = "altitude" + Input.location.lastData.altitude.ToString();
                horizontaAccuracylValue.text = "horizontalAccuracy" + Input.location.lastData.horizontalAccuracy.ToString();
                timestampValue.text = "timestamp" + Input.location.lastData.timestamp.ToString();
            }
            else
            {
                // service is stoped
                GPSStatus.text = "Stop";
            }
        }
    }// end of UpdateGPSData

    // 取得目前位置的經緯度
    public Vector2 GetLatAndLon()
    {
        return new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }
}