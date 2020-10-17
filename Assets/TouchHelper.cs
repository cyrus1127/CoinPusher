using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHelper : MonoBehaviour
{
    public GameManger manger;
    public CoinDropController theDropper; 
    private Ray ray;

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
        GetTouchPoint();

        if (manger)
        {
            manger.TouchPlaneOnTouched();
        }
    }


    [System.Obsolete]
    void GetTouchPoint()
    {
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

        // Ray check
        if (Physics.Raycast(ray))
        {
            float dist = (float)(((double)0.15f - (double)ray.direction.x) / (double)0.3f);

            // Create a particle if hit
            if (theDropper)
            {
                theDropper.SetDroppingPointBy(dist);
            }

            Debug.Log("touch is here " + ray.direction + " , % ? " + dist);
            //Debug.DrawRay(transform.position, ray.direction, Color.blue);
        }
        else
        {
            Debug.Log("Raycast by ray failed ");
        }
    }


}
