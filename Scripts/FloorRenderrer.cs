using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class FloorRenderrer : MonoBehaviour
{
    private GameObject[] Children;
    //public CollisionDetection detection;
    // Start is called before the first frame update


    void Awake()
    {

        Children = GameObject.FindGameObjectsWithTag("DirtyTile");

    }


    // Update is called once per frame
    public int GetChildren()
    {
        return Children.Length;
    }



    public void ResetAll()
    {
        for (int i = 0; i < Children.Length; i++)

        {
            Children[i].GetComponent<CollisionDetection>().ResetRender();
        }
    }
}
