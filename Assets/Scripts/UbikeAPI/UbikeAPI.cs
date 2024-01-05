using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class UbikeAPI : MonoBehaviour
{
    private AccessToken token;
    private JArray stationDataArray;
    private JArray stationBikeInfo;
    // private JArray Database;
    [SerializeField] private GPSLocation gps; // 經緯度資料
    [SerializeField] private CameraSenceController senceController;

    private string nearestStationUID; // 最近站點的UID
    private string stationName; // 最近站點的中文名

    private float lat;
    private float lon;
    private float currentLat;
    private float currentLon;
    [SerializeField] private string clientId = "YOUR_CLIENT_ID";
    [SerializeField] private string clientSecret = "YOUR_CLIENT_SECRET";

    private void Start()
    {
        Debug.Log("run api");
        // 步驟1：取得 Access Token

        // 步驟2：呼叫 TDX API 服務，取得數據資料
        //StartCoroutine(GetAccessToken(clientId, clientSecret));
        

    }

    public void GetUbikeStationInformation(float la, float ln)
    {
        currentLat = la;
        currentLon = ln;
        StartCoroutine(GetAccessToken(clientId, clientSecret));
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("UbikeAPI!!");
        //gps.GetLatAndLon(); // 取得經緯度。return type: Vector2 x是lat, y是lon
        //lat = gps.GetLatAndLon().x;
        //lon = gps.GetLatAndLon().y;

        //StartCoroutine(GetUbikeAPI("https://tdx.transportdata.tw/api/basic/v2/Bike/Availability/City/Chiayi?$format=JSON", 1));
    }

    private IEnumerator GetAccessToken(string clientId, string clientSecret)
    {
        // API 端點
        string apiUrl = "https://tdx.transportdata.tw/auth/realms/TDXConnect/protocol/openid-connect/token";

        // 準備 POST 請求的表單數據
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "client_credentials");
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);

        // 使用 UnityWebRequest 發送 POST 請求
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            // 設定請求標頭
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            // 發送請求，等待回應
            yield return www.SendWebRequest();

            // 確認是否有錯誤
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Get token: \n" + www.downloadHandler.text);
                // 讀取回應內容
                string responseStr = www.downloadHandler.text;

                // 解析 JSON 格式的回應
                token = JsonUtility.FromJson<AccessToken>(responseStr);

                // 步驟2：呼叫 TDX API 服務，取得數據資料
                StartCoroutine(GetUbikeAPI("https://tdx.transportdata.tw/api/basic/v2/Bike/Station/City/Chiayi?$format=JSON", 0));
                StartCoroutine(GetUbikeAPI("https://tdx.transportdata.tw/api/basic/v2/Bike/Availability/City/Chiayi?$format=JSON", 1));
            }
            else
            {
                Debug.LogError($"Error getting Access Token: {www.result} - {www.error}");
                Debug.LogError($"Error response: {www.downloadHandler.text}");
            }
        }
    }
    public class AccessToken
    {
        public string access_token;
        public int expires_in;
        public int refresh_expires_in;
        public string token_type;
        public int notbeforepolicy;
        public string scope;
    }

    IEnumerator GetUbikeAPI(string uri, int curInfo)
    {
        if (token == null || string.IsNullOrEmpty(token.access_token))
        {
            Debug.LogError("Access Token is not available.");
            yield break;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Authorization", $"Bearer {token.access_token}");
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 讀取回應內容
                string response = webRequest.downloadHandler.text;
                Debug.Log("ubike api data load success\n" + response);

                // 解析 JSON 資料
                if (curInfo == 0)
                {
                    ParseBikeStationData(response);
                }
                else if (curInfo == 1)
                {
                    GetCurInfo(response);      // update current info
                }
            }
            else
            {
                Debug.LogError($"Error fetching bike station data: {webRequest.result} - {webRequest.error}");
                Debug.LogError($"Error response: {webRequest.downloadHandler.text}");
            }
        }
    }

    private void ParseBikeStationData(string jsonData)
    {
        stationDataArray = JArray.Parse(jsonData);
        Debug.Log(stationDataArray);

        float stationLat, stationLon;
        foreach (var item in stationDataArray)
        {
            stationLat = float.Parse(item["StationPosition"]["PositionLat"].ToString());
            stationLon = float.Parse(item["StationPosition"]["PositionLon"].ToString());

            //Debug.Log("Lat :" + stationLat);
            //Debug.Log("Lon :" + stationLon);
        }
        CaculatenearestUID();
    }


    private void GetCurInfo(string jsonData)
    {
        stationBikeInfo = JArray.Parse(jsonData);
        Debug.Log(stationBikeInfo);

        float AvailableRentBikes, AvailableReturnBikes;
        foreach (var item in stationBikeInfo)
        {
            AvailableRentBikes = float.Parse(item["AvailableRentBikes"].ToString());
            AvailableReturnBikes = float.Parse(item["AvailableReturnBikes"].ToString());

            //Debug.Log("Rent :" + AvailableRentBikes);
            //Debug.Log("Return :" + AvailableReturnBikes);
        }
        
    }

    private void CaculatenearestUID()
    {
        // 迭代 YouBike 站點，計算站點
        float sum = float.MaxValue;
        float compare;

        foreach (var item in stationDataArray)
        {
            float stationLat = float.Parse(item["StationPosition"]["PositionLat"].ToString());
            float stationLon = float.Parse(item["StationPosition"]["PositionLon"].ToString());

            // 計算距離
            compare = Mathf.Abs(stationLat - currentLat) + Mathf.Abs(stationLon - currentLon);

            // 更新最短距離的站點
            if (compare < sum)
            {
                sum = compare;
                nearestStationUID = item["StationUID"].ToString();
                stationName = item["StationName"]["Zh_tw"].ToString();
            }
        }
        Debug.Log("nearestStationUID :" + nearestStationUID);
        DataProcess();
    }
    private void DataProcess()
    {
        float rentBikes = 0.0f, returnBikes = 0.0f;
        if(stationBikeInfo == null)
        {
            GetUbikeStationInformation(currentLat, currentLon);
            return;
        }

        foreach (var item in stationBikeInfo)
        {
            string stationUID = item["StationUID"].ToString();
            if (nearestStationUID == stationUID)
            {
                rentBikes = float.Parse(item["AvailableRentBikes"].ToString());
                returnBikes = float.Parse(item["AvailableReturnBikes"].ToString());
            }
        }
        Debug.Log("StationName :" + stationName);
        Debug.Log("AvailableRentBikes :" + rentBikes);
        Debug.Log("AvailableReturnBikes :" + returnBikes);

        string ubikeText = "站點名稱 :" + stationName + "\n剩下腳踏車數量 :" + rentBikes + "\n剩下空位 :" + returnBikes + "\n";
        senceController.GetInformation(ubikeText);
    }
}
