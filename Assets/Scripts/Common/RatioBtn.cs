using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RatioBtn : MonoBehaviour
{
    Text lbl_;
    Image img_onSelect;
    Button btn_self;
    UnityAction<int> _onClickCallback = null;

    int _idx;

    // Start is called before the first frame update
    private void Awake()
    {
        InitComponents();
    }

    void Start()
    {

        InitComponents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitComponents() {
        /// find object
        if (btn_self == null) {
            Image[] imgs = GetComponentsInChildren<Image>();
            foreach (Image img in imgs)
            {
                if (img_onSelect == null && img.gameObject.name == "img_onSelected")
                {
                    img_onSelect = img;
                }
            }

            lbl_ = GetComponentInChildren<Text>();
            btn_self = GetComponent<Button>();

            btn_self.onClick.AddListener(OnClicked);
        }
    }

    public void SetData(int idx, string text, UnityAction<int> onClickCallback ,bool onSelected = false ) {
        InitComponents();
        _idx = idx;
        lbl_.text = text;
        _onClickCallback = onClickCallback;
        SetOnSelected(onSelected);
    }

    public void SetOnSelected(bool isSelected) {
        if (img_onSelect != null)
         img_onSelect.gameObject.SetActive(isSelected);
    }

    void OnClicked() {
        _onClickCallback?.Invoke(_idx);
    }
}
