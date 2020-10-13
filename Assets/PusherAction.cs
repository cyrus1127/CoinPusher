using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherAction : MonoBehaviour
{
    public bool isMoveToward = false;
    public float moveSpeed = 0.01f;
    private float startPoint = 0f;
    public float moveDistance ;
    public float endPoint;
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

        float moveSpeedInDeltaTime = moveSpeed * Time.deltaTime;

        if (isMoveToward){ // to forward
            newMove.z += moveSpeedInDeltaTime;
            if (NewMove.z - startPoint >= moveDistance)
            {
                //Debug.Log("case [" + isMoveToward + "] : switch (" + moveDistance + ") -> " + NewMove.z);
                isMoveToward = false;
                //newMove.z = moveDistance;
            }
        } else {
            newMove.z -= moveSpeedInDeltaTime;
            if (NewMove.z - startPoint <= endPoint)
            {
                //Debug.Log("case [" + isMoveToward + "] : switch (" + moveDistance + ") -> " + NewMove.z);
                isMoveToward = true;
                //newMove.z = endPoint;
            }
        }

        //Cyrus：set new 
        gameObject.transform.position = NewMove;
    }
}
