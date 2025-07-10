using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpReqHandler : MonoBehaviour
{
    public delegate void RequestResponse(string data);
    RequestResponse _delegate;

    string server_domain = "";
    int server_port = -1;
    string req_path = "";
    string post_body = "";
    string bearerKey = "";
    const int timeOutTime = 10;
    Coroutine onReqThread = null;

    public void Setup(string domain, int port = -1)
    {
        server_domain = domain;
        server_port = port;
    }

    public void SetBearer(string n_bearerKey)
    {
        bearerKey = n_bearerKey;
    }

    private string DataDicToString(Dictionary<string, object> bodyDic ,bool inGetForm = true) {
        string n_str_body = "";
        if (bodyDic != null && bodyDic.Count > 0)
        {
            n_str_body = "{";
            int index = 0;
            foreach (KeyValuePair<string, object> data in bodyDic)
            {
                if (inGetForm)
                {
                    string str_KnV = "？";
                    if (index > 0)
                    {
                        str_KnV += "&";
                    }
                    str_KnV += $"{data.Key}={data.Value}";

                    n_str_body += str_KnV;
                }
                else
                {
                    string str_KnV = "";
                    if (index > 0)
                    {
                        str_KnV += ",";
                    }
                    if (data.Value.GetType().Equals(typeof(string)))
                        str_KnV += $"\"{data.Key}\":\"{data.Value}\"";
                    else if (data.Value.GetType().Equals(typeof(Boolean)))
                    {
                        if (Boolean.TryParse(data.Value.ToString(), out bool boolVal))
                        {
                            str_KnV += $"\"{data.Key}\":{ (boolVal ? "true" : "false") }";
                        }
                    }
                    else
                        str_KnV += $"\"{data.Key}\":{data.Value}";

                    n_str_body += str_KnV;
                    
                }
                index++;
            }
            
            n_str_body += "}";
        }
        
        
        return n_str_body;
    }

    public void MakeGetRequest(string path, Dictionary<string, object> bodyDic, RequestResponse n_callback)
    {
        if (onReqThread == null)
        {
            req_path = (path.StartsWith("/") ? "" : "/") + path;
            Debug.Log($"req uri : {req_path}");

            post_body = DataDicToString(bodyDic);

            _delegate = n_callback;
            onReqThread = StartCoroutine(AwaitRespond(req_path, post_body));
        }

    }

    public void MakePutRequest(string path, Dictionary<string, object> bodyDic, RequestResponse n_callback)
    {
        if (onReqThread == null)
        {
            req_path = (path.StartsWith("/") ? "" : "/") + path;
            Debug.Log($"req uri : {req_path}");

            post_body = DataDicToString(bodyDic, false);

            _delegate = n_callback;
            onReqThread = StartCoroutine(AwaitPutRespond(req_path, post_body));
        }

    }

    public void MakePostRequest(string path, Dictionary<string,object> bodyDic , RequestResponse n_callback ) {
        if (onReqThread == null) {
            req_path = (path.StartsWith("/") ? "" : "/" ) + path;

            _delegate = n_callback;
            if (bodyDic != null && bodyDic.Count > 0)
            {
                /// direct pass object to reqest function 
                //onReqThread = StartCoroutine(AwaitRespond(req_path, bodyDic, false));
                post_body = DataDicToString(bodyDic, false);
                onReqThread = StartCoroutine(AwaitRespond(req_path, post_body, false));
            }
            else {
                onReqThread = StartCoroutine(AwaitRespond(req_path, "", false));
            }
        }
    }

    public void MakeDeleteRequest(string path, RequestResponse n_callback)
    {
        if(onReqThread == null)
        {
            req_path = (path.StartsWith("/") ? "" : "/") + path;
            Debug.Log($"req uri : {req_path}");

            _delegate = n_callback;
            onReqThread = StartCoroutine(AwaitDeleteRespond(req_path));
        }
    }

    IEnumerator AwaitRespond(string path , System.Object post_body, bool isGet = true)
    {
        string full_url = server_domain + (server_port > -1 ? ":" + server_port : "") + path;
        UnityWebRequest webRequest = null;
        if (!isGet)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            
            if (post_body != null && post_body.GetType().Equals(typeof(Dictionary<string, System.Object>)))
            {

                Dictionary<string,string> post_body_dss = new();
                foreach (KeyValuePair<string, object> data in post_body as Dictionary<string, System.Object>)
                {
                    if (data.Value.GetType().Equals(typeof(string)))
                        post_body_dss.Add(data.Key, $"\"{data.Value}\"");
                    else
                        post_body_dss.Add(data.Key, $"{data.Value}");
                }
                // Set the request body
                webRequest = UnityWebRequest.Post(full_url, post_body_dss);
            }
            else {
                webRequest = new UnityWebRequest(full_url, "POST");
                if ((post_body as string).Length > 0)
                {
                    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes((post_body as string));
                    webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                    webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                    //byte[] bodyRaw = Encoding.UTF8.GetBytes((post_body as string));
                    //formData.Add(new MultipartFormDataSection(bodyRaw));
                }
                else {
                    webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                }

                //webRequest = UnityWebRequest.Post(full_url, formData);
            }

            Debug.Log($"start request to {full_url} , body {post_body}");

            
        }
        else {
            full_url += $"{post_body}";
            webRequest = UnityWebRequest.Get(full_url);
            
        }
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept", "application/json");
        if(!string.IsNullOrEmpty(bearerKey))
            webRequest.SetRequestHeader("Authorization", "Bearer " + bearerKey);
        webRequest.timeout = timeOutTime;
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
            _delegate?.Invoke(webRequest.error);
            if (onReqThread != null)
            {
                StopCoroutine(onReqThread);
                onReqThread = null;
            }
        }
        else
        {
            Debug.Log($"Post request complete! \n response : {webRequest.result} " );
            string response_body = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
                response_body = webRequest.result.ToString();

            _delegate?.Invoke(response_body);
            if (onReqThread != null) {
                StopCoroutine(onReqThread);
                onReqThread = null;
            }
        }
    }

    IEnumerator AwaitPutRespond(string path , string post_body)
    {
        string full_url = server_domain + (server_port > -1 ? ":" + server_port : "") + path;
        UnityWebRequest webRequest = null;
        //full_url += $"{post_body}";
        webRequest = UnityWebRequest.Put(full_url, post_body);
        // set header
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + bearerKey);
        webRequest.timeout = timeOutTime;
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
            _delegate?.Invoke(webRequest.error);
            if (onReqThread != null)
            {
                StopCoroutine(onReqThread);
                onReqThread = null;
            }
        }
        else
        {
            Debug.Log($"Post request complete! \n response : {webRequest.result} ");
            string response_body = webRequest.downloadHandler.text;

            _delegate?.Invoke(response_body);
            if (onReqThread != null)
            {
                StopCoroutine(onReqThread);
                onReqThread = null;
            }
        }
    }

    IEnumerator AwaitDeleteRespond(string path)
    {
        string full_url = server_domain + (server_port > -1 ? ":" + server_port : "") + path;
        UnityWebRequest webRequest = null;
        //full_url += $"{post_body}";
        webRequest = UnityWebRequest.Delete(full_url);
        // set header
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + bearerKey);
        webRequest.timeout = timeOutTime;
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
            _delegate?.Invoke(webRequest.error);
            if (onReqThread != null)
            {
                StopCoroutine(onReqThread);
                onReqThread = null;
            }
        }
        else
        {
            Debug.Log($"Post request complete! \n response : {webRequest.result} ");
            string response_body = webRequest.result.ToString();

            _delegate?.Invoke(response_body);
            if (onReqThread != null)
            {
                StopCoroutine(onReqThread);
                onReqThread = null;
            }
        }
    }
}

