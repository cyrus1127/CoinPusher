using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QuestData
{
    public int id; // 26
    public string questName; // "連續累計登入10天",
    public string eventQuestName; // "連續累計登入10天",
    public string questDescription; // "連續累計登入10天",
    public string questDescription_sub; // "連續累計登入10天",
    public string eventQuestDescription; // "連續累計登入10天",
    public string eventStartTime;
    public string expirationTime;
    public int type = -1; // quest type
    public int quantity; // mission target
    public int quantitySecondStage; // mission target
    public RewardData reward = null;

    public string GetName() {
        string name = !string.IsNullOrEmpty(questName) ? questName : (!string.IsNullOrEmpty(eventQuestName) ? eventQuestName : "") ;

        return CommonConfig.GetLangQuestWithKey(name);
    }

    public string GetDescription()
    {
        string desc = !string.IsNullOrEmpty(questDescription) ? questDescription : (!string.IsNullOrEmpty(eventQuestDescription) ? eventQuestDescription : "");

        return CommonConfig.GetLangQuestWithKey(desc);
    }

    public bool IsEvent() {
        if(!string.IsNullOrEmpty(eventQuestName))
            return true;

        return false;
    }

    [Serializable]
    public class RewardData
    {
        //public int qty = 0;
        //public string itemId = "";
        public int questId = 0;//": 3,
        public int coinValue = 0;//": 2000,
        public int diamondValue = 0;//": 0,
        public int pointValue = 0;//": 0,
        public int expValue = 0;//": 0,
        public ItemMap[] items = null;

        public ItemMap[] GetItemImgQueue() {
            List<ItemMap> n_q = new();
            if (items != null) {
                n_q.AddRange(items);
            }
            if (coinValue > 0) n_q.Add(ItemMap.NewWith("coin",coinValue));
            if (diamondValue > 0) n_q.Add(ItemMap.NewWith("diamond", diamondValue));
            if (pointValue > 0) n_q.Add(ItemMap.NewWith("point", pointValue));
            if (expValue > 0) n_q.Add(ItemMap.NewWith("exp", expValue));
            
            return n_q.ToArray();
        }

        [Serializable]
        public class ItemMap
        {
            public string itemId = "";
            public int quantity = 0;

            public static ItemMap NewWith(string n_id, int n_qty)
            {
                var n_map = new ItemMap();
                n_map.itemId = n_id;
                n_map.quantity = n_qty;
                return n_map;
            }
        }
    }

   
}


[System.Serializable]
public class QuestDataList
{
    public List<QuestData> quests;

    public static QuestDataList NewFromJsonText(string jsonContentText = "")
    {
        QuestDataList n_obj = null;
        if (jsonContentText.Length > 0)
        {
            if (jsonContentText.StartsWith("[{") || jsonContentText.StartsWith("[ {"))
            {
                jsonContentText = "{\"quests\":" + jsonContentText + "}";
            }

            try
            {
                n_obj = JsonUtility.FromJson<QuestDataList>(jsonContentText);
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

[System.Serializable]
public class QuestItemData
{
    public int id = -1;
    public bool claimed = false;
    public bool isDone = false;
    public bool repeatable = false;
    bool isEvent = false;
    public string eventStartTime;
    public string expirationTime;
    public int currentStage = 0;
    public int qty = 0;
    public int target = 0;
    public int targetSecondStage = 0;
    public string name = "";
    public string eventQuestName = "";
    public string description = "";
    public string descriptionSub = "";
    public string eventQuestDescription = "";
    public QuestData.RewardData rewards = null;

    public static QuestItemData TempData(string name, string description, string description2, QuestData.RewardData rewards, int target, int qty, bool claimed)
    {
        QuestItemData n_d = new QuestItemData();

        n_d.name = name;
        n_d.description = description;
        n_d.descriptionSub = description2;
        n_d.claimed = claimed;
        n_d.rewards = rewards;
        n_d.target = target;
        n_d.qty = qty;

        return n_d;
    }

    public static QuestItemData DataFromQuest(QuestData n_quest, UserQuestData n_userData) {
        QuestItemData n_d = new QuestItemData();

        n_d.id = n_quest.id;
        n_d.name = n_quest.GetName();
        n_d.isEvent = n_quest.IsEvent();
        n_d.eventStartTime = n_quest.eventStartTime;
        n_d.expirationTime = n_quest.expirationTime;

        n_d.description = n_quest.GetDescription();
        if (string.IsNullOrEmpty(n_quest.questDescription_sub)){
            if (n_quest.GetDescription().Length > 0 && n_quest.GetDescription().Contains("*")) {
                var dess = n_quest.GetDescription().Split("*");

                if (dess.Length > 2) {
                    for (int i = 0; i < dess.Length; i++)
                    {
                        if (i == 0)
                            n_d.description = $"{dess[i + 1]}"; //$"{i + 1}. {dess[i + 1]}";
                        else if (i == 1)
                            n_d.descriptionSub = $"{dess[i + 1]}"; // $"{i + 1}. {dess[i + 1]}";
                    }
                }
            }
        }else {
            n_d.descriptionSub = n_quest.questDescription_sub;
        }

        if (n_userData != null) {
            n_d.isDone = n_userData.isDone;
            n_d.claimed = n_userData.claimed;
            n_d.qty = n_userData.quantity;
            n_d.currentStage = n_userData.currentStage;
        }
        
        n_d.rewards = n_quest.reward;
        n_d.target = n_quest.quantity;
        n_d.targetSecondStage = n_quest.quantitySecondStage;

        return n_d;
    }

    public bool IsEvent() => isEvent;
}

[Serializable]
public class UserQuestData
{
    //TODO : Add event field and function
    public UserQuest userQuestKeyId = null;
    public bool claimed = false;
    public bool isDone = false;
    public int quantity = 0;
    public long expirationTime = -1; /// timestamp
    public int currentStage = 0;

    /// =-=-=-=-=-=- event -=-=-=-=-=-=-=
    public UserQuest userEventQuestKeyId = null;
    public bool isUnlocked = false;

    public bool IsEvent() {
        if (userEventQuestKeyId != null && userEventQuestKeyId.userId != 0)
            return true;
        return false;
    }

    public int GetQuestID() {
        return IsEvent() ?  (userEventQuestKeyId != null ? userEventQuestKeyId.eventQuestId : 0) : userQuestKeyId.questId;
    }

    [Serializable]
    public class UserQuest{
        public int userId = 0;
        public int questId = 0;
        public int eventQuestId = 0;
    }
}

[System.Serializable]
public class UserQuestDataList
{
    public List<UserQuestData> quests = null;

    public static UserQuestDataList NewFromJsonText(string jsonContentText = "")
    {
        UserQuestDataList n_obj = null;
        if (jsonContentText.Length > 0)
        {
            if (jsonContentText.StartsWith("[{") || jsonContentText.StartsWith("[ {") || jsonContentText.StartsWith("[")) {
                jsonContentText = "{\"quests\":" + jsonContentText + "}";
            }

            try
            {
                n_obj = JsonUtility.FromJson<UserQuestDataList>(jsonContentText);
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