using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    public double GlobalTotalAdjustedFitness;
    public double GlobalAverageAdjustedFitness;

    public static List<ConnectionGene> TOTALCONNECTIONSMADETHISGENERATION = new List<ConnectionGene>();
    private List<Species> SpeciesList = new List<Species>();
    [Header("References")]
    public HooverAI controller;

    public FloorRenderrer renderrer;
    [Header("Controls")]


    [Header("Crossover Controls")]

    

    private Genome[] population;
    private int GLOBALPOPULATION;
    [Header("Public View")]
    public int CurrentGeneration = 0;
    public int CurrentGenome = 0;

    private void Start()
    {
        
        InitialisePool();
        population = PopulationAsArray(TotalPopulation(SpeciesList), SpeciesList);
        ResetToCurrentGenome();
        
        
    }



    public Genome[] PopulationAsArray(int POPULATION, List<Species> Species)
    {
        

        Genome[] newPopulation = new Genome[POPULATION];
        int i = 0;
        foreach (Species species in Species)
        {

            foreach (Genome genome in species.GetGenomes())
            {

                //Debug.Log(newPopulation.Length);

                newPopulation[i] = genome;
                i++;
                //Debug.Log(i);
            }
        }

        return newPopulation;
    }

    public int TotalPopulation(List<Species> NewSpeciesList)
    {
        int TotalPopulation = 0;
        foreach(Species species in NewSpeciesList)
        {
            TotalPopulation += species.GetGenomes().Count;
        }

        return TotalPopulation;
    }
    private void ResetToCurrentGenome()
    {
        renderrer.ResetAll();
        controller.ResetWithGenome(population[CurrentGenome]);
    }

    public void InitialisePool()
    {
        SpeciesList.Add(new Species());
        for (int i = 0; i < NEAT_CONFIGS.INITIAL_POPULATION; i++)
        {
            SpeciesList[0].GetGenomes().Add(new Genome());
        }
    }



    public void Death(float fitness, Genome genome)
    {

        if (CurrentGenome < population.Length - 1)
        {

            population[CurrentGenome].SetFitness(fitness);
            CurrentGenome++;
            ResetToCurrentGenome();

        }
        else
        {
            BreedNewGeneration();
        }

    }

    private double ChildrenAllocated(double TotalFitness)
    {

        double ChildrenAllocated = NEAT_CONFIGS.INITIAL_POPULATION * (TotalFitness / GlobalTotalAdjustedFitness);
        return ChildrenAllocated;
    }

    private void BreedNewGeneration()
    {
        List<Genome> children = new List<Genome>();
        List<Species> survived = new List<Species>();
        SpeciesCalculations();
        CurrentGeneration++;
        double DoubleChildAllocated = 100;
        double CarryOver = 0;
        
        if (CurrentGeneration > 1)
        {
            RemoveStaleSpecies();
        }
        foreach (Species species in SpeciesList)
        {

            DoubleChildAllocated = ChildrenAllocated(species.GetTotalAdjustedFitness());

            int NChild = (int)DoubleChildAllocated;
            CarryOver += DoubleChildAllocated - NChild;

            if (CarryOver > 1)
            {
                NChild++;
                CarryOver--;
            }
            if (NChild < 1)
            {
                continue;
            }


            survived.Add(new Species(species.GetTopGenome()));
            if(NChild > 100)
            {
                NChild = 10;
            }
            for (int i = 0; i < NChild; i++)
            {

                Genome child = species.BreedOffspring();
                children.Add(child);

            }


            SpeciesList.Clear();
            SpeciesList = survived;
            foreach(Genome child in children)
            {
                AssignToSpecies(child);
            }

            CurrentGenome = 0;
            population = PopulationAsArray(TotalPopulation(SpeciesList), SpeciesList);
            ResetToCurrentGenome();
        }



















        CurrentGenome = 0;

        ResetToCurrentGenome();

    }

    public void RemoveStaleSpecies()
    {
        foreach (Species species in SpeciesList)
        {

            if (species.GetGenomes().Count == 0)
            {
                SpeciesList.Remove(species);
            }

            if (species.GetAverageAdjustedFitness() < GlobalAverageAdjustedFitness)
            {
                species.SetStaleness(species.GetStaleness() + 1);
            }
            else
            {
                species.SetStaleness(0);
            }

            if (species.GetStaleness() > NEAT_CONFIGS.STALE_SPECIES)
            {
                species.RemoveWeakGenomes(true);
                continue;
            }

            species.RemoveWeakGenomes(false);

        }
    }



    public void SpeciesCalculations()
    {
        double GlobalTotalAdjustedFitness = 0;
        double GlobalAverageAdjustedFitness = 0;
        foreach (Species species in SpeciesList)
        {

            species.CalculateGenomeAdjustedFitness();
            species.CalculateTotalAdjustedFitness();
            GlobalTotalAdjustedFitness += species.GetTotalAdjustedFitness();
            species.CalculateAverageAdjustedFitness();
            GlobalAverageAdjustedFitness += species.GetAverageAdjustedFitness();

        }

        this.GlobalTotalAdjustedFitness = GlobalTotalAdjustedFitness;
        this.GlobalAverageAdjustedFitness = GlobalAverageAdjustedFitness / this.SpeciesList.Count;
    }



    private void AssignToSpecies(Genome g)
    {
        foreach(Species species in SpeciesList)
        {
            if (species.GetGenomes().Count == 0)
            {
                SpeciesList.Remove(species);
                continue;

            }


            Genome g1 = species.GetGenomes()[0];
            
            if(Genome.SameSpecies(g, g1))
            {
                species.GetGenomes().Add(g);
                return;
            }




        }


        Species newSpecies = new Species();
        newSpecies.GetGenomes().Add(g);
        SpeciesList.Add(newSpecies);
    }

}


