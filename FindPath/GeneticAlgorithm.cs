using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GeneticAlgorithm<T>
    {
        Func<T[], float> evalFitness;
        Func<T> randomGene;
        Genome<T>[] population;
        Random rnd;
        readonly int populationSize = 20;
        readonly int eliteSize = 5;
        readonly float mutationRate = 0.1f;

        float fitnessSum;

        public GeneticAlgorithm(Func<T[]> randomSolution, Func<T[], float> evalFitness, Func<T> randomGene, Random rnd)
        {
            this.evalFitness = evalFitness;
            this.rnd = rnd;
            this.randomGene = randomGene;
            CreatePopulation(randomSolution);
            SetFitness();
        }

        private void CreatePopulation(Func<T[]> randomSolution)
        {
            population = new Genome<T>[populationSize];

            for (int i = 0; i < population.Length; ++i)
            {
                Genome<T> c = new Genome<T>(randomSolution());
                population[i] = c;
            }
        }

        private void SetFitness()
        {
            fitnessSum = 0;

            for (int i = 0; i < population.Length; ++i)
            {
                float fitness = evalFitness(population[i].Genes); 
                population[i].Fitness = fitness;
                fitnessSum += fitness;
            }

            Array.Sort(population, (x, y) => x.Fitness.CompareTo(y.Fitness));
        }

        private Genome<T> Selection() 
        {
            double random = rnd.NextDouble() * fitnessSum;

            for (int i = 0; i < population.Length; ++i)
            {
                if (random < population[i].Fitness)
                {
                    return population[i];
                }

                random -= population[i].Fitness;
            }

            return null;
        }

        private void Crossover() //One Point Crossover
        {

            Genome<T>[] newGen = new Genome<T>[populationSize];
            int noneElite = populationSize - eliteSize;

            for (int i = 0; i < noneElite; ++i)
            {
                Genome<T> parentA = Selection();
                Genome<T> parentB = Selection();

                int length = parentA.Genes.Length;
                int point = rnd.Next(0, length);
                T[] genes = new T[length];

                for (int m = 0; m < length; ++m)
                {
                    if (m < point)
                    {
                        genes[m] = parentA.Genes[m];
                    }
                    else
                    {
                        genes[m] = parentB.Genes[m];
                    }
                }

                Genome<T> g = new Genome<T>(genes);
                Mutate(ref g);
                newGen[i] = g;
            }

            //elitism
            for (int i = noneElite - 1; i < populationSize; ++i)
            {
                newGen[i] = population[i];
            }

            population = newGen;
            SetFitness();

        }

        private void Mutate(ref Genome<T> c)
        {
            for (int i = 0; i < c.Genes.Length; ++i)
            {
                if (rnd.NextDouble() <= mutationRate)
                {
                    c.Genes[i] = randomGene();
                }
            }
        }



    }
}
