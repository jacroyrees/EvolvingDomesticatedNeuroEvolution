using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionDetection : MonoBehaviour
{

    public HooverAI hooverAI;

    public bool hasCollided = false;


    private void Start()
    {




    }
    void OnTriggerEnter(Collider other)
    {
        if (!hasCollided)
        {
            if (other.gameObject.tag == "Cleaner")
            {

                GetComponent<Renderer>().enabled = false;
                hasCollided = true;
                hooverAI.SetAreaCovered();
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    public bool GetCollided()
    {
        return hasCollided;
    }

    public void ResetRender()
    {
        hasCollided = false;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        hooverAI.ResetAreaCovered();
    }

}
