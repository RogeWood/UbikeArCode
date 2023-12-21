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
    [SerializeField] private GPSLocation gps; // 經緯度資料

    private float lat;
    private float lon;

    private void Start()
    {
        Debug.Log("run api");
        // 步驟1：取得 Access Token
        string clientId = "YOUR_CLIENT_ID";
        string clientSecret = "YOUR_CLIENT_SECRET";

        // 步驟2：呼叫 TDX API 服務，取得數據資料
        StartCoroutine(GetAccessToken(clientId, clientSecret));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("UbikeAPI!!");
        // gps.GetLatAndLon() 取得經緯度。return type: Vector2 x是lat, y是lon
        lat = gps.GetLatAndLon().x;
        lon = gps.GetLatAndLon().y;
    }

    IEnumerator GetUbikeAPI(string uri)
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
                ParseBikeStationData(response);
            }
            else
            {
                Debug.LogError($"Error fetching bike station data: {webRequest.result} - {webRequest.error}");
                Debug.LogError($"Error response: {webRequest.downloadHandler.text}");
            }
        }
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
                StartCoroutine(GetUbikeAPI("https://tdx.transportdata.tw/api/basic/v2/Bike/Station/City/Chiayi?$format=JSON"));
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

    private void ParseBikeStationData(string jsonData)
    {
        // 在這裡進行 JSON 資料的解析
        // 你可以使用 JsonUtility 或其他 JSON 解析工具，取決於資料的複雜程度

        // 這是一個簡單的例子，如果 JSON 資料的根層是一個物件，可以使用 JsonUtility
        // BikeStationData stationData = JsonUtility.FromJson<BikeStationData>(jsonData);

        // 現在，你可以使用 stationData 物件中的屬性進行操作
        // Debug.Log($"Station Name: {stationData.nameZh}");
        // Debug.Log($"Available Rent Bikes: {stationData.availableRentBikes}");

        // string jsonData = "你的JSON字串"; // 將你的 JSON 字串指定到這裡

        // 使用 JsonUtility.FromJson 將 JSON 字串轉換為 BikeStationList 物件
        //BikeStationList[] stationArray = JsonUtility.FromJson<BikeStationData[]>(jsonData);

        // BikeStationData[] stationDataArray = JsonUtility.FromJson<BikeStationData[]>(jsonData);
        //List<BikeStationData> stationDataArray = JsonUtility.FromJson<List<BikeStationData>>(jsonData);
        JArray stationDataArray = JArray.Parse(jsonData);
        Debug.Log(stationDataArray);
        //if (stationDataArray != null)
        //{
        //    // 現在，你可以使用 stationDataArray 中的第一個元素（或根據實際情況選擇元素）進行操作
        //    BikeStationData stationData = stationDataArray[0];
        //    Debug.Log($"Station UID: {stationData.StationUID}");
        //    Debug.Log($"name: {stationData.StationName.Zh_tw}");
        //}
        //else
        //{
        //    Debug.LogError("No bike station data found.");
        //}

        // 現在，你可以訪問每個站點
        //foreach (BikeStationData stationData in stationDataArray.stations)
        //{
        //Debug.Log($"Station Name (Chinese): {stationData.StationName.Zh_tw}");
        //Debug.Log($"Station Name (English): {stationData.StationName.En}");
        //Debug.Log($"Position Lon: {stationData.StationPosition.PositionLon}");
        //Debug.Log($"Position Lat: {stationData.StationPosition.PositionLat}");
        // 以此類推，你可以訪問其他屬性
        //}
    }

    // 定義 BikeStationData 類別以匹配 JSON 資料的結構
   // public class BikeStationList
   // {
   //     public List<BikeStationData> stations;
   // }

   // public class BikeStationData
   //{
   //      public string StationUID;
   //     public string StationID;
   //     public string AuthorityID;
   //     public StationNameData StationName;
   //     public StationPositionData StationPosition;
   //     public StationAddressData StationAddress;
   //     public int BikesCapacity;
   //     public int ServiceType;
   //     public string SrcUpdateTime;
   //     public string UpdateTime;
   // }

   // public class StationNameData
   // {
   //     public string Zh_tw;
   //     public string En;
   // }

   // public class StationPositionData
   // {
   //     public float PositionLon;
   //     public float PositionLat;
   //     public string GeoHash;
   // }

   // public class StationAddressData
   // {
   //     public string Zh_tw;
   //     public string En;
   // }
}
