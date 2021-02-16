using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System;
using System.Linq;

public class Genome : MonoBehaviour
{
    private double fitness;
    private double adjustedFitness;
    private Dictionary<int, NodeGene> NodeGenes = new Dictionary<int, NodeGene>();
    private List<ConnectionGene> ConnectionGenes = new List<ConnectionGene>();
    private System.Random random = new System.Random();
    public Genome()
    {
        InitialNetwork();
    }

    public Genome(Genome child)
    {
        this.NodeGenes = child.GetNodeGenes();
        this.ConnectionGenes = child.GetConnectionGenes();
    }

    public void InitialNetwork()
    {
        //INPUT NODES -> VALUE = 0
        NodeGenes.Add(1, new NodeGene(1, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(2, new NodeGene(2, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(3, new NodeGene(3, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(4, new NodeGene(4, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(5, new NodeGene(5, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(6, new NodeGene(6, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(7, new NodeGene(7, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(8, new NodeGene(8, NodeGene.TYPE.INPUT, 0));
        NodeGenes.Add(9, new NodeGene(9, NodeGene.TYPE.INPUT, 0));
        //BIAS NODE -> VALUE = 1
        NodeGenes.Add(10, new NodeGene(10, NodeGene.TYPE.BIAS, 1)); //Bias


        //OUTPUT NODE -> VALUE = 0
        NodeGenes.Add(11, new NodeGene(11, NodeGene.TYPE.OUTPUT, 0));
        NodeGenes.Add(12, new NodeGene(12, NodeGene.TYPE.OUTPUT, 0));

        int j = 1;
        for (int i = 1; i < 11; i++)
        {
            ConnectionGenes.Add(new ConnectionGene(i, 11, random.NextDouble(), true, j));
            
            ConnectionGenes.Add(new ConnectionGene(i, 12, random.NextDouble(), true, j + 1));
            j = j + 2;


        }

        foreach(ConnectionGene connection in GetConnectionGenes())
        {
            NodeGene node = GetNodeGenes()[connection.GetOutputNode()];

            if(!node.GetIncomingConnections().Contains(connection))
            {
                node.AddIncomingConnection(connection);
            }
        }
        


        

    }



    public (float, float) EvaluateNetwork(float[] input)
    {
        
        float[] output = new float[NEAT_CONFIGS.OUTPUTS];
        for(int i = 1; i < NEAT_CONFIGS.INPUTS; i++)
        {
            GetNodeGenes()[i].SetValue(input[i - 1]);
        }


        foreach(var KeyValue in GetNodeGenes()){
            double sum = 0;
            int key = KeyValue.Key;
            NodeGene node = KeyValue.Value;
            if(key > NEAT_CONFIGS.INPUTS)
            {
                foreach(ConnectionGene con in node.GetIncomingConnections())
                {
                    if (con.GetEnabled())
                    {
                        sum +=(GetNodeGenes()[con.GetInputNode()].GetValue() * con.GetWeight());
                    }
                }
                
                node.SetValue(Math.Tanh(sum));
            }
        }
        int j = 0;
        foreach(NodeGene node in GetNodeGenes().Values)
        {
            if(node.GetType() == NodeGene.TYPE.OUTPUT)
            {
                output[j] = (float)node.GetValue();
                j++;
            }
        }
        return (output[0], output[1]);
    }

    public void Mutate()
    {
        if (random.NextDouble() < NEAT_CONFIGS.ADD_NODE_CHANCE)
        {
            MutateAddNode();
        }
        if (random.NextDouble() < NEAT_CONFIGS.ADD_CONNECTION_CHANCE)
        {
            MutateAddConnection();
        }

        foreach(ConnectionGene con in GetConnectionGenes()) {
            if (random.NextDouble() < NEAT_CONFIGS.ENABLE_CONNECTION_CHANCE)
            {
                EnableConnection(con);
            }
            if (random.NextDouble() < NEAT_CONFIGS.DISABLE_CONNECTION_CHANCE)
            {
                DisableConnection(con);
            }
            if (random.NextDouble() < NEAT_CONFIGS.MUTATE_ANY_CHANCE)
            {
                if (random.NextDouble() < NEAT_CONFIGS.MUTATE_WEIGHT_CHANCE)
                {
                    MutateWeight(con);
                }
                else
                {
                    MutateWeightRandomly(con);
                }
                
            }
        }
    }

    public void MutateAddNode()
    {
        ConnectionGene randomConnection = GetConnectionGenes()[random.Next(GetConnectionGenes().Count)];

        NodeGene InNode = GetNodeGenes()[randomConnection.GetInputNode()];
        NodeGene OutNode = GetNodeGenes()[randomConnection.GetOutputNode()];


        randomConnection.SetEnabled(false);

        NodeGene MiddleNode = new NodeGene(GetNodeGenes().Count + 1, NodeGene.TYPE.HIDDEN, 0);

        ConnectionGene InputToMiddle = new ConnectionGene(InNode.GetID(), MiddleNode.GetID(), 1, true, NEAT_CONFIGS.GLOBAL_INNOVATION_NUMBER++);
        ConnectionGenes.Add(InputToMiddle);
        MiddleNode.AddIncomingConnection(InputToMiddle);
        ConnectionGene MiddleToOutput = new ConnectionGene(MiddleNode.GetID(), OutNode.GetID(), randomConnection.GetWeight(), true, NEAT_CONFIGS.GLOBAL_INNOVATION_NUMBER++);
        ConnectionGenes.Add(MiddleToOutput);
        OutNode.AddIncomingConnection(MiddleToOutput);
        //Add to the total current Pool Connections Made in this instance
        NodeGenes.Add(MiddleNode.GetID(), MiddleNode);



    }

    public void MutateAddConnection()
    {
        List<int> Keys = new List<int>(NodeGenes.Keys);

        int randomKey1 = Keys[random.Next(Keys.Count)];
        int randomKey2 = Keys[random.Next(Keys.Count)];


        NodeGene node1 = NodeGenes[randomKey1];
        NodeGene node2 = NodeGenes[randomKey2];
        int ATTEMPTS = 0;
        while ((node1.GetType() == NodeGene.TYPE.OUTPUT && node2.GetType() == NodeGene.TYPE.OUTPUT || node1.GetType() == NodeGene.TYPE.INPUT && node2.GetType() == NodeGene.TYPE.INPUT) || node1.GetID() == node2.GetID() ||
            (node1.GetType() == NodeGene.TYPE.BIAS && node2.GetType() == NodeGene.TYPE.INPUT || node2.GetType() == NodeGene.TYPE.BIAS && node1.GetType() == NodeGene.TYPE.INPUT) || ATTEMPTS < 10)
        {
            randomKey1 = Keys[random.Next(Keys.Count)];
            node1 = NodeGenes[randomKey1];
            ATTEMPTS++;
        }

        if (ATTEMPTS < 10)
        {

            bool NeedsReversing = false;


            if ((node1.GetType() == NodeGene.TYPE.HIDDEN && node2.GetType() == NodeGene.TYPE.INPUT) || (node1.GetType() == NodeGene.TYPE.HIDDEN && node2.GetType() == NodeGene.TYPE.BIAS))
            {
                NeedsReversing = true;
            }
            else
            {
                if (node1.GetType() == NodeGene.TYPE.OUTPUT && node2.GetType() == NodeGene.TYPE.INPUT || node1.GetType() == NodeGene.TYPE.OUTPUT && node2.GetType() == NodeGene.TYPE.BIAS || node1.GetType() == NodeGene.TYPE.OUTPUT && node2.GetType() == NodeGene.TYPE.HIDDEN)
                {
                    NeedsReversing = true;
                }

            }


            bool ExistsInGenome = false;

            foreach (ConnectionGene connection in GetConnectionGenes())
            {
                if (connection.GetInputNode() == node1.GetID() && connection.GetOutputNode() == node2.GetID())
                {
                    ExistsInGenome = true;
                    break;
                }
                else
                {
                    if (connection.GetOutputNode() == node1.GetID() && connection.GetInputNode() == node2.GetID())
                    {
                        ExistsInGenome = true;
                    }
                }
            }


            if (!ExistsInGenome)
            {
                bool ExistsInPopulation = false;
                foreach (ConnectionGene connection in PoolManager.TOTALCONNECTIONSMADETHISGENERATION)
                {
                    if (connection.GetInputNode() == node1.GetID() && connection.GetOutputNode() == node2.GetID() || connection.GetOutputNode() == node1.GetID() && connection.GetInputNode() == node2.GetID())
                    {
                        ExistsInPopulation = true;
                        ConnectionGene newConnection = new ConnectionGene(NeedsReversing ? node2.GetID() : node1.GetID(), NeedsReversing ? node1.GetID() : node2.GetID(), random.NextDouble(), true, connection.GetInnovation());
                        ConnectionGenes.Add(newConnection);
                        GetNodeGenes()[newConnection.GetOutputNode()].AddIncomingConnection(newConnection);
                        break;
                    }


                }
                if (!ExistsInPopulation)
                {
                    ConnectionGene newConnection = new ConnectionGene(NeedsReversing ? node2.GetID() : node1.GetID(), NeedsReversing ? node1.GetID() : node2.GetID(), random.NextDouble(), true, NEAT_CONFIGS.GLOBAL_INNOVATION_NUMBER++);
                    ConnectionGenes.Add(newConnection);
                    GetNodeGenes()[newConnection.GetOutputNode()].AddIncomingConnection(newConnection);
                    PoolManager.TOTALCONNECTIONSMADETHISGENERATION.Add(newConnection);
                }

            }

        }
    }

    public void EnableConnection(ConnectionGene gene)
    {

        gene.SetEnabled(true);
    }

    public void DisableConnection(ConnectionGene gene)
    {

        gene.SetEnabled(false);

    }


    public void MutateWeight(ConnectionGene gene)
    {
        double randomValueUpDown = random.NextDouble() * (0.1 - 0.01) + 0.01;
        bool UpDown;
        if (random.NextDouble() > 0.5)
        {
            UpDown = true;
        }
        else
        {
            UpDown = false;
        }

        gene.SetWeight(gene.GetWeight() + (randomValueUpDown * (UpDown ? 1 : -1)));

    }

    public void MutateWeightRandomly(ConnectionGene gene)
    {
        gene.SetWeight(random.NextDouble());
    }

    public static Genome CrossOver(Genome Parent1, Genome Parent2)
    {
        System.Random random = new System.Random();
        if (Parent1.GetAdjustedFitness() < Parent2.GetAdjustedFitness())
        {
            Genome temp = Parent1;
            Parent2 = Parent1;
            Parent1 = temp;
        }
        Genome child = new Genome();

        Dictionary<int, ConnectionGene> Parent1Genes = new Dictionary<int, ConnectionGene>();
        Dictionary<int, ConnectionGene> Parent2Genes = new Dictionary<int, ConnectionGene>();

        foreach (ConnectionGene connection in Parent1.GetConnectionGenes())
        {
            Parent1Genes.Add(connection.GetInnovation(), connection);
        }

        foreach (ConnectionGene connection in Parent2.GetConnectionGenes())
        {
            Parent2Genes.Add(connection.GetInnovation(), connection);
        }

        HashSet<int> Parent1Key = new HashSet<int>(Parent1Genes.Keys);
        HashSet<int> Parent2Key = new HashSet<int>(Parent2Genes.Keys);
        HashSet<int> AllKeys = new HashSet<int>(Parent1Key);
        AllKeys.UnionWith(Parent2Key);

        foreach (int key in AllKeys)
        {
            ConnectionGene trait;
            if (Parent1Genes.ContainsKey(key) && Parent2Genes.ContainsKey(key))
            {
                if (random.NextDouble() > 0.5)
                {
                    trait = new ConnectionGene(Parent1Genes[key]);
                }
                else
                {
                    trait = new ConnectionGene(Parent2Genes[key]);
                }

                if (Parent1Genes[key].GetEnabled() != Parent2Genes[key].GetEnabled())
                {

                    //If the gene is enabled in one genome but disabled in another there is a 75% chance we take it from the better genome
                    if (random.NextDouble() < 0.75)
                    {
                        trait.SetEnabled(Parent1Genes[key].GetEnabled());
                    }
                    else
                    {
                        trait.SetEnabled(Parent2Genes[key].GetEnabled());
                    }
                }
                else
                {
                    if (Parent1.GetAdjustedFitness() == Parent2.GetAdjustedFitness())
                    {
                        if (Parent1Genes.ContainsKey(key))
                        {
                            trait = Parent1Genes[key];
                        }
                        else
                        {
                            trait = Parent2Genes[key];
                        }

                        //Only want to take half of the excess/Disjoint genes
                        if (random.NextDouble() > 0.5)
                        {
                            continue;
                        }

                    }

                }

                child.GetConnectionGenes().Add(trait);
            }

        }


        //NODES
        foreach (ConnectionGene con in child.GetConnectionGenes())
        {
            


                if (!child.GetNodeGenes().ContainsKey(con.GetInputNode()))
                {
                    child.GetNodeGenes().Add(con.GetInputNode(), new NodeGene(con.GetInputNode(), NodeGene.TYPE.HIDDEN, 0));
                }
                if (!child.GetNodeGenes().ContainsKey(con.GetOutputNode()))
                {
                    child.GetNodeGenes().Add(con.GetOutputNode(), new NodeGene(con.GetOutputNode(), NodeGene.TYPE.HIDDEN, 0));
                }
                child.GetNodeGenes()[con.GetOutputNode()].AddIncomingConnection(con);
            
            
        }



        return child;
    }





    public static bool SameSpecies(Genome Parent1, Genome Parent2)
    {

        int matching = 0;
        int disjoint = 0;
        int excess = 0;
        double weight = 0;
        int lowMaxInnovation;
        double delta;
        int N;
        Dictionary<int, ConnectionGene> Parent1Genes = new Dictionary<int, ConnectionGene>();
        Dictionary<int, ConnectionGene> Parent2Genes = new Dictionary<int, ConnectionGene>();

        foreach(ConnectionGene con in Parent1.GetConnectionGenes())
        {
            Parent1Genes.Add(con.GetInnovation(), con);
        }

        foreach(ConnectionGene con in Parent2.GetConnectionGenes())
        {
            Parent2Genes.Add(con.GetInnovation(), con);
        }

        if(!Parent1Genes.Any() || !Parent2Genes.Any())
        {
            lowMaxInnovation = 0;
        }
        else
        {
            lowMaxInnovation = Math.Min(Parent1Genes.Keys.Max(), Parent2Genes.Keys.Max());
        }

        HashSet<int> InnovationP1 = new HashSet<int>(Parent1Genes.Keys);
        HashSet<int> InnovationP2 = new HashSet<int>(Parent2Genes.Keys);

        HashSet<int> AllInnovations = new HashSet<int>(InnovationP1);
        AllInnovations.UnionWith(InnovationP2);

        foreach(int key in AllInnovations)
        {
            if(Parent1Genes.ContainsKey(key) && Parent2Genes.ContainsKey(key))
            {
                matching++;
                weight += Math.Abs(Parent1Genes[key].GetWeight() - Parent2Genes[key].GetWeight());
            }
            else
            {
                if (key < lowMaxInnovation)
                {
                    disjoint++;
                }
                else
                {
                    excess++;
                }
            }


        }

        if(AllInnovations.Max() < 20)
        {
            N = 1;
        }
        else
        {
            N = Math.Max(Parent1Genes.Keys.Max(), Parent2Genes.Keys.Max());
        }

        delta = ((NEAT_CONFIGS.C1 * excess)/N) + ((NEAT_CONFIGS.C2 * disjoint)/N) + NEAT_CONFIGS.C3 * weight;

        return delta < NEAT_CONFIGS.DELTA_THRESHOLD;
    }


    public void SetFitness(double fitness)
    {
        this.fitness = fitness;
    }

    public double GetFitness()
    {
        return this.fitness;
    }

    public void SetAdjustedFitness(double adjustedFitness)
    {
        this.adjustedFitness = adjustedFitness;

    }

    public double GetAdjustedFitness()
    {
        return this.adjustedFitness;
    }


    public Dictionary<int, NodeGene> GetNodeGenes()
    {
        return this.NodeGenes;
    }

    public List<ConnectionGene> GetConnectionGenes()
    {
        return this.ConnectionGenes;
    }

    public void SetnodeGenes(Dictionary<int, NodeGene> NodeGenes)
    {
        this.NodeGenes = NodeGenes;
    }

    public void SetConnectionGenes(List<ConnectionGene> ConnectionGenes)
    {
        this.ConnectionGenes = ConnectionGenes;
    }



}
