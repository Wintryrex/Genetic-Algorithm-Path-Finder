using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GeneticAlgorithm<T>
    {
        Func<T[], float> evalFitness;
        Genome<T>[] population;
        Random rnd;
        const int populationSize = 20;

        float fitnessSum;

        public GeneticAlgorithm(Func<T[]> randomSolution, Func<T[], float> evalFitness, Random rnd)
        {
            this.evalFitness = evalFitness;
            this.rnd = rnd;
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



    }
}
