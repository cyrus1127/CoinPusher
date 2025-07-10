using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =-=-=-=-=-=-=- List -=-=-=-=-=-=-=

[Serializable]
public class BackPackItemDataList
{
    //public ItemData[] itemDatas;
    public List<BackPackItemData> itemDatas;

    public static BackPackItemDataList NewFromJsonText(string jsonContentText = "")
    {
        BackPackItemDataList n_obj = null;
        if (jsonContentText.Length > 0)
        {
            // do replace:
            if(!jsonContentText.Contains("itemDatas"))
                jsonContentText = "{\"itemDatas\":" + jsonContentText + "}";
            try
            {
                n_obj = JsonUtility.FromJson<BackPackItemDataList>(jsonContentText);
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
}

[Serializable]
public class ShopItemDataList
{
    //public ItemData[] itemDatas;
    public List<ShopItemData> items;

    public static ShopItemDataList NewFromJsonText(string jsonContentText = "")
    {
        ShopItemDataList n_obj = null;
        if (jsonContentText.Length > 0)
        {
            if (jsonContentText.StartsWith("[{")) {
                jsonContentText = "{\"items\":" + jsonContentText + "}";
            }

            try
            {
                n_obj = JsonUtility.FromJson<ShopItemDataList>(jsonContentText);
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
}


[Serializable]
public class FittingItemDataList
{
    //public ItemData[] itemDatas;
    public List<AvaterItemData> items;

    public static FittingItemDataList NewFromJsonText(string jsonContentText = "")
    {

        FittingItemDataList n_obj = null;
        if (jsonContentText.Length > 0)
        {
            if (jsonContentText.StartsWith("[{") || jsonContentText.StartsWith("["))
            {
                jsonContentText = "{\"items\":" + jsonContentText + "}";
            }

            try
            {
                n_obj = JsonUtility.FromJson<FittingItemDataList>(jsonContentText);
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
}

// =-=-=-=-=-=-=- base -=-=-=-=-=-=-=

/// <summary>
///  Shop item
/// </summary>
[Serializable]
public class ShopItemData
{
    // Start is called before the first frame update
    public string id = "";
    public string itemIapId = "";  // iap_002
    public string itemName = "";
    public string subName = "";
    public string itemDescription = "";
    public int type = -1; // 
    public string image_path = "";
    public string iconFileName = "";
    public int coinPrice = 0;
    public int diamondPrice = 0;
    public int pointPrice = 0;
    public float storePrice = 0;
    public bool purchasable = false;
    public int quantity = 0;
    public int rarity = 0;

    public bool isRare => false;

    public string GetTypeName() {
        string[] map = { "subscription", "currency", "skill" };

        return type > -1 ? map[type] : "";
    }

    public static ShopItemData FromJsonData(string jsonContentText = "")
    {

        ShopItemData n_obj = null;
        if (jsonContentText.Length > 0)
        {
            try
            {
                n_obj = JsonUtility.FromJson<ShopItemData>(jsonContentText);
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

    public string GetCurrencyType()
    {
        var currencyType = "coin";
        if (diamondPrice > 0)
            currencyType = "diamond";
        else if (pointPrice > 0)
            currencyType = "point";

        return currencyType;
    }

    public int GetValue() {
        
        var value = coinPrice;
        if (diamondPrice > 0)
            value = diamondPrice;
        else if (pointPrice > 0)
            value = pointPrice;

        return value;
    }


    ///  =-=-=-=-=-=-=- file =-=-==-=-=-=-

    public string GetImageFileName()
    {
        string out_sprite_name = "";

        string _file_name = iconFileName;

        if (_file_name.Length > 0)
        {
            if (_file_name.Contains(".png"))
            {
                _file_name = _file_name.Replace(".png", "");
            }

            out_sprite_name = _file_name;

        }
        else
        {
            Debug.Log("This item is missing resource file itemName");
        }

        return out_sprite_name;
    }

    public virtual string GetImagePath()
    {
        return "UI&Images/lobby/shop&Market/items/";
    }
    
    public Sprite GetImage() {
        return GetImage(GetImagePath()) ;
    }

    public Sprite GetImage(string foldPath)
    {
        Sprite out_sprite = null;

        try
        {
            if (!string.IsNullOrEmpty(id))
                out_sprite = Resources.Load<Sprite>(foldPath + GetImageFileName());
            else {
                if (GetValue() > 0) {
                    Debug.Log("This is a only currency item for view");
                    //out_sprite = Resources.Load<Sprite>($"UI&Images/general/icn-{GetCurrencyType()}");
                    out_sprite = Resources.Load<Sprite>($"UI&Images/lobby/shop&Market/items/icn-backpack-{GetCurrencyType()}");
                }
            }

        }
        catch (Exception e)
        {
            Debug.Log("Resources Load failed - error : " + e.Message);
        }

        return out_sprite;
    }

}

// Backpack 
[Serializable]
public class BackPackItemData
{
    // Start is called before the first frame update
    public InventoryItemData item = null;
    public InventoryUserItem userItem = null;
    public string expirationTime = ""; // yyyy-MM-ddThh:mm:ss
    public int quantity = 0; //
    public core.data.UserInfo user;

    public string[] items;
    public string[] avatarItems;


    public static BackPackItemData FromJsonData(string jsonContentText = "")
    {
        BackPackItemData n_obj = null;
        if (jsonContentText.Length > 0)
        {
            try
            {
                n_obj = JsonUtility.FromJson<BackPackItemData>(jsonContentText);
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
}

[Serializable]
public class InventoryUserItem
{
    public InventoryUserItemKeyId userItemKeyId = null;
    public InventoryUserAvatarItemKeyId userAvatarItemKeyId = null;

    public int quantity ;
    public string expirationTime;
}

[Serializable]
public class InventoryUserItemKeyId
{
    public int itemId = -1;
}

[Serializable]
public class InventoryUserAvatarItemKeyId
{
    public int avatarItemId = -1;
}

[Serializable]
public class InventoryItemData
{
    // Start is called before the first frame update
    public int id = 0; 
    public string itemIapId = "";// iap_002
    public string itemName = "";
    public string itemDescription = "";
    public int itemCategory = -1;
    public string type = ""; // subscription , currency , skill
    public int value = 0; /// will related with "type" field


    /*item category
            0 = normal
            1 = avatar
            2 = subscription
            3 = ticket
            4 = account
            5 = quest
    */
    public bool isAvaterItem() => itemCategory == 1;
    public bool isSubscriptItem() => itemCategory == 2;
    public bool isTicketItem() => itemCategory == 3;
    public bool isAccountItem() => itemCategory == 4;
    public bool isQuestItem() => itemCategory == 5;
}

/// <summary>
///  fitting avatar 
/// </summary>
[Serializable]
public class AvaterItemData
{
    // Start is called before the first frame update
    public int id = -1;
    public string index = "";
    public string avatarItemName = "";
    public string avatarItemDescription = "";
    public string image_path = "";
    public string icnFileName = "";
    public string fileName = "";
    public string secondLayerFileName = "";
    public int type = 0;  ///// 0 = common , 1 = consume , 2 = appearence , 3 = ....
    public int gender = 0; // 0 = M , 1 = F
    public string status = ""; // "", onsale , expired
    public int p_style = 0;
    public int p_add = 0;
    public int p_lucky = 0;
    public int rarity = 0;
    public int coinPrice = 0;
    public int diamondPrice = 0;
    public int pointPrice = 0;
    public bool purchasable = false;
    public int[] invokeStatus = { };
    public string expirationTime = "";

    public bool IsEquipment()
    {
        if ((int)(id / 100000) >= 1 && (int)(id / 100000) <= 8)
        {
            return true;
        }

        return false;
    }

    public int GetEquipmentPrefix()
    {
        int sectionID = (int)(id / 100000);

        return sectionID * 100000;
    }

    public bool HaveSecondLayer => secondLayerFileName.Length > 0;

    public bool isRare => (rarity >= 5);

    public bool isExpired() {
    //expirationTime = "2025-06-05T22:26:42.466147";

        if (!string.IsNullOrEmpty(expirationTime)) {
            if (DateTime.TryParse(expirationTime, out DateTime dateTime))
            {
                long diff = (long)(dateTime - DateTime.UtcNow).TotalMilliseconds;
                //long diff = (long)(dateTime - DateTime.UtcNow.AddHours(CommonConfig.TIMEZONE_HK)).TotalMilliseconds;
                if (diff < 5000 ) return true;
            }
        }
        return false;
    }

    public string GetImageFileName(bool needIcon = false, bool getSecond_layer = false)
    {
        string out_sprite_name = "";

        string _file_name = needIcon ? icnFileName : (getSecond_layer ? secondLayerFileName : fileName);

        if (_file_name.Length > 0)
        {
            if (_file_name.Contains(".png"))
            {
                _file_name = _file_name.Replace(".png", "");
            }

            out_sprite_name = _file_name;

        }
        else
        {
            Debug.Log("This item is missing resource file itemName");
        }

        return out_sprite_name;
    }


    public Sprite GetImage(string foldPath)
    {
        Sprite out_sprite = null;

        try
        {
            out_sprite = Resources.Load<Sprite>(foldPath + GetImageFileName());
        }
        catch (Exception e)
        {
            Debug.Log("Resources Load failed - error : " + e.Message);
        }

        return out_sprite;
    }

    /// =-=-=-=-=-=-=- Fitting room only -=-=-=-=-=-=-=-=-=
    public string GetImagePath(bool needIcon = false)
    {
        string out_path = "";
        string _directory_path = (needIcon ? CommonConfig.rFolder_avatar_icons : CommonConfig.rFolder_avatar_parts);

        if (_directory_path.Length > 0)
        {
            out_path = _directory_path;
        }
        else
        {
            Debug.Log("This item is missing resource _directory_path");
        }

        return out_path;
    }

    public Sprite GetImage(bool needIcon = false, bool getSecond_layer = false)
    {
        Sprite out_sprite = null;

        try
        {
            string path = GetImageFileName(needIcon, getSecond_layer);
            if (!string.IsNullOrEmpty(path))
            {
                if (CommonConfig.IsAssetBundled())
                    out_sprite = BundleResourcesManager.Instance().GetResource("avatar", "outfit", path);
                else
                    out_sprite = Resources.Load<Sprite>(GetImagePath(needIcon) + path);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Resources Load failed - error : " + e.Message);
        }

        return out_sprite;
    }

}


