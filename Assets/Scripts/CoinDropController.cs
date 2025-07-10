using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDropController : MonoBehaviour
{
    public List<GameObject> objects;
    public BonusDropController bonusDrop;
    public float max_left;
    public float max_right;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    GameObject DoDropObjectByIndex (int item_idx)
    {
        GameObject new_object = null;
        //Debug.Log("DoDropObjectByIndex[" + item_idx + "] ");
        if (item_idx < objects.Count)
        {
            new_object = GameObject.Instantiate(objects[item_idx], this.transform.position, Quaternion.Euler(0,0,0));

            //Debug.Log("DoDropObjectByIndex[" + item_idx + "]  \n new_object ? " + new_object);
            new_object.transform.position = this.transform.position;
            new_object.GetComponent<Rigidbody>().isKinematic = false;
        }

        return new_object;
    }

    public GameObject DoDropCoins()
    {
        //Debug.Log("DoDropCoins called");
        return DoDropObjectByIndex(0);
    }

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

    public void SetDroppingPointBy(float inParcentage)
    {
        float total_dist = max_right - max_left;
        float move_dist = max_right - (total_dist * inParcentage);
        Vector3 current_pos = this.transform.position;
        Debug.Log("move_dist ? " + move_dist);
        transform.position = new Vector3(move_dist, current_pos.y, current_pos.z);
    }
}