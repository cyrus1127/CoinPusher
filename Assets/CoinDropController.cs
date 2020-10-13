using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDropController : MonoBehaviour
{
    public List<GameObject> objects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Obsolete]
    void DoDropObjectByIndex (int item_idx)
    {
        //Debug.Log("DoDropObjectByIndex[" + item_idx + "] ");
        if (item_idx < objects.Count){
            GameObject new_object = GameObject.Instantiate(objects[item_idx], this.transform.position, Quaternion.Euler(0,0,0));

            //Debug.Log("DoDropObjectByIndex[" + item_idx + "]  \n new_object ? " + new_object);
            new_object.transform.position = this.transform.position;
            new_object.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    [System.Obsolete]
    public void DoDropCoins()
    {
        //Debug.Log("DoDropCoins called");
        DoDropObjectByIndex(0);
    }

    [System.Obsolete]
    public void DoDropSepicalObject(int item_idx)
    {
        int idx = item_idx;
        if (idx < 0) {
            idx = Random.Range(1, objects.Count - 2);
        }
        if (idx > 0 && idx < objects.Count){
            DoDropObjectByIndex(idx);
        }
    }
}
