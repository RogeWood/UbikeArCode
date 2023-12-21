// utf-8 編碼
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleMapAPIControl : MonoBehaviour
{

    [SerializeField] private string apiKey = "YOUR_API_KEY";
    [SerializeField] public float lat = 23.554517933133102f;
    [SerializeField] public float lon = 120.47184243500892f;
    [SerializeField] private int radius = 2000;
    [SerializeField] private string keyword = "單車借用站"; // restaurant
    private string url = ""; 


    [SerializeField] private CameraSenceController senceController;
    //void Start()
    //{
    //}

    // Update is called once per frame
    void Update()
    {
        // update location poistion 更新座標
        if(Input.location.lastData.latitude != 0)
        {
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;
        }
    }


    public IEnumerator GetRequestNearUbikeStation()
    {
        // 得到附近Ubike站點的資料
        //url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat + "," + lon + "&radius=" + radius + "&type=" + keyword + "&language=zh-TW&key=" + apiKey;
        url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat + "," + lon + "&radius=" + radius + "&keyword=" + keyword + "&language=zh-TW&key=" + apiKey;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    senceController.GetNearUbikeData(webRequest.downloadHandler.text);
                    break;
            }
        }
    }


    public IEnumerator GetRequestLocationData()
    {
        // 取得目前所在位置的資訊
        url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lon + "&key=" + apiKey;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public IEnumerator GetLocationDistance(float disLat, float disLon)
    {
        // 取得距離
        url = "https://maps.googleapis.com/maps/api/distancematrix/json?destinations=" + disLat + "," + disLon +
                                                                        "&origins=" + lat + "," + lon +
                                                                        "&mode=walking&language=zh-TW&key=" + apiKey;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    senceController.GetDistance(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public IEnumerator GetRequestDirectionsData(float disLat, float disLon)
    {
        //string url = @"https://maps.googleapis.com/maps/api/directions/json?origin=75+9th+Ave+New+York,+NY&destination=MetLife+Stadium+1+MetLife+Stadium+Dr+East+Rutherford,+NJ+07073&key=" + apiKey;
        url = "https://maps.googleapis.com/maps/api/directions/json?"
            + "origin=" + lat + "," + lon
            + "&destination=" + disLat + "," + disLon
            + "&travelMode=WALKING&language=zh-TW&key=" + apiKey;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    senceController.GetDirection(webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}