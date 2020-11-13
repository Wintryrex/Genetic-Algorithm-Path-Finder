using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GeneticAlgorithm<T>
    {
        Func<T[], float> evalFitness;
        Genome<T>[] population;
        const int populationSize = 20;

        public GeneticAlgorithm(Func<T[]> randomSolution, Func<T[], float> evalFitness)
        {
            this.evalFitness = evalFitness;
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
            for (int i = 0; i < population.Length; ++i)
            {
                population[i].Fitness = evalFitness(population[i].Genes);
            }

            Array.Sort(population, (x, y) => x.Fitness.CompareTo(y.Fitness));
        }

    }
}
