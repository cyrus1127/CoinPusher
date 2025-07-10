using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalMulitChoiceHelper : ExternalEditingHelper
{
    List<string> choices;
    [SerializeField] GameObject choice_prefab;
    [SerializeField] GameObject content_list;

    int onSelected_idx = -1;
    string preset_content = "";
    int preset_content_idx = 0;

    public override void startEdit(string n_preset_content, EndEditCallback n_delegate)
    {
        preset_content = n_preset_content;
        UpdateContentList();

        base.startEdit(preset_content, n_delegate);
    }

    public void startEdit(int n_preset_content_idx, EndEditCallback n_delegate)
    {
        base.startEdit(preset_content, n_delegate);

        SetValToIndex();
        preset_content_idx = n_preset_content_idx;
        UpdateContentList();
    }

    string isLangMapReq = "";
    public void SetChoices(List<string> n_choises, string isChoisesLangKey = "")
    {
        //if (choices == null) {
        //    choices = new(CommonConfig.regionList);
        //}
        isLangMapReq = isChoisesLangKey;
        choices = n_choises;
        if (choices.Count > 0)
        {
            UpdateContentList();
        }
    }

    public void ChangeSelected(int n_preset_content_idx)
    {
        preset_content_idx = n_preset_content_idx;
        UpdateContentList();
    }

    void UpdateContentList()
    {
        if (content_list != null)
        {
            /// add new choice
            if (choice_prefab != null)
            {
                /// do clear content list
                if (content_list.transform.childCount > 0)
                {
                    for (int i = content_list.transform.childCount - 1; i >= 0; i--)
                    {
                        GameObject.Destroy(content_list.transform.GetChild(i).gameObject);
                    }
                }

                RectTransform pre_rect = choice_prefab.GetComponent<RectTransform>();
                foreach (string choice in choices)
                {
                    int i = choices.IndexOf(choice);

                    Quaternion n_rot = Quaternion.AngleAxis(0, Vector3.zero);
                    GameObject n_item = GameObject.Instantiate(choice_prefab, Vector3.zero, n_rot, content_list.transform);
                    n_item.transform.localPosition = new Vector3(0, 0, 0);
                    RatioBtn rbtn = n_item.GetComponent<RatioBtn>();
                    if (isIndexValOut)
                    {
                        onSelected_idx = preset_content_idx;
                    }
                    else {
                        if (preset_content == choice)
                        {
                            onSelected_idx = i;
                        }
                    }

                    if(string.IsNullOrEmpty(isLangMapReq))
                        rbtn.SetData(i, choice, onChoisePressed, preset_content == choice);
                    else
                        rbtn.SetData(i, CommonConfig.GetLangWithKey($"{isLangMapReq}{choice}"), onChoisePressed, preset_content == choice);
                }
            }
            else
            {
                /// update child
                Transform listTf = content_list.transform;
                for (int i = 0; i < listTf.childCount; i++)
                {
                    string choice = "";
                    GameObject child = listTf.GetChild(i).gameObject;
                    RatioBtn rbtn = child.GetComponent<RatioBtn>();

                    if (isIndexValOut)
                    {
                        onSelected_idx = preset_content_idx;
                    }
                    else
                    {
                        choice = choices[i];
                        if (preset_content == choice)
                        {
                            onSelected_idx = i;
                        }
                        rbtn.SetData(i, choice, onChoisePressed, preset_content == choice);
                    }
                    
                    rbtn.SetOnSelected(i == onSelected_idx);
                    /// TODO : get handler , set the onSelected value
                }
            }
        }
    }

    


    protected override void childAction(dynamic value)
    {
        string n_input = $"{value}";
        if (value is int intVal && !isIndexValOut) {
            onSelected_idx = intVal;
            n_input = choices[intVal];

            /// update child
            Transform listTf = content_list.transform;
            for (int i = 0; i < listTf.childCount; i++)
            {
                GameObject child = listTf.GetChild(i).gameObject;
                RatioBtn rbtn = child.GetComponent<RatioBtn>();
                rbtn.SetOnSelected(i == onSelected_idx);
                /// TODO : get handler , set the onSelected value
            }
        }

        myDelegate?.Invoke(n_input);
    }

    public void onChoisePressed(int idx)
    {
        if (myDelegate != null)
        {
            childAction(idx);
        }

        //gameObject.SetActive(false);
    }
}
