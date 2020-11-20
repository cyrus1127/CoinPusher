using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStore : MonoBehaviour
{

    public int coins;
    public int cashs;
    public string lastLogoutTime;

    static string key_coins = "key_coins";
    static string key_cashs = "key_cashs";
    static string key_logoutTime = "key_logoutTime";

    // Start is called before the first frame update
    void Start()
    {
        //Get
        coins = GetIntValByKey(key_coins);
        cashs = GetIntValByKey(key_cashs);
        
    }

    private void OnApplicationQuit()
    {
        

        PlayerPrefs.Save();
    }

    private bool IsKeyExisting(string in_key)
    {
        return PlayerPrefs.HasKey(in_key);
    }

    //----- get functions -----
    /// <summary>
    /// Get Integer Value 
    /// </summary>
    /// <param name="in_key"></param>
    /// <returns></returns>
    private int GetIntValByKey(string in_key)
    {
        if (in_key.Length != 0) {
            if (!IsKeyExisting(in_key))
            {
                //initial the value
                SetIntValByKey(in_key, 0);
            }
            else
            {
                //Direct return the stored value
                return PlayerPrefs.GetInt(in_key);
            }
        }

        return 0;       
    }

    /// <summary>
    /// Get Float Value 
    /// </summary>
    /// <param name="in_key"></param>
    /// <returns></returns>
    private float GetFloatValByKey(string in_key)
    {
        if (in_key.Length != 0)
        {
            if (!IsKeyExisting(in_key))
            {
                //initial the value
                SetFloatValByKey(in_key, 0f);
            }
            else
            {
                //Direct return the stored value
                return PlayerPrefs.GetFloat(in_key);
            }
        }

        return 0f;
    }

    /// <summary>
    /// Get String Value 
    /// </summary>
    /// <param name="in_key"></param>
    /// <returns></returns>
    private string GetStringValByKey(string in_key)
    {
        if (in_key.Length != 0)
        {
            if (!IsKeyExisting(in_key))
            {
                //initial the value
                SetStringValByKey(in_key, "");
            }
            else
            {
                //Direct return the stored value
                return PlayerPrefs.GetString(in_key);
            }
        }

        return "";
    }

    //----- set functions -----
    /// <summary>
    /// Set Integer Value 
    /// </summary>
    /// <param name="in_key"></param>
    /// <param name="in_val"></param>
    public void SetIntValByKey(string in_key , int in_val)
    {
        if (in_key.Length != 0)
        {
            PlayerPrefs.SetInt(in_key, in_val);
        }
    }

    /// <summary>
    /// Set Float Value 
    /// </summary>
    /// <param name="in_key"></param>
    /// <param name="in_val"></param>
    public void SetFloatValByKey(string in_key, float in_val)
    {
        if (in_key.Length != 0)
        {
            PlayerPrefs.SetFloat(in_key, in_val);
        }
    }

    /// <summary>
    /// Set String Value 
    /// </summary>
    /// <param name="in_key"></param>
    /// <param name="in_val"></param>
    public void SetStringValByKey(string in_key, string in_val)
    {
        if (in_key.Length != 0)
        {
            PlayerPrefs.SetString(in_key, in_val);
        }
    }
}
