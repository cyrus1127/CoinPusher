using System;
using System.Collections;
using System.Collections.Generic;
//using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonSceneViewManager : MonoBehaviour
{
    [SerializeField] protected Common_alert_box alert_Box;
    protected NotOnDestroy notDestory;
    protected virtual bool RequireFirebaseUserStateListener() => false;
    //EzyTCPHandler _ezyTCPHandler;

    private void Awake()
    {
        GameObject sectObj = GameObject.Find("loginNotDestroy");
        if (sectObj != null)
        {
            notDestory = sectObj.GetComponent<NotOnDestroy>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject sectObj = GameObject.Find("loginNotDestroy");
        if (sectObj != null)
        {
            notDestory = sectObj.GetComponent<NotOnDestroy>();
            notDestory.InitFireBase((passVal) =>
            {
                if (passVal >= 0)
                {
                    //default call to check firebase user login status
                    //if (notDestory.auth != null && RequireFirebaseUserStateListener())
                    //{
                    //    notDestory.auth.StateChanged += AuthStateChanged;
                    //    notDestory.auth.IdTokenChanged += IdTokenChanged;
                    //    AuthStateChanged(this, null);
                    //}
                }
                else
                {
                    Print("InitFireBase failed");
                }

                InitAtStart();
            });
            //ezyTCPHandler = sectObj.GetComponent<EzyTCPHandler>();

            Application.memoryUsageChanged += MemoryStateChanged;
            Application.lowMemory += notDestory.MemoryAutoUnload;
        }
        
    }

    private void OnDestroy()
    {
        //if (notDestory != null && notDestory.auth != null && RequireFirebaseUserStateListener())
        //{
        //    notDestory.auth.StateChanged -= AuthStateChanged;
        //    notDestory.auth.IdTokenChanged -= IdTokenChanged;
        //}
    }

    // =-=-=-=-=-=-=- FireBase login session handlings =-=-=-=-=-=-=-=-

    bool isPreRegisterIngore = false;
    protected void SetIsPreRegister(bool isPause) {
        isPreRegisterIngore = isPause;
    }

    // Track state changes of the auth object.
    protected void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        //if (notDestory.auth.CurrentUser != notDestory.user)
        //{
        //    bool signedIn = notDestory.user != notDestory.auth.CurrentUser && notDestory.auth.CurrentUser != null;
        //    if (!signedIn && notDestory.user != null)
        //    {
        //        Debug.Log("Signed out " + notDestory.user.UserId);

        //        /// clean the
        //        if ( !isPreRegisterIngore)
        //        {
        //            if(notDestory != null)
        //                notDestory.LogoutDestroy();
        //            /// TODO : call scene manage to back to login view
        //            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        //        }
        //    }
        //    notDestory.user = notDestory.auth.CurrentUser;
        //    if (signedIn)
        //    {
        //        Debug.Log("Signed in " + notDestory.user.UserId);
        //        notDestory.UpdateJWTToken();
        //    }
        //}
    }

    protected void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        //if (notDestory.auth.CurrentUser != notDestory.user)
        //{
        //    notDestory.UpdateJWTToken();
        //}
    }

    public string GetJWT()
    {
        if(notDestory != null) {
            notDestory.UpdateJWTToken();
            //Print(String.Format("Token[0:8] = {0}", notDestory.firebaseUserJWT.Substring(0, 8)));
            return notDestory.firebaseUserJWT;
        }
        
        return "";
    }


    // =-=-=-=-=-=-=- 
    void MemoryStateChanged(in ApplicationMemoryUsageChange newUsage)
    {
        Print($"Current App memory usage was changed - { newUsage.memoryUsage }");
        switch (newUsage.memoryUsage) {
            case ApplicationMemoryUsage.Critical: break;
            case ApplicationMemoryUsage.High: break;
            case ApplicationMemoryUsage.Medium: break;
            case ApplicationMemoryUsage.Low: break;
            case ApplicationMemoryUsage.Unknown:
                GC.Collect();
                break;
        }
    }

    //public EzyTCPHandler ezyTCPHandler() {
    //    GameObject sectObj = GameObject.Find("loginNotDestroy");
    //    if (sectObj != null)
    //    {
    //        _ezyTCPHandler = sectObj.GetComponent<EzyTCPHandler>();
    //    }

    //    return _ezyTCPHandler;
    //}

    public void RoomQuit() {
        //_ezyTCPHandler = null;
    }

    protected virtual void InitAtStart() {}

    protected void ReloadSoundboxSetup()
    {
        /// get user preferences
        float volumn_BGM = CommonConfig.GetCurBGMLv;
        float volumn_SFX = CommonConfig.GetCurSFXLv;
        if (CommonConfig.GetCurMuteState == 1) {
            volumn_BGM = 0;
            volumn_SFX = 0;
        }

        GetSoundBoxController()?.ChangeVolume(SoundBoxController.SoundBoxAudioType.BGM , volumn_BGM);
        GetSoundBoxController()?.ChangeVolume(SoundBoxController.SoundBoxAudioType.SFX, volumn_SFX);
    }

    protected SoundBoxController GetSoundBoxController() {
        if (notDestory != null)
        {
            return notDestory.GetComponent<SoundBoxController>();
        }
        return null;
    }

    /// =-=-=-=-=-=-=-=-=-=-= BGM & SFX =-=-=-=-=-=-=-=-=-=-=

    protected void PlayOnClickSound_lobby()
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlaySFX(0);
        }
    }

    protected void PlayOnClickSound_inGame()
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlaySFX(1);
        }
    }

    protected void PlayOnClickSound_inGame_tableItemClick()
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlaySFX(2);
        }
    }

    protected void PlayInGameBGM(string gameAppName, string clipName)
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlayClip(SoundBoxController.SoundBoxAudioType.BGM, gameAppName, clipName);
        }
    }

    protected void PlayInGameClip(string gameAppName, string clipName , SoundBoxController.SoundBoxAudioType layer = SoundBoxController.SoundBoxAudioType.SFX)
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlayClip(layer, gameAppName, clipName);
        }
    }

    protected void PlayCommonClip(string clipName, SoundBoxController.SoundBoxAudioType layer = SoundBoxController.SoundBoxAudioType.SFX)
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlayClip(layer, "COMMON", clipName);
        }
    }

    protected void StopAllInGameClip()
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlayClip(SoundBoxController.SoundBoxAudioType.SFX);
        }
    }

    protected void StopClip(SoundBoxController.SoundBoxAudioType channel)
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.StopSFX(channel);
        }
    }

    protected void ResetBGM() {
        if (notDestory != null)
        {
            GetSoundBoxController()?.StartBGM();
        }
    }

    protected async void Print(string msg)
    {
        await Awaitable.MainThreadAsync();
        //DevDebugLoggerController devDebugLogger = GameObject.Find("logContainer")?.GetComponent<DevDebugLoggerController>();
        //devDebugLogger?.PrintLog(msg);

        Debug.Log($"{msg}");
    }


    //// =-=-=-=-=-=-=- Network handling

    HttpReqManager httpReqManager = null;
    protected HttpReqManager GetHttpReqManager() {

        if (httpReqManager == null)
            httpReqManager = new(this.gameObject);

        return httpReqManager;
    }

    protected bool IsResponseError(string response , out int errorCode) {

        var result = HttpReqManager.IsResponseError(response, out int errCode);

        errorCode = errCode;
        return result;
    }

    protected bool DoCheckNetworkConnection(bool isBeforeLogin = false)
    {
        if (!CommonFunction.HaveConnection())
        {
            if (isBeforeLogin)
            {
                alert_Box?.SetTypeWithDelegateCallback(Common_alert_box.AlertType.Warning, "Network connection failed. Would you like to restart the game?", (obj) =>
                {
                    //BundleResourcesManager.Instance().ClearMainBundlesManifest();
                    Application.Quit();
                });
            }
            else {
                alert_Box?.SetTypeWithDelegateCallback(Common_alert_box.AlertType.Warning, "Network connection failed. Would you like to relaunch the app and back to login?", (obj) =>
                {
                    //BundleResourcesManager.Instance().ClearMainBundlesManifest();
                    SceneManager.LoadScene("Launcher_reload");
                });
            }
            
            return false;
        }
        return true;
    }

    protected void DoCheckServiceState()
    {
        if (DoCheckNetworkConnection() && notDestory != null)
        {
            // check service status
            GetHttpReqManager().CheckServiceStatus(GetJWT(), (response) => {
                if (!IsResponseError(response, out int errorCode))
                {
                    Debug.Log(response);
                    string to_response = "[{\"id\": 1, \"serverStatus\":2}]";
                    if (response.Contains("true"))
                    {
                        to_response = "[{\"id\": 1, \"serverStatus\":1}]";
                    }
                    notDestory.UpdateServerStatus(to_response);

                    if (notDestory.serverService.GetIsHTTPServerDown())
                    {
                        //TODO : show popup
                        alert_Box.SetTypeWithDelegateCallback(Common_alert_box.AlertType.Reminder, "遊戲正在維護中(3)\n游戏正在维护中(3)\nThe game is under maintenance(3)", (obj) => {
                            Application.Quit();
                        });
                    }
                    else if (notDestory.serverService.GetIsHTTPServerInMaintain())
                    {
                        //TODO : show popup
                        alert_Box.SetTypeWithDelegateCallback(Common_alert_box.AlertType.Reminder, "遊戲正在維護中\n游戏正在维护中\nThe game is under maintenance", (obj) => {
                            Application.Quit();
                        });
                    }
                }
                else
                {
                    //TODO : server is adsolutly down (or having other issues)
                    //do nothing
                    //notDestory.UpdateServerStatus("[{\"id\": 1, \"serverStatus\":3}]");

                    alert_Box.SetTypeWithDelegateCallback(Common_alert_box.AlertType.Reminder, CommonConfig.GetLangWithKey("common_reconnect_failed_2"), (obj) => {
                        Application.Quit();
                    });
                }

                
            });
        }
    }

    // =-==-=-=-=-=-=-= device application callback =-=-=-=-=-=-=--=

    protected long inBackground_s = 0;
    
    protected bool onAppPause = false;
    protected virtual void OnApplicationPause(bool pauseStatus)
    {
        Print($"OnApplicationPause : pauseStatus ? {pauseStatus}");
        onAppPause = pauseStatus;

        if (!CommonConfig.IsAssetBundled())
            return; // on local dev dont apply Application pause handling

        if (onAppPause)
        {
            // mark the timestamp for on pause status changed
            inBackground_s = CommonConfig.GetCurrentDateTimeSince1970inMS();
            /// disconnect TCP
        }
        else
        {
            // check server
            //DoCheckServiceState();

            // back from background check 
            if (inBackground_s != 0)
            {
                // 
                long cur_ms = CommonConfig.GetCurrentDateTimeSince1970inMS();
                if (cur_ms - inBackground_s > (60 * 1000))
                {
                    // more then 1min , kick
                    //alert_Box.SetTypeWithDualCallback(Common_alert_box.AlertType.Reminder, CommonConfig.GetLangWithKey("Ingame_disconnection_timeoutkick"),
                    //() => {});
                }
                else
                {
                    /// less then 1min    
                }
                inBackground_s = 0;
            }
        }
    }


    
}
