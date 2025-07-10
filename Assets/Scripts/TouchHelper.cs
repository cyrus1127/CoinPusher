using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHelper : MonoBehaviour
{
    public GameManger manger;
    public CoinDropController theDropper; 
    private Ray ray;

    [SerializeField] bool isLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Obsolete]
    void OnMouseDown()
    {
        //Debug.Log("OnMouseDown - hit ");
        

        if (manger)
        {
            if (manger.gameObject.GetComponent<GameMangerCoinPusher>() != null)
            {
                // Create a particle if hit
                if (theDropper != null)
                {
                    theDropper.SetDroppingPointBy(GetTouchPointHorizontalDistance());
                }


                manger.TouchPlaneOnTouched();


            }
            else if (manger.gameObject.GetComponent<GameMangerBubbleUp>() != null)
            {
                bool isMoveUp = false;
                if (GetTouchPointVeritcalDistance() < 0) {
                    isMoveUp = true;
                }
                manger.gameObject.GetComponent<GameMangerBubbleUp>().TouchPlaneOnTouched(isLeft , isMoveUp);
            }
        }
    }

    private void GetRay() {
        if (Input.touches.Length > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                //Debug.Log("touch ? " + touch + " , fingerId ? " + touch.fingerId);
                if (touch.phase == TouchPhase.Ended)
                {
                    // Construct a ray from the current touch coordinates
                    ray = Camera.main.ScreenPointToRay(touch.position);
                    break;
                }
            }
        }
        else
        {
            //Debug.Log("Mouse ? " + Input.mousePosition);
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }


    /// =-=-=-=-=-=- touch point handling -=-=-=-=-=-=
    [Header("--- judgement ---")]
    [SerializeField] double padding = 0.08f;
    [SerializeField] double centerPoint = 0f;

    private float GetTouchPointHorizontalDistance()
    {
        GetRay();
        // Ray check
        if (Physics.Raycast(ray))
        {
              //this is the max position_x on the left , the right position is nagtive
            double padding_r = padding * 2;
            //padding_l = 0.15f;
            //padding_r = 0.3f;
            float dist = (float)((padding - (double)ray.direction.x) / padding_r);

            //Debug.Log("touch is here " + (double)ray.direction.x + " , % ? " + dist);
            //Debug.Log("touch is here " + ray.direction + " , % ? " + dist);
            //Debug.DrawRay(transform.position, ray.direction, Color.blue);

            return dist;
        }
        else
        {
            Debug.Log("Raycast by ray failed ");
        }

        return 0;
    }

    private float GetTouchPointVeritcalDistance()
    {
        GetRay();
        // Ray check
        if (Physics.Raycast(ray))
        {
            //this is the max position_x on the top , the right position is nagtive
            double padding_d = padding * 2;
            //padding_l = 0.15f;
            //padding_r = 0.3f;
            double centrePos = ((double)ray.direction.y + centerPoint);

            float dist = (float)((padding - centrePos) / padding_d);

            //Debug.Log("touch is here " + (double)ray.direction.y + " , % ? " + dist);
            Debug.Log($"touch-{ (isLeft? "L" : "R") } is here {centrePos} ,? {dist}%" );
            Debug.DrawRay(transform.position, ray.direction, Color.blue);

            return dist;
        }
        else
        {
            Debug.Log("Raycast by ray failed ");
        }

        return 0;
    }


}
