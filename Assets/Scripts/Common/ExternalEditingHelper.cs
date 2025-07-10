using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ExternalEditingHelper : MonoBehaviour
{
    public delegate void EndEditCallback(string n_input);
    public EndEditCallback myDelegate = null;

    [SerializeField] Button btn_sub;
    protected bool isIndexValOut = false;

    void Start()
    {
        initBtn();
    }

    public void SetValToIndex() {
        isIndexValOut = true;
    }

    public virtual void startEdit(string preset_content, EndEditCallback n_delegate)
    {
        myDelegate = n_delegate;
        gameObject.SetActive(true);
    }

    protected virtual void initBtn()
    {
        if (btn_sub != null) btn_sub.onClick.AddListener(onBtnSubmitPressed);
    }

    protected virtual void onBtnSubmitPressed()
    {
        childAction(-1);

        gameObject.SetActive(false);
    }

    protected virtual void childAction(dynamic value) { }
}