public class HttpReqManager {

    private string server_domain;
    private int port;
    List<HttpReqHandler> httpReqHandlers;
    GameObject parentHolder;
    public HttpReqManager(GameObject n_parentHolder)
    {
#if PRD
    server_domain = AppStartupTCPConfig.SERVER_IP_PRD;
#else
    #if DEV
                    server_domain = AppStartupTCPConfig.SERVER_IP_DEV;
    #else
        server_domain = AppStartupTCPConfig.SERVER_IP_PRE_DEPLOY;
    #endif
#endif
        port = AppStartupTCPConfig.HTTP_SERVER_PORT;
        server_domain = $"{ ((port > -1) ? "http" : "https") }://{server_domain}";
        httpReqHandlers = new();
        parentHolder = n_parentHolder;
    }

    public HttpReqManager(GameObject n_parentHolder , string n_server_domain, int n_port) {
        port = n_port;
        server_domain = $"{ ((port > -1) ? "http" : "https") }://{n_server_domain}";
        httpReqHandlers = new();
        parentHolder = n_parentHolder;
    }
    
    protected HttpReqHandler GetHttpReqHandler(string fireBaseJWT = null)
    {
        var n_httpReqHandler = parentHolder.AddComponent<HttpReqHandler>();
        n_httpReqHandler.Setup(server_domain, port);
        if (!httpReqHandlers.Contains(n_httpReqHandler))
        {
            httpReqHandlers.Add(n_httpReqHandler);
        }

        if (fireBaseJWT != null) {
            n_httpReqHandler.SetBearer(fireBaseJWT);
        }
        
        return n_httpReqHandler;
    }

