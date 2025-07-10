using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace core.data
{
    [Serializable]
    public class UserInfo
    {
        public string id = "";
        public string uid = ""; /// user's unique device identifier , or firebase uid "ymIYT2PkhLbw3ADHytaYtNGJki23",
        public string nickname = "";
        public string firstName = "";
        public string lastName = "";
        public string title = "";
        public string region = "";
        public string invite_code = "";
        public string email = "";
        public string mobileNo = "";
        public string createdAt = "";
        public string updatedAt = "";
        public int gender = 0; // M = 0  , F = 1
        public int genderIrl = 0; // avatar only :  M = 0  , F = 1 
        public bool guest;

        // out look 
        public string thumbNailPicUri = ""; //"https://adhocbuildbuck.s3.eu-west-2.amazonaws.com/DevGameBundles/img-avatar-thumbnail-sample.jpeg";
        public int thumbnailRingId = 1; // default 1, black frame
        public int avatarHair = 0;
        public int avatarFace = 0;
        public int avatarUpper = 0;
        public int avatarLower = 0;
        public int avatarItem = 0;
        public int avatarVibe = 0;
        public int avatarShoes = 0;

        /// login log
        public int continuousLoginDay = 0;
        public int totalLoginDay = 0;
        public string lastLogin = ""; // "2024-09-24T10:05:26.089482"


        public int level = 0;
        public int exp = 0;
        public long coin = 0;
        public long diamond = 0;
        public int point = 0;
        public long vaultCoin = 0;
        public long vaultDiamond = 0;
        public bool vip_sub = false;
        public string acc_link = ""; /// should be contant the social media account ID 
        //public ActiveBuffList userStatus = null;


        public int backpackCap = 10;
        public string banTime = null;
        public int wearCap = 50; // 10 for each
        public List<int> wearingIDs = new(); // avatar

        public static UserInfo NewFromJsonText(string jsonContentText = "") {

            UserInfo n_obj = null;
            if (jsonContentText.Length > 0)
            {
                try
                {
                    n_obj = JsonUtility.FromJson<UserInfo>(jsonContentText);
                    if (n_obj.level <= 1 && n_obj.exp == 0) {
                        // set a default wear as new user
                        n_obj.wearingIDs = new(CommonConfig.fixedEquipment);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Json prase error - reason : " + e);
                }
            }
            else {
                Debug.Log("Json text is empty , give out a new object");
                n_obj = new();
                n_obj.wearingIDs = new(CommonConfig.fixedEquipment);
            }

            return n_obj;
        }


        public string GetJsonText()
        {
            string json = JsonUtility.ToJson(this);
            // json now contains: '{"level":1,"timeElapsed":47.5,"playerName":"Dr Charles Francis"}'
            return json;
        }

        //// =-=-=-=-=-=-= other handy function  =-=-=-=-=-=-= 

        public string GetUUID() {
            if (uid.Length == 0) {
                // return device UUID in default
                return SystemInfo.deviceUniqueIdentifier;
            }

            return uid;
        }

        public string GetName() {
            if (nickname.Length == 0 || nickname == "guest") {
                return "GuestPlayer" + GetUUID().Split("-")[0];
            }
            return nickname;
        }


        public List<int> GetEquipingWearList(){

            return new(new int[]{
                avatarHair,
                avatarFace,
                avatarUpper,
                avatarLower,
                avatarShoes,
                avatarItem,
                avatarVibe });
        }

        public List<int> GetServerMappingWearList()
        {
            return new(new int[]{
                avatarHair,
                avatarFace,
                avatarUpper,
                avatarLower,
                avatarItem,
                avatarVibe,
                avatarShoes,});
        }

        public string GetBanTimeLeft() {

            if (!string.IsNullOrEmpty(banTime))
            {
                if (DateTime.TryParse(banTime, out DateTime dateTime))
                {
                    DateTime expire_dateTime = dateTime;

                    long diff = (long)(expire_dateTime - DateTime.UtcNow).TotalMilliseconds;
                    //long diff = (long)(expire_dateTime - DateTime.UtcNow.AddHours(CommonConfig.TIMEZONE_HK)).TotalMilliseconds;


                    if (diff > 0)
                    {
                        var diffDT = expire_dateTime - DateTime.UtcNow;
                        //var diffDT = expire_dateTime - DateTime.UtcNow.AddHours(CommonConfig.TIMEZONE_HK);
                        return $"{ CommonFunction.GetDigitString(diffDT.Days) }d {CommonFunction.GetDigitString(diffDT.Hours) }:{CommonFunction.GetDigitString(diffDT.Minutes) }:{CommonFunction.GetDigitString(diffDT.Seconds) }";
                    }
                }
            }

            return "00:00:00";
        }

    }

    [Serializable]
    public class ServerService {
        public StatusSet[] statusSets;

        public static ServerService NewFromJsonText(string jsonContentText = "")
        {

            ServerService n_obj = null;
            if (jsonContentText.Length > 0)
            {
                try
                {
                    if (jsonContentText.StartsWith("["))
                    {
                        jsonContentText = "{\"statusSets\":" + jsonContentText + "}";
                    }

                    n_obj = JsonUtility.FromJson<ServerService>(jsonContentText);
                }
                catch (Exception e)
                {
                    Debug.Log("Json prase error - reason : " + e);
                }
            }
            else
            {
                Debug.Log("Json text is empty , give out a new object");
            }

            return n_obj;
        }

        public bool GetIsHTTPServerInMaintain()
        {
            if (statusSets != null)
                foreach (var server in statusSets) {
                    if (server.id == 1) {
                        return server.serverStatus == 2;
                    }
                }

            return true;  
        }

        public bool GetIsTCPServerInMaintain()
        {
            if(statusSets != null)
                foreach (var server in statusSets)
                {
                    if (server.id == 2 )
                    {
                        return server.serverStatus == 2;
                    }
                }

            return true;
        }

        public bool GetIsHTTPServerDown()
        {
            if (statusSets != null)
                foreach (var server in statusSets)
                {
                    if (server.id == 1)
                    {
                        return server.serverStatus == 3;
                    }
                }

            return true;
        }

        [Serializable]
        public class StatusSet {
            public int id = -1;
            public int serverStatus = -1;

            /*
                id: 
                1 - httpserver

                serverStatus:
                1 - normal
                2 - maintenacne
             */
        }
    }

    //=-=-=-=- Friend User Info

    [Serializable]
    public class UserFriendInfos
    {
        public List<UserFriendInfo> datas;

        public static UserFriendInfos NewFromJsonText(string jsonContentText = "")
        {

            UserFriendInfos n_obj = null;
            if (jsonContentText.Length > 0)
            {
                try
                {
                    if (jsonContentText.StartsWith("["))
                    {
                        jsonContentText = "{\"datas\":" + jsonContentText + "}";
                    }

                    n_obj = JsonUtility.FromJson<UserFriendInfos>(jsonContentText);
                }
                catch (Exception e)
                {
                    Debug.Log("Json prase error - reason : " + e);
                }
            }
            else
            {
                Debug.Log("Json text is empty , give out a new object");
                n_obj = new();
            }

            return n_obj;
        }

        [Serializable]
        public class UserFriendInfo
        {
            public int id = -1;  //req queue id
            public string name = "";
            public UserInfo friend;
        }
    }

}

