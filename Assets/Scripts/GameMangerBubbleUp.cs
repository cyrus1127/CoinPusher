using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameMangerBubbleUp : GameManger
{
    [Header("--- BubbleUp propers --- ")]
    [SerializeField] GameObject bar;
    [SerializeField] float moveSpeed = 2.0f;

    // Private
    float bar_left_pos = 0f;
    float bar_right_pos = 0f;
    Vector3 bar_pos_org = Vector3.zero;

    bool isGameStarted = false;
    GameObject ball = null;

    protected override void InitAtStart()
    {
        base.InitAtStart();
    }

    public void TouchPlaneOnTouched(bool isLeftHand , bool isMoveUp)
    {
        if (isGameStarted)
        {
            // TODO : move the bar
            if (isLeftHand)
            {
                bar_right_pos += (isMoveUp ? moveSpeed : -moveSpeed);
            }
            else
            { // do handle right hand side
                bar_left_pos += (isMoveUp ? moveSpeed : -moveSpeed);
            }

            if (bar != null)
            {
                if (bar_pos_org == Vector3.zero)
                {
                    bar_pos_org = bar.transform.localPosition;
                }

                float barWidth = 50f;
                //float rot = Mathf.Tan(Mathf.Abs(bar_left_pos - bar_right_pos) / barWidth) * (bar_left_pos - bar_right_pos > 0 ? 1 : -1);
                float rot = (Mathf.Abs(bar_left_pos - bar_right_pos) / barWidth) * 360 * (bar_left_pos - bar_right_pos > 0 ? 1 : -1);
                float pos_n = Mathf.Min(bar_left_pos, bar_right_pos) + Mathf.Abs(bar_left_pos - bar_right_pos) / 2f;

                Vector3 bar_pos_new = bar_pos_org + new Vector3(0, pos_n, 0);

                Debug.Log($" bL :{bar_left_pos}  bR : {bar_right_pos}  ;  R : {rot } , P : {pos_n}");

                //set new pos and rotation
                bar.transform.localPosition = bar_pos_new;
                bar.transform.localRotation = Quaternion.AngleAxis(rot, Vector3.forward);
            }
        }
        else {
            DropTheBall();
        }
        
        //Debug.Log("Func TouchPlaneOnTouched() called");
    }


    void DropTheBall() {
        if (dropper != null && !isGameStarted)
        {
            isGameStarted = true;
            ball = dropper.DoDropCoins();
        }
    }

    public void BallInTheHole() {
        isGameStarted = false;
        if (ball != null) {
            GameObject.DestroyImmediate(ball);
            ball = null;

            bar.transform.localPosition = bar_pos_org; // do reset
            bar.transform.localRotation = Quaternion.AngleAxis(0, Vector3.forward);
            bar_left_pos = 0f;
            bar_right_pos = 0f;
        }
    }

    public override void GetScore()
    {
        // TODO : game reset

        Debug.Log("ball hit");
    }
}
