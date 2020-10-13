using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{

    public CoinDropController dropper;
    public Camera Camera;
    public GameObject TouchPlane;
    public GameObject txt_cash_val;
    public GameObject txt_total_coin_val;

    public float second_tofillCoins = 3f;  //3 sec
    float timeCounter_fillCoins;
    readonly int max_coins = 99999;
    readonly int init_coins = 100;
    readonly int score_special_1 = 10;
    readonly int score_special_2 = 100;
    int total_coins = 0;
    int total_score = 0;

    // Start is called before the first frame update
    void Start()
    {
        total_coins = init_coins;
        UpdateCoinsVal();
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter_fillCoins += Time.deltaTime;
        if (timeCounter_fillCoins >= second_tofillCoins) {
            if (total_coins < max_coins)
            {
                total_coins++;
            }

            timeCounter_fillCoins = 0;
        }

        UpdateCoinsVal();
    }

    private void UpdateCoinsVal()
    {
        if (txt_total_coin_val)
        {
            txt_total_coin_val.GetComponent<Text>().text = total_coins.ToString();
        }
    }

    private void UpdateScoreVal()
    {
        if (txt_cash_val)
        {
            txt_cash_val.GetComponent<Text>().text = total_score.ToString();
        }
    }

    [System.Obsolete]
    public void TouchPlaneOnTouched() {
        //Debug.Log("Func TouchPlaneOnTouched() called");
        if (total_coins > 0) {
            if (dropper)
            {
                total_coins--;
                dropper.DoDropCoins();
            }
        }

        //do update
        UpdateCoinsVal();
    }

    [System.Obsolete]
    public void GetScore() {
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
