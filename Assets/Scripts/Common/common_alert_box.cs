using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Common_alert_box : MonoBehaviour
{

    [SerializeField] protected Text lbl_type;
    [SerializeField] protected Text lbl_msg;
    [SerializeField] protected Text lbl_title;
    [SerializeField] protected Button btn_confirm;
    [SerializeField] protected Button btn_set_confirm;
    [SerializeField] protected Button btn_set_cancel;

    protected NotOnDestroy notDestory;

    protected List<UnityAction> confirmListers = new();
    public delegate void ContentTextOnClickCallBack();
    public delegate void ConfirmBtnCallBack(Object returnObj);
    protected ConfirmBtnCallBack _delegate;
    protected ContentTextOnClickCallBack _delegate_contentText;
    protected Object objReturn;
    


    public enum AlertType
    {
        Warning,
        Reminder,
        ReminderSystem,
        Information,
        Confirm
    }
    protected AlertType curType;



    // Start is called before the first frame update
    void Start()
    {
        GameObject sectObj = GameObject.Find("loginNotDestroy");
        if (sectObj != null)
        {
            notDestory = sectObj.GetComponent<NotOnDestroy>();
        }

        // set button onclick defeat
        btn_confirm?.onClick.AddListener(()=>SetHidden(true));
        btn_set_cancel?.onClick.AddListener(() => SetHidden());
        InitAtStart();
    }

    protected virtual void InitAtStart() {
        btn_set_confirm?.onClick.AddListener(() => SetHidden(true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHidden(bool isTrigDelegate = false)
    {
        PlayOnClickSound_lobby();
        if (isTrigDelegate) {
            _delegate?.Invoke(objReturn);
        }
        gameObject.SetActive(false);
        removeConfirmCallback();
    }


    public void SetTypeWithDualCallback(AlertType n_type, string n_msg, System.Action confirmCallbacks = null , System.Action cancelCallbacks = null)
    {

        SetTypeWithMsg(n_type, n_msg);
        if (confirmCallbacks != null)
        {
            AddConfirmCallback(confirmCallbacks , cancelCallbacks);
        }
        gameObject.SetActive(true);
    }

    public void SetTypeWithDelegateCallback(AlertType n_type, string n_msg, ConfirmBtnCallBack n_delegate = null , ContentTextOnClickCallBack n_delegate_contentText = null)
    {
        SetTypeWithMsg(n_type, n_msg);
        _delegate = n_delegate;
        _delegate_contentText = n_delegate_contentText;
        gameObject.SetActive(true);
    }


    protected void SetTypeWithMsg(AlertType n_type, string n_msg) {
        switch (n_type)
        {
            case AlertType.Warning:
            case AlertType.Reminder:
            
                btn_confirm.gameObject.SetActive(true);
                btn_set_confirm.gameObject.SetActive(false);
                btn_set_cancel.gameObject.SetActive(false);

                if (n_type == AlertType.Warning)
                {
                    lbl_type.text = CommonConfig.GetLangWithKey("common_msgBox_warning");
                }
                else
                {
                    lbl_type.text = CommonConfig.GetLangWithKey("common_msgBox_reminder");
                }

                break;
            case AlertType.Information:
                lbl_type.text = CommonConfig.GetLangWithKey("common_msgBox_reminder");
                btn_confirm.gameObject.SetActive(false);
                btn_set_confirm.gameObject.SetActive(false);
                btn_set_cancel.gameObject.SetActive(false);
                break;
            case AlertType.Confirm:
            case AlertType.ReminderSystem:
                
                lbl_type.text = CommonConfig.GetLangWithKey(n_type == AlertType.Confirm ? "common_msgBox_reminder" : "common_msgBox_reminder_system");
                btn_confirm.gameObject.SetActive(false);
                btn_set_confirm.gameObject.SetActive(true);
                btn_set_cancel.gameObject.SetActive(true);
                break;
        }
        curType = n_type;
        lbl_msg.text = n_msg;
    }

    protected void AddConfirmCallback( UnityAction n_callback ) {
        if (!confirmListers.Contains(n_callback))
        {
            Debug.Log("new callback");
            confirmListers.Add(n_callback);
            if (curType == AlertType.Confirm)
            {
                btn_set_confirm.onClick.AddListener(n_callback);
            }
            else
            {
                btn_confirm.onClick.AddListener(n_callback);
            }
        }
        else {
            Debug.Log("new callback is existing");
        }
    }

    protected void AddConfirmCallback(System.Action n_callback , System.Action n_cancelCallback = null)
    {
        if (curType == AlertType.Confirm || curType == AlertType.ReminderSystem)
        {
            btn_set_confirm.onClick.AddListener(()=> {
                n_callback?.Invoke();
                n_callback = null;
                n_cancelCallback = null;
            });
            btn_set_cancel.onClick.AddListener(() => {
                n_cancelCallback?.Invoke();
                n_callback = null;
                n_cancelCallback = null;
            });
        }
        else
        {
            btn_confirm.onClick.AddListener(() => {
                n_callback?.Invoke();
                n_callback = null;
            });
        }
    }

    void removeConfirmCallback()
    {
        foreach (UnityAction callback in confirmListers ) {
            if (curType == AlertType.Confirm)
            {
                //btn_set_confirm.onClick.RemoveListener(callback);
                btn_set_confirm.onClick.RemoveAllListeners();
                btn_set_cancel.onClick.RemoveAllListeners();
            }
            else {
                //btn_confirm.onClick.RemoveListener(callback);
                btn_confirm.onClick.RemoveAllListeners();
            }
        }
        confirmListers.Clear();
        _delegate = null;

        Debug.Log("clear all callbacks ");
    }

    public void HiddenBtns() {
        btn_confirm.gameObject.SetActive(false);
        btn_set_confirm.gameObject.SetActive(true);
        btn_set_cancel.gameObject.SetActive(true);
    }

    public void ChangeTitle(string n_txt)
    {
        lbl_type.text = n_txt;
    }

    public void ContentTextOnClicked() {

        if (_delegate_contentText != null)
        {
            _delegate_contentText?.Invoke();
        }
    }

    /// =-=-=-=-=-=-=-=-=-=-= BGM & SFX =-=-=-=-=-=-=-=-=-=-=
    protected SoundBoxController GetSoundBoxController()
    {
        if (notDestory != null)
        {
            return notDestory.GetComponent<SoundBoxController>();
        }
        return null;
    }

    protected void PlayOnClickSound_lobby()
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlaySFX(0);
        }
    }

    protected void PlayCommonClip(string clipName, SoundBoxController.SoundBoxAudioType layer = SoundBoxController.SoundBoxAudioType.SFX)
    {
        if (notDestory != null)
        {
            GetSoundBoxController()?.PlayClip(layer, "COMMON", clipName);
        }
    }

}
