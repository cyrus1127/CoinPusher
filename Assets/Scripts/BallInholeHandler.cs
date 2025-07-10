using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallInholeHandler : MonoBehaviour
{
    public bool isNoPointArea;
    public GameManger manger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(9))
        {
            if (isNoPointArea)
            {
                //do nothing for score
                Destroy(other.gameObject);
            }
            else
            {
                manger.GetScore();
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(9)) {
            if (isNoPointArea)
            {
                //do nothing for score
                Destroy(collision.gameObject);
            }
            else {
                manger.GetScore();
            }
        }
    }
}
