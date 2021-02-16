using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Genome))]
public class HooverAI : MonoBehaviour
{


    //The starting position and rotation
    private Vector3 startPosition, startRotation;
    private Genome network;
    Dictionary<(int, int), int> Places = new Dictionary<(int, int), int>();
    private int AreaCovered;
    private int PreviousAreaCovered;
    [Range(-1f, 1f)]
    public float a, t;
    public int i = 0;
    private int TimeSinceAreaCovered = 0;
    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float overallFitness;

    //How important multiplier is to the fitness evaluation
    public float distanceMultiplier = 1.2f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.5f;
    public float AreaMultiplier = 1.6f;


    [Header("Network Options")]
    public int NODES;
    public int CONNECTIONS;

    //Calculate fitnesses
    private Vector3 lastPosition;
    private float totalDistanceTravelled;

    private float avgSpeed;

    private float aSensor, bSensor, cSensor, dSensor, eSensor, fSensor, gSensor, hSensor, iSensor;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<Genome>();
        AreaCovered = 0;
        PreviousAreaCovered = 0;
        CONNECTIONS = network.GetConnectionGenes().Count;
        NODES = network.GetNodeGenes().Count;


    }
    public int GetAreaCovered()
    {
        return AreaCovered;
    }
    public void SetAreaCovered()
    {
        AreaCovered++;
    }

    public void ResetAreaCovered()
    {
        AreaCovered = 0;
        PreviousAreaCovered = 0;
    }

    public void SetPreviousAreaCovered(int n)
    {
        PreviousAreaCovered = n;
    }

    public int GetPreviousAreaCovered()
    {
        return PreviousAreaCovered;
    }
    public void ResetWithGenome(Genome net)
    {
        Places.Clear();
        network = net;
        Reset();
    }
    public void Reset()
    {
        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {



        Death();



    }


    private void FixedUpdate()
    {

        InputSensors();
        lastPosition = transform.position;

        float[] inputs = new float[9] { aSensor, bSensor, cSensor, dSensor, eSensor, fSensor, gSensor, hSensor, iSensor };
        (a, t) = network.EvaluateNetwork(inputs);
        

        MoveHoover(a, t);

        timeSinceStart += Time.deltaTime;

        calculateFitness();

        //a = 0;
        //t = 0;


    }

    private bool distanceCovered()
    {
        if (!Places.ContainsKey(((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.z))))
        {
            Places.Add(((int)Math.Round(transform.position.x, 0),
                (int)Math.Round(transform.position.z, 0)), i);
            i += 1;
            return true;
        }
        else
        {
            return false;
        }

    }

    private float MinMaxNormalisation(float val)
    {
        float min = 0.01f;
        float max = 120f;

        return (val - min) / (max - min);
    }
    private void InputSensors()
    {
        Vector3 DiagonalRight = (transform.forward + transform.right); //Diagnol right
        Vector3 Forward = (transform.forward); //Forward
        Vector3 DiagonalLeft = (transform.forward + transform.right * -1); //Diagnol left
        Vector3 BackwardsDiagonalRight = ((transform.forward * -1) + transform.right); //right 
        Vector3 Backwards = (transform.forward * -1); //Backward
        Vector3 BackwardsDiagonalLeft = ((transform.right * -1) - (transform.forward)); //Diagnol left back
        Vector3 DiagonalLeftDirty = (transform.forward + transform.right * -1) + (transform.up + new Vector3(0f, -1.2f, 0f));
        Vector3 ForwardDirty = (transform.forward) + (transform.up + new Vector3(0f, -1.2f, 0f));
        Vector3 DiagonalRightDirty = (transform.forward + transform.right + (transform.up + new Vector3(0f, -1.2f, 0f)));

        Ray r = new Ray(transform.position, DiagonalRight);
        Ray r_dirty = new Ray((transform.position + new Vector3(0f, 0.8f, 0f)), ForwardDirty);
        RaycastHit hit;


       
        if (Physics.Raycast(r, out hit))
        {
            aSensor = MinMaxNormalisation(hit.distance); //Divide by 20 due to normalisation (values between 0 and 1/-1 and 1)
                                    // print("A : " + aSensor);
            //Debug.DrawLine(r.origin, hit.point, Color.red);
            //Debug.Log(aSensor);
        }

        r.direction = Forward;

        if (Physics.Raycast(r, out hit))
        {
            bSensor = MinMaxNormalisation(hit.distance); //Divide by 20 due to normalisation (values between 0 and 1/-1 and 1)
                                                         //  print("B : " + bSensor);
            //Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = DiagonalLeft;

        if (Physics.Raycast(r, out hit))
        {
            cSensor = MinMaxNormalisation(hit.distance); //Divide by 20 due to normalisation (values between 0 and 1/-1 and 1)
                                                         // print("C : " + cSensor);
           // Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = BackwardsDiagonalRight;

        if (Physics.Raycast(r, out hit))
        {
            dSensor = MinMaxNormalisation(hit.distance);  //Divide by 20 due to normalisation (values between 0 and 1/-1 and 1)
                                                          //  print("D : " + dSensor);
          // Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = Backwards;

        if (Physics.Raycast(r, out hit))
        {
            eSensor = MinMaxNormalisation(hit.distance); //Divide by 20 due to normalisation (values between 0 and 1/-1 and 1)
                                                         //  print("E : " + eSensor);
           // Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = BackwardsDiagonalLeft;

        if (Physics.Raycast(r, out hit))
        {
            fSensor = MinMaxNormalisation(hit.distance); //Divide by 20 due to normalisation (values between 0 and 1/-1 and 1)
                                                         // print("f : " + fSensor);
            //Debug.DrawLine(r.origin, hit.point, Color.red);
        }




        if (Physics.Raycast(r_dirty, out hit))
        {
            if (hit.transform.tag.Equals("DirtyTile"))
            {
                if (hit.collider.gameObject.GetComponent<CollisionDetection>().GetCollided() != true)
                {
                    gSensor = MinMaxNormalisation(hit.distance);
                    Debug.DrawLine(r_dirty.origin, hit.point, Color.blue);
                }
                else
                {
                    Debug.DrawLine(r_dirty.origin, hit.point, Color.green);
                    gSensor = 1f;
                }
            }
            



            // print("f : " + fSensor);
           
        }
        




        r_dirty.direction = DiagonalLeftDirty;

        if (Physics.Raycast(r_dirty, out hit))
        {
            if (hit.transform.tag.Equals("DirtyTile"))
            {
                if (hit.collider.gameObject.GetComponent<CollisionDetection>().GetCollided() != true)
                {
                    hSensor = MinMaxNormalisation(hit.distance);
                    Debug.DrawLine(r_dirty.origin, hit.point, Color.blue);
                }
                else
                {
                    Debug.DrawLine(r_dirty.origin, hit.point, Color.green);
                    hSensor = 1f;
                }

            }
            





            // print("f : " + fSensor);

        }
        

        r_dirty.direction = DiagonalRightDirty;

        if (Physics.Raycast(r_dirty, out hit))
        {
            if (hit.transform.tag.Equals("DirtyTile"))
            {
                if (hit.collider.gameObject.GetComponent<CollisionDetection>().GetCollided() != true)
                {
                    iSensor = MinMaxNormalisation(hit.distance);
                    Debug.DrawLine(r_dirty.origin, hit.point, Color.blue);
                }
                else
                {
                    Debug.DrawLine(r_dirty.origin, hit.point, Color.green);
                    hSensor = 1f;
                }
            }
           




            // print("f : " + fSensor);

        }
        




    }




    //acceleration and rotation

    //Stores values from previous

    //acceleration and rotation
    private Vector3 input;
    public void MoveHoover(float v, float h)
    {

        //Lerp between vector3.zero and v*11.4 
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);


        //Take it from global z axis over to local z axis
        input = transform.TransformDirection(input);
        transform.position += input;

        //Percentage of how much we turn 90 degrees
        transform.eulerAngles += new Vector3(0, (h * 90) * 0.02f, 0);
    }



    
    private void calculateFitness()
    {
        int CurrentAreaCovered = GetAreaCovered() - GetPreviousAreaCovered();
        
        
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;
        if (GetAreaCovered() == GetPreviousAreaCovered())
        {
            TimeSinceAreaCovered++;
        }
        else
        {
            TimeSinceAreaCovered = 0;
        }


        float SensorCalculation = (1f - (gSensor + hSensor + iSensor)/3f) * 5f;
        
        overallFitness = ((GetAreaCovered()*distanceMultiplier)/2f)  + (((aSensor+bSensor+cSensor+dSensor+eSensor+fSensor)/6)* sensorMultiplier) + (CurrentAreaCovered * AreaMultiplier) + SensorCalculation;

        SetPreviousAreaCovered(GetAreaCovered());


        if (timeSinceStart > 20 && overallFitness < 100)
        {
            Death();
        }

        
        if(TimeSinceAreaCovered > 200)
        {
            Death();
        }
        
        //Really Good Car
        if (timeSinceStart >= 1000)
        {
            //Save network
            Death();

        }
    }

    private void Death()
    {
        GameObject.FindObjectOfType<PoolManager>().Death(overallFitness, network);
    }

}
