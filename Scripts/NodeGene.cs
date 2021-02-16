using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NodeGene
{

    private double value;
    private int ID;
    private List<ConnectionGene> IncomingConnections = new List<ConnectionGene>();
    private TYPE type;

    public enum TYPE
    {
        INPUT,
        OUTPUT,
        HIDDEN,
        BIAS
    }
    public NodeGene(int ID, TYPE type, float value)
    {
        this.value = value;
        this.ID = ID;
        this.type = type;
    }

    public int GetID()
    {
        return this.ID;
    }

    public void SetID(int ID)
    {
        this.ID = ID;
    }


    public void AddIncomingConnection(ConnectionGene connection)
    {
        this.IncomingConnections.Add(connection);
    }
    public double GetValue()
    {
        return value;
    }

    public void SetValue(double val)
    {
        this.value = val;
    }

    public void SetType(TYPE type)
    {
        this.type = type;
    }

    public TYPE GetType()
    {
        return this.type;
    }
    public void SetIncomingConnection(List<ConnectionGene> IncomingConnections)
    {
        this.IncomingConnections = IncomingConnections;
    }

    public List<ConnectionGene> GetIncomingConnections()
    {
        return IncomingConnections;
    }


}
