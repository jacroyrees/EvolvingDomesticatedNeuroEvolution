using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Species
{
    
    private List<Genome> GenomeList;
    private int Staleness;
    private System.Random random = new System.Random();
    private double TopFitness;
    private double TotalAdjustedFitness;
    private double AverageAdjustedFitness;
    
    
    public Species()
    {
        GenomeList = new List<Genome>();
        
        Staleness = 0;
        TotalAdjustedFitness = 0;
        AverageAdjustedFitness = 0;

    }

    

    public Species(Genome top)
    {
        GenomeList = new List<Genome>();

        Staleness = 0;
        TotalAdjustedFitness = 0;
        AverageAdjustedFitness = 0;
        this.GenomeList.Add(top);

    }
    public Genome GetTopGenome()
    {
        
        Debug.Log(GenomeList[0].GetFitness());
        return GenomeList[0];
        
    }

    public void SetTopFitness(double TopFitness)
    {
        this.TopFitness = TopFitness;
    }

    public double GetTopFitness()
    {
        return this.TopFitness;
    }
    public int GetStaleness()
    {
        return Staleness;
    }

    public void SetStaleness(int Staleness)
    {
        this.Staleness = Staleness;
    }

  

    public void SetGenomes(List<Genome> Genomes)
    {
        this.GenomeList = Genomes;
    }

    public List<Genome> GetGenomes()
    {
        return this.GenomeList;
    }


   

    public void CalculateGenomeAdjustedFitness()
    {
        foreach (Genome genome in GenomeList)
        {
            genome.SetAdjustedFitness(genome.GetFitness() / GenomeList.Count);
        }
    
    }


    public void CalculateTotalAdjustedFitness()
    {
        double TotalAdjustedFitness = 0;
        foreach (Genome genome in GenomeList)
        {

            TotalAdjustedFitness += genome.GetAdjustedFitness();
        }
        this.TotalAdjustedFitness = TotalAdjustedFitness;
       
    }
    public void CalculateAverageAdjustedFitness()
    {

        double AverageAdjustedFitness = 0;
        foreach (Genome genome in GenomeList)
        {
            AverageAdjustedFitness += genome.GetAdjustedFitness();
        }

        this.AverageAdjustedFitness  = AverageAdjustedFitness / GenomeList.Count;
    }




    public double GetAverageAdjustedFitness()
    {
        return this.AverageAdjustedFitness;
    }

    public double GetTotalAdjustedFitness()
    {
        return this.TotalAdjustedFitness;
    }


    public void RemoveWeakGenomes(bool AllButOne)
    {
        SortGenomes();
        int SurvivalCount = 1;
        if (!AllButOne)
        {
            SurvivalCount = (int)Math.Ceiling((GenomeList.Count) / 2f);
        }

        List<Genome> SurvivedGenomes = new List<Genome>();
        for (int i = GenomeList.Count; i > GenomeList.Count - SurvivalCount; i--)
        {
            SurvivedGenomes.Add(new Genome(GenomeList[i]));

        }
        GenomeList = SurvivedGenomes;
    }

    public void SortGenomes()
    {
        for (int i = 0; i < GenomeList.Count; i++)
        {
            for (int j = i; j < GenomeList.Count; j++)
            {
                if (GenomeList[i].GetAdjustedFitness() < GenomeList[j].GetAdjustedFitness())
                {
                    Genome temp = GenomeList[i];
                    GenomeList[i] = GenomeList[i];
                    GenomeList[j] = temp;
                }
            }
        }
        
    }


    public Genome BreedOffspring()
    {
        Genome child;
        if(random.NextDouble() < NEAT_CONFIGS.CROSSOVER_CHANCE){

            Genome g1 = GenomeList[random.Next(GenomeList.Count)];
            Genome g2 = GenomeList[random.Next(GenomeList.Count)];


            //Attempt to make it a different genome
            int ATTEMPTS = 0;
            while (g1 == g2 || ATTEMPTS < 10)
            {
                g1 = GenomeList[random.Next(GenomeList.Count)];
                ATTEMPTS++;
            }

            child = Genome.CrossOver(g1, g2);


        }
        else
        {
            Genome g1 = GenomeList[random.Next(GenomeList.Count)];
            child = g1;
        }

        child = new Genome(child);
        child.Mutate();
        return child;
    }

    
}