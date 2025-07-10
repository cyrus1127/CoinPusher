using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    int curState = 0;
    [SerializeField] bool defaultOn;

    [SerializeField] Text lbl_0;
    [SerializeField] Text lbl_1;
    [SerializeField] RectTransform img_indicator_rect;

    Image _img;

    [SerializeField] bool haveColorTransit;
    [SerializeField] List<Color> colors = new();

    public delegate void onChangeDelegate(int state);
    public onChangeDelegate onChangeDel = null;

    

    // Start is called before the first frame update
    void Start()
    {
        if (_img == null)
            _img = GetComponent<Image>();

        //if (defaultOn)
        //{
        //    curState = 1;
        //}
        updateIndicatorPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void updateIndicatorPos() {

        if(_img == null)
            _img = GetComponent<Image>();

        RectTransform myRect = GetComponent<RectTransform>();
        float padding = 5;
        float indi_margin = (48 - (img_indicator_rect.sizeDelta.x / 2));
        // Update the indicator
        float n_pos_x = 0 - indi_margin + padding;
        if (curState != 0)
        {
            n_pos_x = (myRect.sizeDelta.x - img_indicator_rect.sizeDelta.x) - indi_margin - padding;
        }

        if (haveColorTransit && _img != null) {
            _img.color = colors[curState];
        }

        //Debug.Log(curState + " ? " + n_pos_x);
        img_indicator_rect.localPosition = new Vector3(n_pos_x, 0, 0);
    }

    public int GetCurState => curState;
    public void SetState(int isOn = 0)
    {
        curState = isOn;
        updateIndicatorPos();
    }


    /// =-=-=-=-=-=-=-= callback =-=-=-=-=-=-=-= 
    public void SwitchonClicked()
    {
        curState = (curState + 1) % 2;
        updateIndicatorPos();

        if (onChangeDel.Target != null)
        {
            onChangeDel(curState);
        }
    }
}
