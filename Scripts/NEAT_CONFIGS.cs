using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEAT_CONFIGS
{
    public static int GLOBAL_INNOVATION_NUMBER = 20;

    public static double ADD_NODE_CHANCE = 00.03;

    public static double ADD_CONNECTION_CHANCE = 0.05;

    public static double ENABLE_CONNECTION_CHANCE = 0.1;

    public static double DISABLE_CONNECTION_CHANCE = 0.05;

    public static double MUTATE_WEIGHT_CHANCE = 0.9;

    public static double MUTATE_ANY_CHANCE = 0.8;

    public static int OUTPUTS = 2;

    public static int INPUTS = 10;


    public static double C1 = 1.0;

    public static double C2 = 1.0;

    public static double C3 = 0.6;

    public static double DELTA_THRESHOLD = 1.0;

    public static double CROSSOVER_CHANCE = 0.75;

    public static int INITIAL_POPULATION = 100;

    public static int STALE_SPECIES = 15;

    public static int STALE_POOL = 20;


}

   