    protected void RemoveHandler(HttpReqHandler endReqHandler) {
        if (httpReqHandlers.Contains(endReqHandler))
        {
            httpReqHandlers.Remove(endReqHandler);
            GameObject.Destroy(endReqHandler);
        }
    }

    static public bool IsResponseError(string response, out int errorCode)
    {
        if (response.ToLowerInvariant().Contains("not found") || response.Length == 0)
        {
            errorCode = 1010;
            return true;
        }
        else if (response.Contains("HTTP/") && (response.Contains("400") || response.Contains("401") || response.Contains("404") || response.Contains("405") || response.Contains("500")))
        {
            int errCode = response.Contains("400") ? 400 :
                (response.Contains("401") ? 401 :
                (response.Contains("404") ? 404 :
                (response.Contains("405") ? 405 : 500)));

            errorCode = errCode;
            return true;
        }
        else if (response.ToLowerInvariant().Contains("unknown error"))
        {
            errorCode = 1000;
            return true;
        }
        else if (response.ToLowerInvariant().Contains("timeout"))
        {
            errorCode = 1020;
            return true;
        }


        errorCode = 0;
        return false;
    }


    /// =-==-=-=-=-=-==- apis =-==-=-=-=-=-=-=-=-=
    const string API_BUNDLES = "/api/bundles";
    const string API_USER_INFO = "/api/user";
    const string API_USER_INV = "/api/user-item";
    const string API_USER_QUEST = "/api/user-quest";
    const string API_USER_WARDROBE = "api/user-avatar-item";
    const string API_VAULT = "/api/user/vault-";
    const string API_Event_KEY = "event-";
    

    /// =-==-=-=-=-=-==- request apis =-==-=-=-=-=-=-=-=-=


