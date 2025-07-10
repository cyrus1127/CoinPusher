using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
#if UNITY_EDITOR
    using UnityEditor;
#endif

//// =-=-=-=-=- Static Class for config
///
public class AppStartupTCPConfig
{
    ///  HTTP 
    public static string password_d = "12345";
    public static string SERVER_IP_DEV = "dev.littlejoyltd.com"; 
    public static string SERVER_IP_PRE_DEPLOY = "api.happygoldgames.com"; 
    public static string SERVER_IP_PRD = "api-prod.happygoldgames.com";
    public static int HTTP_SERVER_PORT = -1;//443;
    //public static string SERVER_IP_DEV =  "18.166.45.15";
    //public static string SERVER_IP_PRE_DEPLOY = "43.198.135.71";
    //public static string SERVER_IP_PRD =  "127.0.0.1";



    //public static int HTTP_SERVER_PORT = 3000;


    ///  TCP
    //public static string SERVER_TCP_IP_DEV = "dev.littlejoyltd.com"; 
    public static string SERVER_TCP_IP_DEV = "18.166.45.15";
    public static string SERVER_TCP_IP_PRE_DEPLOY = "43.198.135.71";
    public static string SERVER_TCP_IP_PRD = "43.198.132.33";
}

public class CommonConfig
{
    public static bool IsAssetBundled() => true;
    //public static bool IsAssetBundled() => false;
    public static string build_version = "1.1.10";
    public static string bundle_version = "1.1.10.17";
    public static string website_deleteAccount = "https://happygoldgames.io/delete-account";

    /// All keys
    public static string prefs_key_login_timestamp = "login_timestamp";
    public static string prefs_key_login_token = "login_token";
    public static string prefs_key_phoneReg_VID = "phoneReg_VID";
    //// Setting
    public static string prefs_key_language = "prefs_key_language";
    public static string prefs_key_mute = "prefs_key_mute";
    public static string prefs_key_bgmLv = "prefs_key_bgmLv";
    public static string prefs_key_sfxLv = "prefs_key_sfxLv";
    public static string prefs_key_chat = "prefs_key_chat";
    public static string prefs_key_notification = "prefs_key_notification";
    public static string prefs_key_privacy = "prefs_key_privacy";
    public static string prefs_key_userInfo = "prefs_key_userInfo";
    public static string prefs_key_piggySaving = "prefs_key_piggySaving";
    public static string prefs_key_bundle_lastUpdate = "prefs_key_assetbundle_lastUpdate";
    //// Registration
    public static string prefs_key_reg_steps = "prefs_key_reg_steps";
    public static string prefs_key_reg_email = "prefs_key_reg_email";
    public static string prefs_key_reg_pwd = "prefs_key_reg_pwd";
    //public static string prefs_key_ = "";

    public static string[] regionList = { "MY", "SA", "TH", "HK", "TW", "A", "SUSA", "EU", };
    public static string[] phoneRegionCodes = { "+852", "+60", "+44", };
    public static string[] fittingTypes =  { "hair", "face", "upperOutfit", "lowerOutfit", "shoes", "item", "vibe",  };
    public static int[] fittingTypeMap =  { 0, 1, 2, 3, 6, 4, 5, };
    public static int[] fixedEquipment = { 1, 21, 81, 86, 106 }; // M , vibe , F

    public static int TIMEZONE_HK = 8;

    /// =-=-=-=-=-=-=-=-= Resources Paths =-=-=-=-=-=-=-=-=

    public static string rFolder_avatar_icons = "UI&Images/avaters/icons/";
    public static string rFolder_avatar_parts = "UI&Images/avaters/parts/";
    public static string rFolder_cards_mj => IsAssetBundled() ? "" : "UI&Images/games/MJ_HR/cards/";

    /// =-=-=-=-=-=-=-=-= global delegate =-=-=-=-=-=-=-=-=

    public delegate void LanguageChangeCallback();
    public static event LanguageChangeCallback OnLanguageChange;
    static List<LanguageChangeCallback> allCallback = new();
    public static void TriggerLanguageChange()
    {
        OnLanguageChange?.Invoke();
    }
    public static void addLangChangedCallBack(LanguageChangeCallback n_callback)
    {
        if (!allCallback.Contains(n_callback))
        {
            CommonConfig.OnLanguageChange += n_callback;
        }
        else
        {
            Debug.Log("cant add new callback, may exist");
        }
    }

    public static void ClearLangChangesCallBack() {
        OnLanguageChange = null;
        allCallback.Clear();
    }

    /// =-=-=-=-=-=-=-=-= global funcs =-=-=-=-=-=-=-=-=

