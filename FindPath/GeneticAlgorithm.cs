using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GeneticAlgorithm<T>
    {
        Genome<T>[] population;
        const int populationSize = 20;

        public GeneticAlgorithm(Func<T[]> randomSolution)
        {
            CreatePopulation(randomSolution);
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

    }
}
