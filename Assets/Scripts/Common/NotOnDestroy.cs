using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;
using core.data;
//using Unity.Services.Core;
//using Unity.Services.Core.Environments;
//using UnityEngine.Purchasing;



/// <summary>
/// The most of the 
/// </summary>
public class NotOnDestroy : MonoBehaviour
{
    public bool shouldLobbyUITransact = true;
    //FittingItemDataList outfitShopItemList;
    //ShopItemDataList shopItemList;
    //QuestDataList questDataList;
    //UserFriendInfos friendList;
    

#if DEV
    public string environment = "development";
#else
    public string environment = "production";
#endif


    UserInfo curUserInfo;
    public UnityEvent infoUpdated = new();

    //EzyApp curApp;

    


    // Use this for initialization
    void Start()
    {
        //this.gameObject.name = "loginSession";
        DontDestroyOnLoad(this);
        LoadAllShopItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void LogoutDestroy() {
        //if (auth.CurrentUser != null) {
        //    auth.SignOut();
        //}
        // clean all afterlogin cached list content
        //outfitShopItemList = null;
        //shopItemList = null;
        //questDataList = null;
        //friendList = null;
        userItem = null;

        CommonConfig.LogoutClean();
        CommonConfig.ResetAllPrefs();
        //GameObject.DestroyImmediate(this.gameObject);
    }

    /// =-=-=-=-=-=-=-=  FireBase config =-=-=-=-=-=-=-=
    //public Firebase.FirebaseApp app = null;
    //public FirebaseAuth auth = null;
    //public PhoneAuthProvider phoneAuthprovider = null;
    //public FirebaseUser user;
    public string firebaseUserJWT = "";
    public void InitFireBase(Action<int> n_callback)
    {
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        //    var dependencyStatus = task.Result;
        //    if (dependencyStatus == Firebase.DependencyStatus.Available )
        //    {
        //        // Create and hold a reference to your FirebaseApp,
        //        // where app is a Firebase.FirebaseApp property of your application class.
        //        if (app == null)
        //        {
        //            app = Firebase.FirebaseApp.DefaultInstance;
        //            // Set a flag here to indicate whether Firebase is ready to use by your app.
        //            if (app != null)
        //            {
        //                Debug.Log("FirebaseApp Available");
        //                auth = FirebaseAuth.DefaultInstance;

        //                if (auth != null)
        //                {
        //                    Debug.Log("FirebaseAuth Available");
        //                    n_callback?.Invoke(0);
        //                }
        //                else
        //                {
        //                    Debug.Log("FirebaseAuth have issue to config");
        //                    n_callback?.Invoke(-1);
        //                }
        //            }
        //        }
        //        else {
        //            //already get instance .. call instead for other views to call
        //            n_callback?.Invoke(0);
        //        }
        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError(System.String.Format(
        //          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //        // Firebase Unity SDK is not safe to use here.

        //        n_callback?.Invoke(-2);
        //    }
        //});
    }

    //public PhoneAuthProvider GetPhoneAuth()
    //{
    //    if (auth != null && phoneAuthprovider == null) {
    //        phoneAuthprovider = PhoneAuthProvider.GetInstance(auth);
    //    }

    //    return phoneAuthprovider;
    //}

    long lastJWTTimeStamp = -1;
    public void UpdateJWTToken() {
        bool needForceUpdate = false;
        long currentMillis = CommonConfig.GetCurrentDateTimeSince1970inMS();
        if (lastJWTTimeStamp == -1 || currentMillis >= lastJWTTimeStamp + (1800 * 1000))// 1hr
        {
            lastJWTTimeStamp = CommonConfig.GetCurrentDateTimeSince1970inMS();
            needForceUpdate = true;
        }

        //if (user == null) {
        //    user = auth.CurrentUser;
        //}

        //if (user != null)
        //{
        //    user.TokenAsync(needForceUpdate).ContinueWithOnMainThread(task_JWT =>
        //    {
        //        Debug.Log(needForceUpdate ? "JWT updated" : "JWT get");
        //        {
        //            firebaseUserJWT = task_JWT.Result;
        //        }
        //    });
        //}
        //else {
        //    Debug.Log("UpdateJWTToken user null");
        //}
        
        
    }

    /// =-=-=-=-=-=-=-= Server service  =-=-=-=-=-=-=-=

    public ServerService serverService = null; // inventory
    public void UpdateServerStatus(string dataString) {
        if (!string.IsNullOrEmpty(dataString)) {
            serverService = ServerService.NewFromJsonText(dataString);
            Debug.Log($"{dataString}");
        }
    }

    /// =-=-=-=-=-=-=-= local userInfo  =-=-=-=-=-=-=-=

    public UserInfo Info => curUserInfo;
    public void UpdateInfo(UserInfo value) {

        if (value != null) {
            if (curUserInfo != null && curUserInfo.level != value.level)
            {
                isLevelUpped = true;
            }

            curUserInfo = value;

            infoUpdated.Invoke();
        }
    }
    bool isLevelUpped = false;
    public bool GetIsLevelUpped() {
        if (isLevelUpped) {
            isLevelUpped = false;
            return true;
        }
        return false;
    }

    public BackPackItemDataList userItem = null; // inventory
    public void UpdateBackpack(string dataString)
    {
        userItem = BackPackItemDataList.NewFromJsonText(dataString);
    }
    public int GetItemsCount(bool countWithQty = false) {
        int cnt = 0;

        if (userItem != null) {
            foreach (var itemData in userItem.itemDatas) {
                if (itemData.quantity > 0) {
                    if (countWithQty)
                    {
                        cnt += itemData.quantity;
                    }
                    else {
                        cnt++;
                    }
                }
            }
        }

        return cnt;
    }

    public void UpdateFriendList(string jsonDataText)
    {
        if (!string.IsNullOrEmpty(jsonDataText))
        {
            Debug.Log($"UpdateFriendList : {jsonDataText}");
            //UserFriendInfos n_dataList = UserFriendInfos.NewFromJsonText(jsonDataText);
            //if (n_dataList != null)
            //{
            //    friendList = n_dataList;
            //}
        }
    }

    public UserFriendInfos GetFriendList()
    {
        //return friendList;
        return null;
    }

    // =-=-=-=-=-=-=-=- All Quests  (Global Quest) -=-=-=-=-=-=-=-=


    public void UpdateFullQuestList(string jsonDataText , bool isAddOn = false)
    {
        if (!string.IsNullOrEmpty(jsonDataText))
        {
            Debug.Log($"Update {(isAddOn? "Event" : "") } FullQuestList : {jsonDataText}");
            QuestDataList n_dataList = QuestDataList.NewFromJsonText(jsonDataText);
            if (n_dataList != null)
            {
                //if (questDataList != null)
                //{ // handle extra quest
                //    questDataList.quests.AddRange(n_dataList.quests);
                //}
                //else { // the base
                //    questDataList = n_dataList;
                //}
            }
        }
    }

    public QuestDataList GetFullQuestList()
    {
        //if (questDataList == null)
        //{
        //    //LoadAllShopItems();
        //}

        //return questDataList;
        return null;
    }


    

    // =-=-=-=-=-=-=-=- All Shop items (Item and Wears) -=-=-=-=-=-=-=-=

    async void LoadAllShopItems()
    {
        // Load the text asset from Resources
        //TextAsset textAsset = Resources.Load<TextAsset>("csv/fitting_items");
        //if (textAsset != null)
        //{
        //    // Parse the JSON data from the text asset
        //    FittingItemDataList n_fittingItemDataList = FittingItemDataList.NewFromJsonText(textAsset.text);
        //    if (n_fittingItemDataList != null)
        //    {
        //        fittingItemList = n_fittingItemDataList;
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Failed to load fitting_items JSON file.");
        //}


        //// Load the text asset from Resources
        //textAsset = Resources.Load<TextAsset>("csv/shop_Items");
        //if (textAsset != null)
        //{
        //    // Parse the JSON data from the text asset
        //    UpdateFullShopItemList(textAsset.text);
        //}

        //StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        //StandardPurchasingModule.Instance().useFakeStoreAlways = false;
        //try
        //{
        //    var options = new InitializationOptions().SetEnvironmentName(environment);

        //    await UnityServices.InitializeAsync(options).ContinueWithOnMainThread(task=> {
        //        if (task.IsCompleted)
        //        {
        //            Debug.Log($" IAP services initialization finished ");
        //        }
        //        else if (task.IsFaulted) {
        //            Debug.Log($" IAP services initialization faulted ");
        //        }
        //    });
            
        //}
        //catch (Exception exception)
        //{
        //    Debug.LogWarning($"An error occurred during IAP services initialization. {exception}");
        //}
    }

    public void UpdateFullShopItemList(string jsonDataText)
    {
        if (!string.IsNullOrEmpty(jsonDataText))
        {
            ShopItemDataList n_itemDataList = ShopItemDataList.NewFromJsonText(jsonDataText);
            if (n_itemDataList != null)
            {
                //shopItemList = n_itemDataList;
            }
        }
    }

    public ShopItemDataList GetFullShopItemList() {
        //if (shopItemList == null)
        //{
        //    //LoadAllShopItems();
        //}

        //return shopItemList;
        return null;
    }

    public ShopItemData GetShopItem(string id)
    {

        //if (shopItemList == null)
        //{
        //    LoadAllShopItems();
        //}

        ShopItemData n_out = null;

        //if (!id.Contains("iap_")) {
        //    id = $"iap_{CommonFunction.GetPadNumberFormat(long.Parse(id))}";
        //}

        //foreach (ShopItemData item in shopItemList.items)
        //{
        //    if (item.itemIapId == id)
        //    {
        //        n_out = item;
        //        break;
        //    }
        //}

        return n_out;
    }

    public void UpdateFullOutfitShopItemList(string jsonDataText)
    {
        if (!string.IsNullOrEmpty(jsonDataText))
        {
            FittingItemDataList n_itemDataList = FittingItemDataList.NewFromJsonText(jsonDataText);
            if (n_itemDataList != null)
            {
                //outfitShopItemList = n_itemDataList;
            }
        }
    }

    public AvaterItemData[] GetFittingItems(int type_id , int gender)
    {
        List<AvaterItemData> n_out = new();
        //foreach (AvaterItemData item in outfitShopItemList.items)
        //{
        //    if (item.type == type_id && (item.gender == 2 || item.gender == gender))
        //    {
        //        n_out.Add(item);
        //    }
        //}

        return n_out.ToArray();
    }

    public AvaterItemData GetFittingItem(int id)
    {
        AvaterItemData n_out = null;
        //foreach (AvaterItemData item in outfitShopItemList.items)
        //{
        //    if (item.id == id)
        //    {
        //        n_out = item;
        //    }
        //}

        return n_out;
    }


    // =-=-=-=-=-=-=-=- Share Game App information -=-=-=-=-=-=-=-=

    //public void SetGameRoomApp(EzyApp n_app) {
    //    curApp = n_app;
    //}

    //public EzyApp GetCurGameRoomApp() {
    //    return curApp;
    //}

    bool IsRoomLeft = false;
    public void SetLeaveRoomLobbyHandling() {
        IsRoomLeft = true;
    }

    public void RemoveRoomTCPComponent()
    {
        if (IsRoomLeft) {
            IsRoomLeft = false;
            //var _tcpHandler = GetComponent<EzyTCPHandler>();
            //// do remove existing TCP
            //if (_tcpHandler != null)
            //{
            //    Destroy(_tcpHandler);
            //}
            appIndex = -1; // reset
        }
    }

    string[] players = new string[]{ };
    int location = -1;
    int roomId = -1;
    int appIndex = -1;
    int roomLevelType = -1;

    /// =-=-=-=-=-=-=-=- Scene and folder path (local)  -=-=-=-=-=-=-=-= 

    public string GetSceneName() {
        string out_name = "";
        switch (appIndex) {
            ///  RoomID : 0-99 -> casual
            ///         : 100-199 -> hundred
            case 0: out_name = "MJHRGameRoom"; break;
            case 1: out_name = "PKBTGameRoom"; break;
            case 2: out_name = "PKGameRoom"; break;
            case 3: out_name = "PKBJGameRoom"; break;
            case 4: out_name = "PKDGameRoom"; break;

            case 100: out_name = "PKSGGameRoom"; break;
            case 101: out_name = "PKCGameRoom"; break;
        }

        if (!CommonConfig.IsAssetBundled() && GetGameAssetBundleName().Length > 0)
        {
            out_name += "_nab";
        }
        else {
            out_name = "BundledInGameRoomScene"; // all bundled inGame generally use this scene
        }

        return out_name;
    }

    public string GetGameName() {
        string out_name = "";
        switch (appIndex)
        {
            ///  RoomID : 0-99 -> casual
            ///         : 100-199 -> hundred
            case 0: out_name = "GameList_game_jokey"; break;
            case 1: out_name = "GameList_game_big2"; break;
            case 2: out_name = "GameList_game_taxes"; break;
            case 3: out_name = "GameList_game_blackjack"; break;
            case 4: out_name = "GameList_game_doudizhu"; break;

            case 100: out_name = "GameList_game_soccer"; break;
            case 101: out_name = "GameList_game_cowcow"; break;
        }

        return CommonConfig.GetLangWithKey(out_name);
    }

    public string GetGameFolderName()
    {
        string out_name = "";
        switch (appIndex)
        {
            ///  RoomID : 0-99 -> casual
            ///         : 100-199 -> hundred
            case 0: out_name = "MJ_HR"; break;
            case 1: out_name = "PK_BT"; break;
            case 2: out_name = "PK_TX"; break;
            case 3: out_name = "PK_BJ"; break;
            case 4: out_name = "PK_DDZ"; break;

            case 100: out_name = "PK_SG"; break;
            case 101: out_name = "PK_CC"; break;
        }

        return out_name;
    }

    /// =-=-=-=-=-=-=-=- bundle -=-=-=-=-=-=-=-= 

    public string GetGameAssetBundleName()
    {
        string out_name = "";
        switch (appIndex)
        {
            ///  RoomID : 0-99 -> casual
            ///         : 100-199 -> hundred
            case 0: out_name = "jackey"; break;
            case 1: out_name = "bigtwo"; break;
            case 2: out_name = "texas"; break;
            case 3: out_name = "blackjack"; break;
            case 4: out_name = "doudizhu"; break;

            case 100: out_name = "soccer"; break;
            case 101: out_name = "cowcow"; break;
        }
        return out_name;
    }

    public string GetGameSoundAssetBundleName()
    {
        string out_name = "";
        switch (appIndex)
        {
            ///  RoomID : 0-99 -> casual
            ///         : 100-199 -> hundred
            case 0: out_name = "jackey"; break;
            case 1: out_name = "bigtwo"; break;
            case 2: out_name = "texas"; break;
            case 3: out_name = "blackjack"; break;
            case 4: out_name = "doudizhu"; break;

            case 100: out_name = "soccer"; break;
            case 101: out_name = "cowcow"; break;
        }
        return out_name;
    }

    /// =-=-=-=-=-=-=-=- TCP connection -=-=-=-=-=-=-=-= 

    public int GetBanGameServerID()
    {
        int out_id = 0;
        switch (appIndex)
        {
            ///  RoomID : 0-99 -> casual
            ///         : 100-199 -> hundred
            case 0: out_id = 6; break;
            case 1: out_id = 2; break;
            case 2: out_id = 5; break;
            case 3: out_id = 1; break;
            case 4: out_id = 4; break;

            case 100: out_id = 7; break;
            case 101: out_id = 3; break;
        }
        return out_id;
    }


    bool isPrivateRoom = false;
    int joinRoomCode = -1;
    public void SetToGameConnectionInfo(int n_appIndex = -1 , bool isPrivate = false , int roomPassCode = -1) {
        appIndex = n_appIndex;
        isPrivateRoom = isPrivate;
        joinRoomCode = roomPassCode;

        //var _tcpHandler = GetComponent<EzyTCPHandler>();
        //// do remove existing TCP
        //if (_tcpHandler != null) {
        //    Destroy(_tcpHandler); 
        //}

        //switch (appIndex) {
        //    ///  RoomID : 0-99 -> casual
        //    ///         : 100-199 -> hundred

        //    case 0: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_MJHR>(); break;
        //    case 1: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_PKBT>(); break;
        //    case 2: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_PK>(); break;
        //    case 3: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_PKBJ>(); break;
        //    case 4: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_PKD>(); break;

        //    case 100: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_PKSG>(); break;
        //    case 101: _tcpHandler = gameObject.AddComponent<EzyTCPHandler_PKC>(); break;
        //}

        //if (_tcpHandler != null) {
        //    Debug.Log($"EzyTCPHandler ({appIndex}) added");
        //}
    }
    public void ChangeRoomPortByLevel(int n_roomLevelType) {
        //var _tcpHandler = GetComponent<EzyTCPHandler>();
        //if (_tcpHandler != null)
        //{
        //    _tcpHandler.ChangeSubport(n_roomLevelType * 1000);
        //    roomLevelType = n_roomLevelType;
        //}
    }

    public bool GetIsPrivateRoom() => isPrivateRoom;
    public int GetPrivateRoomPassCode() => joinRoomCode;

    public void SetGameRoomSeatInInfo(string[] n_players, int n_location, int n_roomId)
    {
        players = n_players;
        location = n_location;
        roomId = n_roomId;
    }

    public void UpdateSelfGameRoomSeat(int n_location)
    {
        location = n_location;
    }

    public int GetOnStartSeatNo => location;
    public int GetOnStartRoomNo => roomId;
    public int GetOnStartRoomLevel => roomLevelType;
    public string[] GetOnStartPlayers => players;

    public void MakeTCPActionCommand(string tCPCommands , Dictionary<string,dynamic> para) {
        //EzyObjectBuilder eob = EzyEntityFactory.newObjectBuilder();
        //eob.append("action", tCPCommands);
        //foreach (string key in para.Keys) {
        //    if (para.TryGetValue(key, out object value)) {
        //        eob.append(key, value); // add data row
        //    }
        //}
        //var dataBody = eob.build();
        //GetCurGameRoomApp().send(TCPCommands.ACTION_MAKE, dataBody);
    }

    /// <summary>
    /// For the non turn action command. e.g. : standup , sitdown
    /// </summary>
    /// <param name="tCPCommands"></param>
    /// <param name="para"></param>
    public void MakeTCPOtherActionCommand(string tCPCommands, Dictionary<string, dynamic> para)
    {
        //EzyObjectBuilder eob = EzyEntityFactory.newObjectBuilder();
        foreach (string key in para.Keys)
        {
            if (para.TryGetValue(key, out object value))
            {
                //eob.append(key, value); // add data row
            }
        }
        //var dataBody = eob.build();
        //GetCurGameRoomApp().send(tCPCommands, dataBody);
    }

    string prev_kicked_out_app = "";
    public void SetAutoKickoutMessage(string appName) {
        prev_kicked_out_app = appName;
        SetLeaveRoomLobbyHandling();
    }
    public string GetKickedOutAppName() {
        string out_val = prev_kicked_out_app;
        prev_kicked_out_app = ""; /// clear

        return out_val;
    }

    string prev_leave_gameFinished = "";
    public void SetFinishAndLeaveGame(string appName) {
        prev_leave_gameFinished = appName;
    }
    public string GetFinishAndLeaveGameName() {
        string out_val = prev_leave_gameFinished;
        prev_leave_gameFinished = ""; /// clear

        return out_val;
    }

    // =-=-=-=-=-=-=-=-  resources preload -=-=-=-=-=-=-=-=
    public void MemoryAutoUnload() {
        Resources.UnloadUnusedAssets();
    }
}