    public static long GetCurrentDateTimeSince1970inMS()
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddHours(TIMEZONE_HK);
        long currentMillis = dateTimeOffset.ToUnixTimeMilliseconds();
        return currentMillis;
    }

    

    public static void LogoutClean()
    {
        //do keep language
        var lastLang = GetCurLang; /// Display
        var lastBundleVer = GetLastBundleUpdateBuildVersion;

        PlayerPrefs.DeleteAll();

        ClearLangChangesCallBack();
        SetCurLang(lastLang); // recovery
        SetLastBundleUpdateBuildVersion(lastBundleVer); // recovery
    }

    public static void ResetAllPrefs()
    {
        //PlayerPrefs.DeleteAll();

        SetCurChatState(1); /// service
        SetCurNotification(1);
        SetCurPrivacy(0);
        SetCurMuteState(0); /// Sound
        SetCurBGMLv(1f);
        SetCurSFXLv(1f);
        if(GetCurLang == null || string.IsNullOrEmpty(GetCurLang))
            SetCurLang("TC"); /// reset
        SetLastPiggySave(0);
        //PlayerPrefs.Save();
    }

    

    public static bool isiOS()
    {
        return (GetSystemVersion().Contains("iPhone")
            || GetSystemVersion().Contains("iPad")
            || GetSystemVersion().Contains("iOS")
            || GetSystemVersion().Contains("Mac"));
    }

    public static bool isAndroid()
    {
        return GetSystemVersion().Contains("Android");
    }

    public static string GetSystemVersion()
    {
        //return "iOS 17.4.1";
        return SystemInfo.operatingSystem;
    }


    /// Language Mapping
    static Dictionary<string, List<string>> langsMappingDic = null;
    static Dictionary<string, List<string>> langsMappingDic_quest = null;
    static Dictionary<string, List<string>> langsMappingDic_item = null;
    static Dictionary<string, List<string>> langsMappingDic_avatar = null;

    static List<string> sensitivewords = null;

    public static void InitLangPack()
    {
        langsMappingDic = CSVParser.ParseCSV(Path.Combine("", "csv/LanguageMapping.csv"));
        langsMappingDic_quest = CSVParser.ParseCSV(Path.Combine("", "csv/LanguageMappingQuests.csv"));
        langsMappingDic_item = CSVParser.ParseCSV(Path.Combine("", "csv/LanguageMappingItem.csv"));
        langsMappingDic_avatar = CSVParser.ParseCSV(Path.Combine("", "csv/LanguageMappingAvatar.csv"));


        sensitivewords = TextFileParser.ParseCSV(Path.Combine("", "csv/Sensitivewords.txt"), "|");

    }

    static List<string> langQueue = new(new string[] { "TC", "SC", "EN", "MY" });
    public static string[] GetAllLangKeys() {
        var keys = langsMappingDic.GetValueOrDefault("Key");
        return keys.ToArray();
    }

    public static string GetLangWithKey(string key , string targetLang = null)
    {
        return GetLangWithKeyFromDic(langsMappingDic, key, targetLang);
    }

    public static string GetLangQuestWithKey(string key, string targetLang = null)
    {
        return GetLangWithKeyFromDic(langsMappingDic_quest, key, targetLang);
    }

    public static string GetLangItemWithKey(string key, string targetLang = null)
    {
        return GetLangWithKeyFromDic(langsMappingDic_item, key, targetLang);
    }

    public static string GetLangAvatarWithKey(string key, string targetLang = null)
    {
        return GetLangWithKeyFromDic(langsMappingDic_avatar, key, targetLang);
    }

    public static bool CheckContentSensitive(string in_content) {

        if (sensitivewords != null) {
            foreach (string word in sensitivewords) {
                if(!string.IsNullOrEmpty(word) &&  in_content.Contains(word))
                {
                    Debug.Log($"sensitive word found : {word}");
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Base lang dic handling , private
    /// </summary>
    /// <param name="langDic"></param>
    /// <param name="key"></param>
    /// <param name="targetLang"></param>
    /// <returns></returns>
    static string GetLangWithKeyFromDic(Dictionary<string, List<string>> langDic, string key, string targetLang = null)
    {
        string value = "";

        if (langDic != null)
        {
            var keys = langDic.GetValueOrDefault("Key");
            var langVals = langDic.GetValueOrDefault(string.IsNullOrEmpty(targetLang) ? GetCurLang : targetLang);

            if (keys.Contains(key))
            {
                value = langVals[keys.IndexOf(key)];

                if (value.Contains("\n"))
                {
                    value = value.Replace("\n", Environment.NewLine);
                }
                if (value.Contains("\\n"))
                {
                    value = value.Replace("\\n", Environment.NewLine);
                }
            }
        }

        return value;
    }


    /// =-=-=-=-=-=-=-=-= global funcs -> GET-SETTer =-=-=-=-=-=-=-=-=
    public static string GetCurLang => PlayerPrefs.GetString(prefs_key_language);
    public static void SetCurLang(string n_val = null)
    {
        SetStringVal(prefs_key_language, n_val);
        TriggerLanguageChange();
    }
    
    public static string GetCurLangNameMap() {
        string lang_sub = "en";
        switch (GetCurLang)
        {
            case "TC":
                lang_sub = "hant";
                break;
            case "SC":
                lang_sub = "hans";
                break;
            case "MY":
                lang_sub = "my";
                break;
            case "EN":
                lang_sub = "en";
                break;
        }
        return lang_sub;
    }

    public static int GetCurMuteState => PlayerPrefs.GetInt(prefs_key_mute);
    public static void SetCurMuteState(int n_val = -1)
    {
        SetIntVal(prefs_key_mute, n_val);
    }

    public static float GetCurBGMLv => PlayerPrefs.GetFloat(prefs_key_bgmLv);
    public static void SetCurBGMLv(float n_val = -1f)
    {
        SetFloatVal(prefs_key_bgmLv, n_val);
    }

    public static float GetCurSFXLv => PlayerPrefs.GetFloat(prefs_key_sfxLv);
    public static void SetCurSFXLv(float n_val = -1f)
    {
        SetFloatVal(prefs_key_sfxLv, n_val);
    }

    public static int GetCurChatState => PlayerPrefs.GetInt(prefs_key_chat);
    public static void SetCurChatState(int n_val = -1)
    {
        SetIntVal(prefs_key_chat, n_val);
    }

    public static int GetCurNotification => PlayerPrefs.GetInt(prefs_key_notification);
    public static void SetCurNotification(int n_val = -1)
    {
        SetIntVal(prefs_key_notification, n_val);
    }

    public static int GetCurPrivacy => PlayerPrefs.GetInt(prefs_key_privacy);
    public static void SetCurPrivacy(int n_val = -1)
    {
        SetIntVal(prefs_key_privacy, n_val);
    }

    public static string GetCurUserInfo => PlayerPrefs.GetString(prefs_key_userInfo);
    public static void SetCurUserInfo(string n_val = null)
    {
        SetStringVal(prefs_key_userInfo, n_val);
    }

    public static string GetLoginTimestamp => PlayerPrefs.GetString(prefs_key_login_timestamp);
    public static string GetLoginToken => PlayerPrefs.GetString(prefs_key_login_token);
    public static void SetLoginPrefs(string n_timestamp, string n_token)
    {
        SetStringVal(prefs_key_login_timestamp, n_timestamp);
        SetStringVal(prefs_key_login_token, n_token);
    }

    public static string GetLastPhoneRegVID => PlayerPrefs.GetString(prefs_key_phoneReg_VID);
    public static void SetLastPhoneRegVID(string n_val = null)
    {
        SetStringVal(prefs_key_phoneReg_VID, n_val);
    }

    public static int GetLastPiggySave => PlayerPrefs.GetInt(prefs_key_piggySaving);
    public static void SetLastPiggySave(int n_val = -1)
    {
        SetIntVal(prefs_key_piggySaving, n_val);
    }

    public static string GetLastBundleUpdateBuildVersion => PlayerPrefs.GetString(prefs_key_bundle_lastUpdate);
    public static void SetLastBundleUpdateBuildVersion(string n_val = "")
    {
        SetStringVal(prefs_key_bundle_lastUpdate, n_val);
    }


    // registration

    public static string GetCurLastRegEmail => PlayerPrefs.GetString(prefs_key_reg_email);
    public static void SetCurLastRegEmail(string n_val = null)
    {
        SetStringVal(prefs_key_reg_email, n_val);
    }

    public static string GetCurLastRegPWD => PlayerPrefs.GetString(prefs_key_reg_pwd);
    public static void SetCurLastRegPWD(string n_val = null)
    {
        SetStringVal(prefs_key_reg_pwd, n_val);
    }

    public static int GetCurRegSteps => PlayerPrefs.GetInt(prefs_key_reg_steps);
    public static void SetCurRegSteps(int n_val = -1)
    {
        SetIntVal(prefs_key_reg_steps, n_val);
    }


    /* ---- template for new key ----

    * string / json value
    public static string GetCur => PlayerPrefs.GetString(prefs_key_);
    public static void SetCur(string n_val = null)
    {
        SetStringVal(prefs_key_, n_val);
    }

    * int / boolean value
    public static int GetCur => PlayerPrefs.GetInt(prefs_key_);
    public static void SetCur(int n_val = -1)
    {
        SetIntVal(prefs_key_, n_val);
    }

    * float value
    public static float GetCur => PlayerPrefs.GetFloat(prefs_key_);
    public static void SetCur(float n_val = -1f)
    {
        SetFloatVal(prefs_key_, n_val);
    }
    */


    /// =-=-=-=-=-=-=-=-= private funcs =-=-=-=-=-=-=-=-= 
    static void SetStringVal(string key, string n_val = null)
    {
        if (n_val == null)
            RemoveVal(key);
        else
            PlayerPrefs.SetString(key, n_val);
    }

    static void SetIntVal(string key, int n_val = -1)
    {
        if (n_val < 0)
            RemoveVal(key);
        else
            PlayerPrefs.SetInt(key, n_val);
    }

    static void SetFloatVal(string key, float n_val = -1f)
    {
        if (n_val < 0f)
            RemoveVal(key);
        else
            PlayerPrefs.SetFloat(key, n_val);
    }

    static void RemoveVal(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
}

public class CSVParser
{

    public static Dictionary<string, List<string>> ParseCSV(string csvFilePath)
    {
        Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

        // Check if the file exists
        string[] lines = new string[] { };
        if (File.Exists(csvFilePath))
        {
            // Read all lines from the CSV file
            lines = File.ReadAllLines(csvFilePath);   
        }
        else
        {
            Debug.Log("CSV file not found at path: " + csvFilePath);

            // get from resource folder
            TextAsset file = Resources.Load<TextAsset>(csvFilePath.Replace(".csv",""));
            string content = file.ToString();
            if (!string.IsNullOrEmpty(content))
            {
                lines = content.Split(Environment.NewLine);
            }
            else {
                Debug.LogError("project local CSV file not found " );
            }
        }


        // Check if there is at least one line in the file
        if (lines.Length > 0)
        {
            // Get the header line (assumed to be the first line)
            string[] headers = lines[0].Split(',');

            // Iterate through the remaining lines
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(",");

                if (values.Length > headers.Length)
                {
                    // base case for non-chinese wording
                    string[] values_sp = lines[i].Split(",\"");

                    List<string> values_r = new();

                    foreach (var sV in values_sp)
                    {
                        if (sV.Contains("\""))
                        {
                            //string a = sV.Replace("\"", "");
                            //Debug.Log(a);
                            //values_r.Add(a);

                            var svSplit = sV.Split(',');
                            if (svSplit != null)
                            {
                                string compStr = "";
                                foreach (var svstr in svSplit)
                                {
                                    if (svstr.Contains("\""))
                                    {
                                        int idx = svstr.IndexOf("\"");
                                        if (idx == svstr.Length - 1) {
                                            // the text of this lang is ended
                                            values_r.Add(compStr + svstr.Replace("\"", ""));
                                            if(svSplit[svSplit.Length - 1] != svstr)
                                                values_r.Add(svSplit[svSplit.Length - 1]); // add the last one
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        compStr += svstr + ", ";
                                    }
                                }

                                //values_r.Add(compStr);
                            }
                        }
                        else
                        {
                            
                            values_r.AddRange(sV.Split(','));
                        }
                    }

                    if (values_r.Count == headers.Length)
                        values = values_r.ToArray();
                }

                {
                    // Create a Dictionary entry for each header with a list of corresponding values
                    for (int j = 0; j < headers.Length && j < values.Length; j++)
                    {
                        string header = headers[j].Trim();
                        string value = values[j].Trim();

                        if (!data.ContainsKey(header))
                        {
                            data[header] = new List<string>();
                        }

                        data[header].Add(value);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("CSV file is empty!");
        }

        // Access the parsed data as needed
        foreach (var entry in data)
        {
            //Debug.Log("Header: " + entry.Key);
            Debug.Log("Values: " + string.Join("|", entry.Value.ToArray()));
        }

        return data;
    }


}

public class TextFileParser {

    
    //string separator = "|";
    public static List<string> ParseCSV(string filePath , string separator)
    {
        List<string> data = new();

        // Check if the file exists
        string[] lines = new string[] { };
        if (File.Exists(filePath))
        {
            // Read all lines from the CSV file
            lines = File.ReadAllLines(filePath);
        }
        else
        {
            Debug.Log("text file not found at path: " + filePath);

            // get from resource folder
            TextAsset file = Resources.Load<TextAsset>(filePath.Replace(".txt", ""));
            string content = file.ToString();
            if (!string.IsNullOrEmpty(content))
            {
                lines = content.Split(Environment.NewLine);
            }
            else
            {
                Debug.LogError("project local text file not found ");
            }
        }


        // Check if there is at least one line in the file
        if (lines.Length > 0)
        {
            // Get the header line (assumed to be the first line)
            string[] headers = lines[0].Split(separator);

            // Iterate through the remaining lines
            if (lines.Length > 1)
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(",");

                    if (values.Length > headers.Length)
                    {
                        //    // base case for non-chinese wording
                    }

                    {
                        //    // Create a Dictionary entry for each header with a list of corresponding values
                    }
                }
            }
            else {
                data = new(headers);
            }
            
        }
        else
        {
            Debug.LogError("text file is empty!");
        }

        // Access the parsed data as needed
        foreach (var entry in data)
        {
            //Debug.Log("Header: " + entry.Key);
            //Debug.Log("Values: " + string.Join("|", entry.Value.ToArray()));
        }

        return data;
    }
}

public class BundleResourcesManager
{

    //static string bundleHost_DEV => "https://adhocbuildbuck.s3.eu-west-2.amazonaws.com/DevGameBundles/"; // local
    static string bundleHost_DEV => "https://adhocbuildbuck.s3.eu-west-2.amazonaws.com/DevGameBundles/";
    static string bundleHost_PRD => "https://happygold.s3.ap-east-1.amazonaws.com/";
    
    #if PRD
            static public string bundleHost => bundleHost_PRD;
    #else
        #if DEV
            static public string bundleHost => bundleHost_DEV;
        #else
            static public string bundleHost => bundleHost_DEV;
            //static public string bundleHost => bundleHost_PRD;
        #endif
    #endif
    static string[] bundlesNames = { };
    static Dictionary<string,uint> bundlesCRC = new Dictionary<string, uint>{
        { "games_mj_jackey" ,2418108897 }
    };

    public int GetTotalBundleCount()
    {
        if (bundlesNames != null)
            return bundlesNames.Length;
        return 0;    
    }

    static public uint GetBundleCRC(int gameID = -1)
    {
        string key = bundlesNames[gameID - 1];
        uint crCode = bundlesCRC.GetValueOrDefault(key);
        return crCode;
    }

    static public uint GetBundleCRC(string key)
    {
        uint crCode = bundlesCRC.GetValueOrDefault(key);
        return crCode;
    }

    public Hash128 GetBundlehash(string key)
    {
        Hash128 hash = manifest.GetAssetBundleHash(key);
        return hash;
    }

    static public string GetBundleName(int gameID = -1) {
        string fileName = "games_mj_jackey";
        return fileName;
    }

    static public string BundlesPath() {
        return Path.Combine(Application.persistentDataPath, "AssetBundleCachePath");
    }

    static BundleResourcesManager _instance;
    static public BundleResourcesManager Instance() {
        if (_instance == null)
            _instance = new();
        return _instance;
    }

    // =-=- non static
    static Dictionary<string, Cache> bundleCaches = new();
    static Dictionary<string, AssetBundle> bundles = new();
    static List<string> bundleChecklist = new();
    AssetBundleManifest manifest = null;

    public string AssetBundlePath => Path.Combine(Application.persistentDataPath, "AssetBundles");
    public bool CheckIsAssetBundleListExist() {

        Debug.Log($"AssetBundlePath : {AssetBundlePath}");
        if (File.Exists(AssetBundlePath)) {
            if (manifest == null) {
                AssetBundle assetBundleMain = AssetBundle.LoadFromFile(AssetBundlePath);
                if (assetBundleMain != null)
                {
                    manifest = assetBundleMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }
                else {
                    Debug.Log($"AssetBundle file may faulty, plx check {AssetBundlePath}");
                }
            }
            return true;
        }

        return false;
    }

    public bool CompareAssetBundleList()
    {
        string curPath = AssetBundlePath;
        string newPath = AssetBundlePath + "_n";
        Debug.Log($"curreny manifest path : {curPath} \n new manifest path : {newPath}");
        
        if (manifest != null && File.Exists(newPath))
        {
            var latestChanges = CommonConfig.GetLastBundleUpdateBuildVersion;
            if (string.IsNullOrEmpty(latestChanges)
                || (!string.IsNullOrEmpty(latestChanges) && latestChanges != CommonConfig.bundle_version)
                //|| true
                ) {
                manifest = null;
                AssetBundle.UnloadAllAssetBundles(true);

                // load new bundle
                AssetBundle assetBundleMani_n = null;
                try
                {
                    assetBundleMani_n = AssetBundle.LoadFromFile(newPath);
                }
                catch (Exception e)
                {
                    Debug.Log($"Exception : {e}");
                }

                if (assetBundleMani_n != null)
                {
                    var manifest_n = assetBundleMani_n.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    if (manifest_n != null)
                    {
                        Debug.Log("new manifest - should update all");

                        // do replacement
                        {
                            manifest = manifest_n;
                            manifest_n = null;
                            File.Replace(curPath, newPath, curPath);

                            // do clear
                            #if UNITY_EDITOR
                                FileUtil.DeleteFileOrDirectory(newPath);
                            #else        
                                #if UNITY_IPHONE
                                    File.Delete("/private" + newPath);
                                #else
                                    File.Delete(newPath);
                                #endif
                            #endif
                            ClearAllBundles();

                            //Set the latest bundle version number
                            CommonConfig.SetLastBundleUpdateBuildVersion(CommonConfig.bundle_version);
                        }

                        return true;
                    }
                }
                else
                {
                    Debug.Log("same manifest");
                }
            }
        }

        return false;
    }

    // =-=-=-=-=-=- User InApp Storage handling 

    public void ClearAllBundles() {

        if (Directory.Exists(BundlesPath())) {
            #if UNITY_EDITOR
                        FileUtil.DeleteFileOrDirectory(BundlesPath());
            #else
                #if UNITY_IPHONE
                        Directory.Delete("/private" + BundlesPath(), true);
                #else
                        Directory.Delete(BundlesPath(), true);
                #endif
            #endif
        }

        CommonConfig.SetLastBundleUpdateBuildVersion(""); // set empty string for clearing
    }

    public void ClearMainBundlesManifest()
    {
        if (File.Exists(AssetBundlePath))
        {
            #if UNITY_EDITOR
                FileUtil.DeleteFileOrDirectory(AssetBundlePath);
            #else
                #if UNITY_IPHONE
                    File.Delete("/private" + AssetBundlePath);
                #else
                    File.Delete(AssetBundlePath);
                #endif
            #endif
            CommonConfig.SetLastBundleUpdateBuildVersion(""); // set empty string for clearing
        }
    }

    public void ClearTargetBundles(string bundleName)
    {
        if (!string.IsNullOrEmpty(bundleName)) {
            string filePath = Path.Combine(BundlesPath(), bundleName); 
            if (Directory.Exists(BundlesPath()) && File.Exists(filePath))
            {
                #if UNITY_EDITOR
                    FileUtil.DeleteFileOrDirectory(filePath);
                #else
                    #if UNITY_IPHONE
                         File.Delete("/private" + filePath);
                    #else
                         File.Delete(filePath);
                    #endif
                #endif
            }
        }
    }

    public bool CheckTargetBundlesFileSize(string bundleName , float targetSize)
    {
        bool pass = false;
        if (!string.IsNullOrEmpty(bundleName) && targetSize > 0)
        {
            string filePath = Path.Combine(BundlesPath(), bundleName);
            if (Directory.Exists(BundlesPath()) && File.Exists(filePath))
            {
                var info = new System.IO.FileInfo(filePath);
                if (info != null) {
                    if (info.Length == targetSize){
                        //Debug.Log("pass");
                        pass = true;
                    }else {
                        Debug.Log("failed , file size not match");
                    }
                }
            }
        }
        return pass;
    }

    // =-=-=-=-=-=- 
    Dictionary<string, float> bundleFilesMap;
    public void ReloadAllCache() {
        if (!Directory.Exists(BundlesPath()))
            Directory.CreateDirectory(BundlesPath());

        bundleChecklist.Clear();
        // Get the main AssetBundle
        if (CheckIsAssetBundleListExist()) {

            bundlesNames = manifest.GetAllAssetBundles();
            Debug.Log($" manifest GetAllAssetBundles: \n  {string.Join("\n", manifest.GetAllAssetBundles())}");

            //bundleCaches.Add
            foreach (string bundleName in bundlesNames)
            {
                try
                {
                    string bundlePath = Path.Combine(BundlesPath(), bundleName);
                    if (File.Exists(bundlePath))
                    {
                        // check file size also
                        // Do check the current bundle file size , do further handling
                        if (bundleFilesMap != null && !CheckTargetBundlesFileSize(bundleName, GetBundleSize(bundleName)))
                        {
                            ClearTargetBundles(bundleName);
                            bundleChecklist.Add(bundleName);
                        }
                        else {
                            if (!bundleChecklist.Contains(bundleName))
                            {
                                bundleChecklist.Add(bundleName);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Bundle ({bundleName}) : not exist");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"file Bundle ({bundleName}) error : {e} ");
                }
            }
        }
        else
        {
            Debug.LogError($"Bundle manifest ({AssetBundlePath}) : not exist");
        }
    }

    public void LoadTargetInGameCache(string gameID)
    {
        if (!Directory.Exists(BundlesPath()))
            Directory.CreateDirectory(BundlesPath());

        // Get the main AssetBundle
        if (CheckIsAssetBundleListExist())
        {
            bundlesNames = manifest.GetAllAssetBundles();
            Debug.Log($" manifest GetAllAssetBundles: \n  {string.Join("\n", manifest.GetAllAssetBundles())}");

            //bundleCaches.Add
            foreach (string bundleName in bundlesNames)
            {
                if (bundleName.Contains(gameID))
                {
                    string bundlePath = Path.Combine(BundlesPath(), bundleName);

                    try
                    {
                        if (File.Exists(bundlePath))
                        {
                            if (!bundles.ContainsKey(bundleName))
                            {
                                var ex_bundle = AssetBundle.LoadFromFile(bundlePath);
                                bundles.Add(bundleName, ex_bundle);
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Bundle ({bundleName}) : not exist");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"file Bundle ({bundleName}) error : {e} ");
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"Bundle manifest ({AssetBundlePath}) : not exist");
        }
    }

    public int GetTotalBundlesExist() {
        return (bundleChecklist != null) ? bundleChecklist.Count : 0;
    }
    public List<string> GetBundleNeedTo(List<string> targetBundle = null) {
        List<string> wishList = new();

        foreach (var bundleName in bundlesNames)
        {
            if (targetBundle != null)
            {
                if (!bundleChecklist.Contains(bundleName) && targetBundle.Contains(bundleName))
                {
                    wishList.Add(bundleName);
                }
            }
            else {
                if (!bundleChecklist.Contains(bundleName))
                {
                    wishList.Add(bundleName);
                }
            }
        }

        return wishList;
    }

    public List<string> GetAllBundleNames()
    {
        List<string> wishList = new(bundlesNames);
        return wishList;
    }

    public void UpdateBundleFileSize(string bundleName , float fileSize = 0) {
        if (bundleFilesMap == null)
            bundleFilesMap = new();

        // clean for update
        if (bundleFilesMap.ContainsKey(bundleName)){
            bundleFilesMap.Remove(bundleName);
        }

        // Update 
        bundleFilesMap.Add(bundleName, fileSize);

        // Do check the current bundle file size , do further handling
        if (!CheckTargetBundlesFileSize(bundleName, fileSize)) {
            ClearTargetBundles(bundleName);
        }
    }
    public float GetTotalBundleSize(List<string> toCheck) {
        float _size = 0;
        foreach (string bundleName in toCheck){
            _size += GetBundleSize(bundleName);
        }
        return _size;
    }
    public float GetBundleSize(string toCheck)
    {
        float _size = 0;
        if (bundleFilesMap != null && bundleFilesMap.ContainsKey(toCheck))
        {
            _size = bundleFilesMap.GetValueOrDefault(toCheck);
        }
        return _size;
    }

    public void AddCache( string name, Cache n_cache ) {
        if (!bundleCaches.ContainsKey(name)) {
            bundleCaches.Add(name, n_cache);
        }
    }

    public void AddBundle(string name, AssetBundle n_assetBundle)
    {
        if (!bundles.ContainsKey(name))
        {
            bundles.Add(name, n_assetBundle);
        }
    }

    /// =-=-=-=-=-=-=- Bundle and resources Get 

    public AssetBundle GetResBundle(string gameID, string section) {

        string bundleName = string.IsNullOrEmpty(section) ? $"{gameID}" : $"{gameID}_{section}";
        AssetBundle bundle = bundles.GetValueOrDefault(bundleName);
        if (bundle != null) {
            return bundle;
        }
        return null;
    }

    public Sprite GetResource(string gameID, string section = "",  string fileName = "")
    {
        Sprite obj = null;
        if ( !string.IsNullOrEmpty(gameID) && !string.IsNullOrEmpty(fileName) )
        {
            List<string> allpath = new();
            Caching.GetAllCachePaths(allpath);

            //Caching.currentCacheForWriting = Caching.GetCacheAt(Caching.cacheCount);
            AssetBundle bundle = GetResBundle(gameID, section);

            ///// load file from bundle
            if (bundle)
            {
                Texture2D image = bundle.LoadAsset<Texture2D>(fileName);
                if (image != null)
                    obj = Sprite.Create(image, new Rect(Vector2.zero, new Vector2(image.width, image.height)), Vector2.zero);
                else
                    Debug.Log($" asset {fileName} : loaded failed ");
            }
        }

        return obj; 
    }

    public AudioClip GetResourceSound(string gameID, string section = "", string fileName = "")
    {
        AudioClip obj = null;
        if (!string.IsNullOrEmpty(gameID) && !string.IsNullOrEmpty(fileName))
        {
            List<string> allpath = new();
            Caching.GetAllCachePaths(allpath);

            //Caching.currentCacheForWriting = Caching.GetCacheAt(Caching.cacheCount);
            AssetBundle bundle = GetResBundle(gameID, section);

            ///// load file from bundle
            if (bundle)
            {
                AudioClip audioClip = bundle.LoadAsset<AudioClip>(fileName);
                if (audioClip != null)
                    obj = audioClip;
                else
                    Debug.Log($" asset {fileName} : loaded failed ");
            }
        }

        return obj;
    }

    /// =-=-=-=-=-=-=- Bundle release 

    public void UnloadBundle(int gameID = -1)
    {
        Sprite obj = null;
        if (gameID > -1)
        {
            List<string> allpath = new();
            Caching.GetAllCachePaths(allpath);

            //Caching.currentCacheForWriting = Caching.GetCacheAt(Caching.cacheCount);
            AssetBundle bundle = bundles.GetValueOrDefault(bundlesNames[gameID - 1]);

            ///// load file from bundle
            if (bundle)
            {
                bundle.Unload(true);
            }
        }
    }

    public void UnloadAllBundle()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        bundles.Clear();
    }

}

public class CommonFunction{

    public static bool IsContentSpecialChar(string strContent)
    {

        if (!String.IsNullOrEmpty(strContent))
        {
            //var hasSpecialChar = new Regex("^(?=.*[$@!%*?&!\"£$% ^&*()_ +{ }:@~<>?|=[\\]; '#,.\\/\\-])$");
            //var result = Regex.Matches(strContent, "^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+\\.[a-zA-z]{2,3}$");

            string specialChars = "!@#$%^&*()_+{}:\"<>?|\\/.,';][";
            for (int i = 0; i < specialChars.Length; i++)
            {
                if (strContent.Contains(specialChars.Substring(i, 1))) {
                    return true;
                }
                    
                
            }
        }

        return false;
    }

    public static bool CheckIsEmailFormat(string strContent)
    {

        if (!String.IsNullOrEmpty(strContent))
        {
            return Regex.IsMatch(strContent, "@.", RegexOptions.IgnoreCase);
            //return Regex.IsMatch(strContent, "\\|*^!#$%&/()=?»«@£§€{}[].-;'\"<>_,", RegexOptions.IgnoreCase);
        }

        return false;
    }

    static public string GetDigitFormat(long value)
    {

        string str_val = $"{value}";

        if (CommonConfig.GetCurLangNameMap() == "hant" || CommonConfig.GetCurLangNameMap() == "hans")
        {
            //double val_k = (value / 1000.0);
            double val_m = (value / 10000.0);
            double val_b = (val_m / 10000.0);

            if (val_b >= 10000)
            {
                str_val = $"{ GetFloatingNumberFormat((val_b / 10000.0), 1) }兆";
            }
            else if ((int)val_b >= 1)
            {
                str_val = $"{ GetFloatingNumberFormat(val_b, 1) }{ (CommonConfig.GetCurLangNameMap() == "hant" ? "億" : "亿") }";
            }
            else if ((int)val_m >= 1)
            {
                str_val = $"{GetFloatingNumberFormat(val_m, 1)}{ (CommonConfig.GetCurLangNameMap() == "hant" ? "萬" : "万") }";
            }
        }
        else {
            double val_k = (value / 1000.0);
            double val_m = (val_k / 1000.0);
            double val_b = (val_m / 1000.0);
            if (val_b > 1000)
            {
                str_val = $"{ GetFloatingNumberFormat((val_b / 1000.0), 1) }T";
            }else if (val_b > 1)
            {
                str_val = $"{ GetFloatingNumberFormat(val_b, 1) }B";
            }
            else if ((int)val_m >= 1)
            {
                str_val = $"{GetFloatingNumberFormat(val_m, 1)}M";
            }
            else if ((int)val_k >= 10)
            {
                str_val = $"{GetFloatingNumberFormat((float)val_k, (int)val_k > 10 ? 1 : 2)}K";
            }
        }

        

        return str_val;
    }

    static public string GetFloatingNumberFormat(double value , int lenght = 2)
    {
        double multi = Math.Pow(10 , lenght);
        string str_val = string.Format("{0:n" + lenght + "}", Math.Truncate(value*(multi))/ multi);

        return str_val;
    }

    static public string GetDotNumberFormat(long value)
    {
        string str_val = string.Format("{0:#,##0}", value);

        return str_val;
    }

    static public string GetPadNumberFormat( long value, int padLengh = 3)
    {
        string str_val = string.Format("{0," + padLengh + ":D" + padLengh + "}", value);

        return str_val;
    }

    static public string GetSecureText(string n_str)
    {
        //string str_val = Regex.Replace(n_str, @"^[a-zA-Z0-9]+$", "*");
        string str_val = "";

        for (int i = 0; i < n_str.Length; i++)
        {
            str_val += "*";
        }

        return str_val;
    }

    //static public SignalTransactionController GetTranscator(GameObject cardHelper)
    //{
    //    SignalTransactionController stc = cardHelper.GetComponent<SignalTransactionController>();
    //    if (stc == null)
    //    {
    //        stc = cardHelper.AddComponent<SignalTransactionController>();
    //    }
    //    return stc;
    //}

    static public int[] ChipExchange(int in_coins, int[] excHole)
    {
        List<int> coins = new();
        while (coins.Count < (excHole.Length + 1)) coins.Add(0);

        int coinLeft = in_coins;
        for (int idx = 0; idx < excHole.Length; idx++)
        {
            int p = excHole[idx];
            if (coinLeft / p > 0)
            {
                coins[idx] = (int)(coinLeft / p);
                coinLeft -= coins[idx] * p;
            }
        }
        // the changes
        coins[excHole.Length] = coinLeft;

        // the hole found
        return coins.ToArray();
    }

    static public bool IsValidURL(string urlStr) {
        if (!string.IsNullOrEmpty(urlStr) && urlStr.Contains("http") && urlStr.Contains("://")) {
            return true;
        }

        return false;
    }

    static public string GetDigitString(int number)
    {
        return number.ToString("D2");
    }

    static public bool HaveConnection() {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            return true;
        }
        return false;
    }

    static public bool HaveWifiConnected() {
        if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return true;
        }
        return false;
    }
}


/// <summary>
/// source ref :  https://github.com/gdsmith/jquery.easing/blob/master/jquery.easing.js
/// </summary>
public class EasingCal {


    //var pow = Math.Pow;
    //    sqrt = Math.sqrt,
    //    sin = Math.sin,
    //    cos = Math.cos,
    static float c1 = 1.70158f;
    static float c2 = c1 * 1.525f;
    static float c3 = c1 + 1;
    static float c4 = (float)(2 * Math.PI) / 3f;
    static float c5 = (float)(2 * Math.PI) / 4.5f;


    public static float EaseInQuad(float x)
    {
        return x * x;
    }

    public static float EaseOutQuad(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }

    public static float EaseInOutQuad(float x)
    {
        return x < 0.5f ?
        2 * x * x :
        1 - (float)Math.Pow(-2 * x + 2, 2) / 2;
    }

    public static float EaseInCubic(float x)
    {
        return x * x * x;
    }

    public static float EaseOutCubic(float x)
    {
        return 1 - (float)Math.Pow(1 - x, 3);
    }

	public static float  EaseInOutCubic(float x)
    {
        return x < 0.5f ?
            4 * x * x * x :
            1 - (float)Math.Pow(-2 * x + 2, 3) / 2;
    }

    public static float BounceOut(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            return n1 * (x -= (1.5f / d1)) * x + .75f;
        }
        else if (x < 2.5f / d1)
        {
            return n1 * (x -= (2.25f / d1)) * x + .9375f;
        }
        else
        {
            return n1 * (x -= (2.625f / d1)) * x + .984375f;
        }
    }

    public static float BounceIn(float x)
    {
        return 1 - BounceOut(1 - x);
    }

}