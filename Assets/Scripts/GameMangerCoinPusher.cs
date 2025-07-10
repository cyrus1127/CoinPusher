using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameMangerCoinPusher : GameManger
{
    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateChild()
    {
        base.UpdateChild();
    }

    /// =-=-=-=-=- functions -=-=-=-=-=-=

    public override void TouchPlaneOnTouched()
    {
        //Debug.Log("Func TouchPlaneOnTouched() called");
        if (total_coins > 0)
        {
            if (dropper)
            {
                total_coins--;
                dropper.DoDropCoins();
                PlayCommonClip("sfx-gacha-insert-coins");
            }
        }

        //do update
        UpdateCoinsVal();
    }
}
