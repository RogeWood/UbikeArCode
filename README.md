# UbikeAR
AR 虛擬實境 Ubike 站點導航

code 位置: `UbikeArCode\Assets\Scripts`

## 開啟檔案

使用 Unity Hub -> 新增 -> 從硬碟新增專案 -> 選取資料夾

點擊專案即可開啟

### token

**Google map API**

到 Google map API 網站上申請後，將 key 放到以下變數

`UbikeArCode\Assets\Scripts\ARCamera\GoogleMapAPIControl.cs`: `private string apiKey = "YOUR_API_KEY"`



**YouBike API**

到交通部申請後，將 key 放到以下變數

`UbikeArCode\Assets\Scripts\UbikeAPI\UbikeAPI.cs` : 

`string clientId = "YOUR_CLIENT_ID"`

`string clientSecret = "YOUR_CLIENT_SECRET"`
