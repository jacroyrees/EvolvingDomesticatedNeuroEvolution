using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

public class ConnectionGene
{

    private int InputNode;
    private int OutputNode;
    private double Weight;
    private bool IsEnabled;
    private int Innovation;


    public ConnectionGene(int Input, int Output,  double Weight, bool IsEnabled, int Innovation)
    {
        this.InputNode = Input;
        this.OutputNode = Output; ;
        this.Innovation = Innovation;
        this.Weight = Weight;
        this.IsEnabled = IsEnabled;
    }



    public ConnectionGene(ConnectionGene Connection)
    {
        if (Connection != null)
        {
            this.InputNode = Connection.GetInputNode();
            this.OutputNode = Connection.GetOutputNode();
            this.Weight = Connection.GetWeight();
            this.IsEnabled = Connection.GetEnabled();
            this.Innovation = Connection.GetInnovation();
        }
    }


    public int GetInputNode()
    {
        return this.InputNode;
    }

    public int GetOutputNode()
    {
        return this.OutputNode;
    }

    public double GetWeight()
    {
        return this.Weight;
    }

    public bool GetEnabled()
    {
        return this.IsEnabled;
    }

    public int GetInnovation()
    {
        return this.Innovation;
    }

    public void SetInputNode(int node)
    {
        this.InputNode = node;
    }

    public void SetOutputNode(int node)
    {
        this.OutputNode = node;
    }

    public void SetWeight(double weight)
    {
        this.Weight = weight;
    }

    public void SetEnabled(bool enabled)
    {
        this.IsEnabled = enabled;
    }

    public void SetInnovation(int Innovation)
    {
        this.Innovation = Innovation;
    }





}
