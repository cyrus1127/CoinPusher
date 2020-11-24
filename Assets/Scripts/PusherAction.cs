using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherAction : MonoBehaviour
{
    public bool isMoveToward = false;
    private bool isDirectionChanged = false;
    public float moveSpeed = 0.01f;
    private float startPoint = 0f;
    public float moveDistance ;
    public float endPoint;
    public float endPoint_bonus;
    private Vector3 newMove;
    

    public Vector3 NewMove { get => newMove; set => newMove = value; }

    // Start is called before the first frame update
    void Start()
    {
        newMove = gameObject.transform.position;
        newMove.z = 0;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(UpdateMyPosition());
    }

    IEnumerator UpdateMyPosition()
    {

        if(isDirectionChanged)
        {
            //Debug.Log("Started Coroutine at timestamp : " + Time.time);
            yield return new WaitForSeconds(1f);
            isDirectionChanged = false;
        }
        else
        {
            MoveByEndPoint(endPoint);
        }
    }

    public void DoBonusPush()
    {
        MoveByEndPoint(endPoint_bonus);
    }


    private void MoveByEndPoint(float in_endPoint)
    {
        float moveSpeedInDeltaTime = moveSpeed * Time.deltaTime;
        //Debug.Log("No need to wait : ");

        if (isMoveToward)
        { // to forward
            newMove.z += moveSpeedInDeltaTime;
            if (NewMove.z - startPoint >= moveDistance)
            {
                //Debug.Log("case [" + isMoveToward + "] : switch (" + moveDistance + ") -> " + NewMove.z);
                isMoveToward = false;
                //newMove.z = moveDistance;
                isDirectionChanged = true;
            }
        }
        else
        {
            newMove.z -= moveSpeedInDeltaTime;
            if (NewMove.z - startPoint <= in_endPoint)
            {
                //Debug.Log("case [" + isMoveToward + "] : switch (" + moveDistance + ") -> " + NewMove.z);
                isMoveToward = true;
                //newMove.z = endPoint;
                isDirectionChanged = true;

            }
        }

        //Cyrus：set new 
        gameObject.transform.position = NewMove;
    }
}
