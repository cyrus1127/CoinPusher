using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingController : CommonSceneViewManager
{
    [SerializeField] ExternalEditingHelper editor_language;
    [SerializeField] Text txt_lang_val;

    [SerializeField] Slider sl_bgm;
    [SerializeField] Slider sl_sfx;

    [SerializeField] SwitchButton sb_audio;
    [SerializeField] SwitchButton sb_chat;
    [SerializeField] SwitchButton sb_notification;
    [SerializeField] SwitchButton sb_privacy;

    Dictionary<string, string> defaultLangMapDic = new()
    {
        { "TC", "繁體中文" },
        { "SC", "简体中文" },
        { "MY", "Melayu" },
        { "EN", "English" }
    };

    // Start is called before the first frame update
    protected override void InitAtStart()
    {
        base.InitAtStart();
        
        /// init MC panel
        if (editor_language != null)
        {
            var MyList = new List<string>(defaultLangMapDic.Values);
            (editor_language as ExternalMulitChoiceHelper).SetChoices(MyList);
        }

        sb_privacy.onChangeDel = SwitchStateChanged_privacy;
        sb_notification.onChangeDel = SwitchStateChanged_notification;
        sb_chat.onChangeDel = SwitchStateChanged_chat;
        sb_audio.onChangeDel = SwitchStateChanged_audio;
        sl_bgm.onValueChanged.AddListener(SliderValChanged_bgm);
        sl_sfx.onValueChanged.AddListener(SliderValChanged_sfx);
    }

    void UpdateAllButton() {
        /// TODO : get the latest UserPreferences

        txt_lang_val.text = defaultLangMapDic.GetValueOrDefault(CommonConfig.GetCurLang);
        Debug.Log("CommonConfig.GetCurBGMLv ? " + CommonConfig.GetCurBGMLv);
        sl_bgm.value = CommonConfig.GetCurBGMLv;
        sl_sfx.value = CommonConfig.GetCurSFXLv;
        sb_audio.SetState(CommonConfig.GetCurMuteState);
        sb_chat.SetState(CommonConfig.GetCurChatState);
        sb_notification.SetState(CommonConfig.GetCurNotification);
        sb_privacy.SetState(CommonConfig.GetCurPrivacy);
    }

    public void SetShow() {
        this.gameObject.SetActive(true);
        UpdateAllButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LanguageChangeRequest()
    {
        MenuButtonClicked();
        /// TODO : show input box
        editor_language.startEdit(txt_lang_val.text, EditDone);
    }

    void EditDone(string n_content)
    {
        Debug.Log(n_content);
        txt_lang_val.text = n_content;

        //find the key
        string val_key = null;
        foreach (KeyValuePair<string, string> entry in defaultLangMapDic)
        {
            // do something with entry.Value or entry.Key
            if (entry.Value == n_content) {
                val_key = entry.Key;
                break;
            }
        }

        CommonConfig.SetCurLang(val_key);
    }

    void SliderValChanged_bgm(float n_val) {
        CommonConfig.SetCurBGMLv(n_val);
        ReloadSoundboxSetup();
    }

    void SliderValChanged_sfx(float n_val)
    {
        CommonConfig.SetCurSFXLv(n_val);
        ReloadSoundboxSetup();
    }

    void SwitchStateChanged_audio(int n_state) {
        CommonConfig.SetCurMuteState(n_state);
        ReloadSoundboxSetup();
    }

    void SwitchStateChanged_chat(int n_state)
    {
        CommonConfig.SetCurChatState(n_state);
    }

    void SwitchStateChanged_notification(int n_state)
    {
        CommonConfig.SetCurNotification(n_state);
    }

    void SwitchStateChanged_privacy(int n_state)
    {
        CommonConfig.SetCurPrivacy(n_state);
    }

    public void MenuButtonClicked()
    {
        PlayOnClickSound_inGame();
    }

}
