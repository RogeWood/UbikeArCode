// utf-8 編碼
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;

public class CameraSenceController : MonoBehaviour
{
    [SerializeField] private GameObject panelGroup;

    [SerializeField] private GoogleMapAPIControl googleMapAPIControl;
    [SerializeField] private CameraSenceUIButtonController buttonController;
    [SerializeField] private UbikeAPI ubikeAPI;

    [SerializeField] private TextMeshProUGUI UbikeText;
    [SerializeField] private TextMeshProUGUI debugMessage;
    [SerializeField] private GameObject locationMarkIcon;

    [SerializeField] private Transform arCamera;

    private GameObject locationMark;
    private JArray ubikeData;

    private bool isOnPress = false;

    // Start is called before the first frame update
    void Start()
    {
        // set scene world origin
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        debugMessage.text = "Camera roation: y = " + arCamera.rotation.y;
        debugMessage.text += "\ncompass roation: y = " + Input.compass.magneticHeading;

        // 偵測點畫面
        if (Input.touchCount == 0) isOnPress = false;
        if ((Input.touchCount > 0 && !isOnPress) || Input.GetMouseButtonDown(0))
        {
            isOnPress = true;

            //Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 點擊地標
                if (hit.collider.CompareTag("LocationMark"))
                {
                    // 點到地標
                    UbikeText.text = hit.transform.gameObject.GetComponent<LocationMarkController>().GetText();
                    Vector2 dis = hit.transform.gameObject.GetComponent<LocationMarkController>().GetLatAndLon();
                    buttonController.setLocationMark(hit.transform.gameObject);
                    // 取得距離
                    StartCoroutine(googleMapAPIControl.GetLocationDistance(dis.x, dis.y));
                    // 取得站點資料
                    ubikeAPI.GetUbikeStationInformation(dis.x, dis.y);
                    // 取得路線
                    StartCoroutine(googleMapAPIControl.GetRequestDirectionsData(dis.x, dis.y));
                    Debug.Log(hit);
                }
            }
        }
    }

    // API 傳入 ubike 站點的資料
    public void GetNearUbikeData(string ubikeDataText)
    {
        // 轉換並儲存地點資料
        JObject data = JObject.Parse(ubikeDataText);
        ubikeData = JArray.Parse(data["results"].ToString());
        //int len = ubikeData.Count;

        string text = "附近有: ";
        foreach (JToken item in ubikeData)    // deal with every item
        {
            //Debug.Log(item["name"].ToString() + "(lat, lng)" + item["geometry"]["location"]["lat"].ToString() + ", " + item["geometry"]["location"]["lng"].ToString());
            string name = item["name"].ToString();
            float lat = float.Parse(item["geometry"]["location"]["lat"].ToString());
            float lon = float.Parse(item["geometry"]["location"]["lng"].ToString());
        }
        UbikeText.text = text + ubikeData.Count + " 個站點";

        // 生成地標物件
        CreatLocationMark();

        //StartCoroutine(googleMapAPIControl.GetLocationDistance(23.5550651f, 120.4715430f));
    }

    private void CreatLocationMark()
    {// 建立地標物件
        // 更新相機羅盤方向
        arCamera.rotation = Quaternion.Euler(0, Input.compass.magneticHeading, 0);

        foreach (JToken item in ubikeData)    // deal with every item
        {
            //Debug.Log(item["name"].ToString() + "(lat, lng)" + item["geometry"]["location"]["lat"].ToString() + ", " + item["geometry"]["location"]["lng"].ToString());
            string name = item["name"].ToString();
            float lat = float.Parse(item["geometry"]["location"]["lat"].ToString());
            float lon = float.Parse(item["geometry"]["location"]["lng"].ToString());

            Vector3 pos = new Vector3
            {
                z = lat - googleMapAPIControl.lat,
                x = lon - googleMapAPIControl.lon
                //z = lat - Input.location.lastData.latitude,
                //x = lon - Input.location.lastData.longitude
            };
            locationMark = Instantiate(locationMarkIcon, pos * 12000, new Quaternion());
            locationMark.GetComponent<LocationMarkController>().SetText(name);
            locationMark.GetComponent<LocationMarkController>().SetLatitudeAndLongitude(lat, lon);
        }
    }

    // API 傳距離進來
    public void GetDistance(string text)
    {
        JObject data = JObject.Parse(text);
        UbikeText.text += "\n" + data["rows"][0]["elements"][0]["distance"]["text"] + "\n";
    }

    // API 傳導航進來
    public void GetDirection(string text)
    {
        JObject data = JObject.Parse(text);
        //Debug.Log("時間: " + data["routes"][0]["legs"][0]["duration"]);
        string directionWay = "";
        foreach (var step in data["routes"][0]["legs"][0]["steps"])
        {
            directionWay += step["html_instructions"] + "\n";
        }
        UbikeText.text += KeepChinese(directionWay);
        Debug.Log(directionWay);
    }
    // API 傳站點資訊
    public void GetInformation(string text)
    {
        UbikeText.text += text;
    }

    private string KeepChinese(string str)
    {
        string chineseString = "";

        for (int i = 0; i < str.Length; i++)
        {
            if ((str[i] >= 0x4E00 && str[i] <= 0x9FA5) || str[i] == '\n') //中文
            {
                chineseString += str[i];
            }
        }

        return chineseString;
    }


}
