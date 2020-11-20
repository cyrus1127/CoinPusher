using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusDropController : MonoBehaviour
{
    public float max_width;
    public float max_lenght;
   
    public bool isTriggered;
    private int dropCount;
    private float drop_period;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void DropBonusSetBy(List<GameObject> objects, int index)
    {
        
        switch (index) {
            case 0:
                dropCount = 10;
                drop_period = 5f;
                break;
            case 1:
                dropCount = 20;
                drop_period = 5f;
                break;
            default:
                dropCount = 0;
                drop_period = 5f;
                break;
        }

        float pos_x = Random.Range(transform.position.x, max_width);
        float pos_z = Random.Range(transform.position.z, max_lenght);

    }
}