    public void GetAppResourceBundleList(string appVersion, Action<string> resultCallback) {
        
        var httpReq = GetHttpReqHandler();
        httpReq.MakeGetRequest($"{API_BUNDLES}/{appVersion}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void CheckServiceStatus(string fireBaseJWT, Action<string> resultCallback) {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        string version = CommonConfig.build_version;
        httpReq.MakeGetRequest($"/api/system-status/client-access/{version}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void CheckForceAppVersion(Action<string> resultCallback) {
        var httpReq = GetHttpReqHandler();
        string version = CommonConfig.build_version;
        httpReq.MakeGetRequest($"/api/system-status/client-access/{version}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    /// =-==-=-=-=-=-==- request apis =-==-=-=-=-=-=-=-=-=

    public void VerifyNewRegEmail(string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/verify-email", null,
            (response) =>
            {
                resultCallback(response);
                RemoveHandler(httpReq);
            }
        );
    }

    public void RegisterNewUser(Dictionary<string, object> body , string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePostRequest($"{API_USER_INFO}/reg", body,
            (response) =>
            {
                resultCallback(response);
                RemoveHandler(httpReq);
            }
        );
    }

    public void GuestLinkAsNewUser(Dictionary<string, object> body, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);

        httpReq.MakePostRequest($"{API_USER_INFO}/non-guest/reg", body,
            (response) =>
            {
                resultCallback(response);
                RemoveHandler(httpReq);
            }
        );
    }

    

    public void GetLoginUserInfo(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/login/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetLoginSocialUserInfo(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/login_social/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    /// =-=-=-=-=-=-=-=- User Info -=-=-=-=-=-=-=-=-=

    public void GetUserInfo(string uid , string fireBaseJWT , Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/id/{uid}", null, 
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetMultiUserInfo(string uids, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/id/{uids}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void UpdateUserInfo(string uid, Dictionary<string,object> body, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePutRequest($"{API_USER_INFO}/id/{uid}", body,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void UpdateUserWearing(string uid, Dictionary<string, object> body, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePutRequest($"{API_USER_INFO}/avatar/id/{uid}", body,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetUserActiveBuffs(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/status/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetUserInventory(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INV}/user-id/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetUserWardrobe(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_WARDROBE}/user-id/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetUserQuests(string uid, bool isEvent, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/user-{(isEvent ? API_Event_KEY : "")}quest/user-id/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void ClaimUserQuests(string uid, int quest_id, bool isEvent, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/user-{(isEvent ? API_Event_KEY : "")}quest/claim/{(isEvent ? API_Event_KEY : "")}quest-id/{quest_id}/user-id/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void ClaimAllUserQuests(string uid, string quest_type, bool isEvent, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        string apiPath = $"/api/user-{(isEvent ? API_Event_KEY : "")}quest/claim/user-id/{uid}/types/{quest_type}";
        if(isEvent)
            apiPath = $"/api/user-{(isEvent ? API_Event_KEY : "")}quest/claim/user-id/{uid}";
        httpReq.MakeGetRequest(apiPath, null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void MakeDeposit(string uid, string fireBaseJWT, bool isCoin, long amount, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePutRequest($"{API_VAULT}{ (isCoin ? "coin" : "diamond") }/deposit/id/{uid}/{amount}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void MakeWithDraw(string uid, string fireBaseJWT, bool isCoin, long amount, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePutRequest($"{API_VAULT}{ (isCoin ? "coin" : "diamond") }/withdraw/id/{uid}/{amount}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }



    public void GetFriendList(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/friends/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetPendingFriendRequest(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/friend-requests/pending/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void AddFriend(string uid, string target_uid, string fireBaseJWT, Action<string> resultCallback) {

        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePostRequest($"/api/friend-requests/send?senderId={(long)int.Parse(uid)}&receiverId={(long)int.Parse(target_uid)}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void HandleFriendReq(int requestId, bool isAccept, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePostRequest($"/api/friend-requests/handle?requestId={requestId}&accept={isAccept}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void UnFriend(int target_Friendid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeDeleteRequest($"/api/friends/delete/{target_Friendid}",
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetSearchUserRequestByFUID(string uid, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/user/uid/{uid}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetSearchUserRequestByName(string nickName, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/user/nickname/{nickName}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    /// =-=-=-=-=-=-=-=- Item -=-=-=-=-=-=-=-=-=

    public void GetItemShopList(string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"api/item/all", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void ItemConsume(string uid, int item_id, int itemCategory, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        string typePath = "";
        switch (itemCategory) {
            case 1: typePath = "avatar-item/"; break;
            default: break;
        }
        string uri = $"{API_USER_INV}/consume/{typePath}user-item-key/{uid}/{item_id}";

        httpReq.MakePutRequest(uri, null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void ItemBuy(string currency, string uid, string item_id, int qty, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePutRequest($"{API_USER_INV}/buy/{currency}/{uid}/{item_id}/{qty}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetOutfitShopList(string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"/api/avatar-item/all", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void OutfitBuy(string currency, string uid, string item_id, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePutRequest($"{API_USER_WARDROBE}/buy/{currency}/{uid}/{item_id}/1", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void IAPAppleStoreConfirm(Dictionary<string, object> body, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePostRequest($"/api/shop/iap/storePurchaseProcessAccepted", body,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void IAPGooglePlayConfirm(Dictionary<string, object> body, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakePostRequest($"/api/shop/iap/playStorePurchaseProcessAccepted", body,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    

    /// =-=-=-=-=-=-=-=- Quest -=-=-=-=-=-=-=-=-=

    public void GetQuestList(bool isEvent, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"api/{(isEvent ? API_Event_KEY : "")}quest/all", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    /// =-=-=-=-=-=-=-=- Lucky Draw -=-=-=-=-=-=-=-=-=
    public void RequestDrawGame(string playerID, int machine, int quantity, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest( $"api/shop/draw/{playerID}/{machine}/{quantity}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    /// =-=-=-=-=-=-=-=- InGame -=-=-=-=-=-=-=-=-=
    public void GetPlayerGameNoBanned(string playerID, int gameID, string fireBaseJWT, Action<string> resultCallback)
    {
        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest($"{API_USER_INFO}/can-join/{gameID}/{playerID}", null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }

    public void GetPlayerInfoSeatInRoom(string playerID, string fireBaseJWT, Action<string> resultCallback)
    {
        string apiPath = $"{API_USER_INFO}/id/{playerID}";
        if (playerID.Contains("bot"))
         apiPath = $"{API_USER_INFO}/uid/{playerID}";


        var httpReq = GetHttpReqHandler(fireBaseJWT);
        httpReq.MakeGetRequest(apiPath, null,
           (response) =>
           {
               resultCallback?.Invoke(response);
               RemoveHandler(httpReq);
           }
        );
    }


    
}