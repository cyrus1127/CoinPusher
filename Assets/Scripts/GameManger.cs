using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameManger : CommonSceneViewManager
{

    //Canvas
    public Canvas myCanvas;

    //Board
    public CoinDropController dropper;
    public BonusDropController dropper_bonus;
    public Camera Camera;
    public GameObject TouchPlane;
    public GameObject txt_cash_val;
    public GameObject txt_total_coin_val;
    public GameObject txt_coinTimer_val;

    //Game Logic
    public float second_tofillCoins = 3f;  //3 sec
    protected float timeCounter_fillCoins;
    readonly protected int max_coins = 99999;
    readonly protected int init_coins = 100;
    readonly protected int score_special_1 = 10;
    readonly protected int score_special_2 = 100;
    protected int total_coins = 0;
    protected int total_score = 0;

    /// <summary>
    /// Life Cycle
    /// </summary>
    // Start is called before the first frame update

    protected override void InitAtStart()
    {
        base.InitAtStart();

        //set Canvas camera
        if (myCanvas)
        {
            myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        }

        total_coins = init_coins;
        timeCounter_fillCoins = second_tofillCoins;
        UpdateCoinsVal();

        Init();
    }
    
    protected virtual void Init()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimerCounter();
        UpdateCoinsVal();
        UpdateChild();
    }

    protected virtual void UpdateChild()
    {

    }

    /// <summary>
    // Funcitons
    /// </summary>

    protected void UpdateTimerCounter()
    {
        
        timeCounter_fillCoins -= Time.deltaTime;
        if (timeCounter_fillCoins <= 0)
        {
            if (total_coins < max_coins)
            {
                total_coins++;
            }

            timeCounter_fillCoins = second_tofillCoins;
        }

        int minute = (int)timeCounter_fillCoins / 60;
        int second = (int)timeCounter_fillCoins % 60;
        string minute_text = AutoFillZoreUnderHundred(minute); 
        string second_text = AutoFillZoreUnderHundred(second);

        txt_coinTimer_val.GetComponent<Text>().text = minute_text + ":" + second_text;
    }

    //Cyrus : for general use
    //will move for common use
    public string AutoFillZoreUnderHundred(int number)
    {
        string text = number.ToString();
        if (number < 10)
        {
            text = "0" + number.ToString();
        }
        return text;
    }

    protected void UpdateCoinsVal()
    {
        if (txt_total_coin_val)
        {
            txt_total_coin_val.GetComponent<Text>().text = total_coins.ToString();
        }
    }

    protected void UpdateScoreVal()
    {
        if (txt_cash_val)
        {
            txt_cash_val.GetComponent<Text>().text = total_score.ToString();
        }
    }

    public virtual void TouchPlaneOnTouched() {
       
    }

    
    public virtual void GetScore() {
        total_score++;
        UpdateScoreVal();
        //Debug.Log("GetScore : current score ? " + total_score);

        if (dropper)
        {
            if (total_score % score_special_1 == 0)
            {
                dropper.DoDropSepicalObject(1);
            }
            else if (total_score % score_special_2 == 0)
            {
                dropper.DoDropSepicalObject(-1); // Cyrus : drop radmon items
            }
        }
    }
}
