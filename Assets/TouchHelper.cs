using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHelper : MonoBehaviour
{
    public GameManger manger;

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
        if (manger) {
            manger.TouchPlaneOnTouched();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter - hit ?? " + collision);
    }


}
