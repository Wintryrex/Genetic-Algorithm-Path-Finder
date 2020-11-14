using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GeneticAlgorithm<T>
    {
        List<Generation<T>> records;
        Func<float[], bool> algorithmContinue;
        Func<T[], float> evalFitness;
        Func<T> randomGene;
        Genome<T>[] population;
        Random rnd;
        readonly int populationSize = 20;
        readonly int eliteSize = 5;
        readonly float mutationRate;
        float fitnessSum;
        float[] fitnesses;

        public List<Generation<T>> Records
        {
            get
            {
                return records;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgorithm{T}"/> class.
        /// </summary>
        /// <param name="randomSolution">A random solution generator</param>
        /// <param name="evalFitness">The function that evaluates fitness values. The fitness value has to be normalized. </param>
        /// <param name="randomGene">A function that generates a random bit of a solution </param>
        /// <param name="algorithmContinue">A condition for when to terminate the algorithm </param>
        /// <param name="rnd">An instance of the Random class</param>
        /// <param name="mutationRate">The mutation rate</param>
        public GeneticAlgorithm(Func<T[]> randomSolution, Func<T[], float> evalFitness, Func<T> randomGene, Func<float[], bool> algorithmContinue, Random rnd, float mutationRate)
        {
            this.evalFitness = evalFitness;
            this.rnd = rnd;
            this.randomGene = randomGene;
            this.algorithmContinue = algorithmContinue;
            this.mutationRate = mutationRate;
            records = new List<Generation<T>>(); // Store all generations
            CreatePopulation(randomSolution);
            SetFitness();
        }

        public void Run()
        {
            while (algorithmContinue(fitnesses))
            {
                Crossover();
            }
        }

        private void CreatePopulation(Func<T[]> randomSolution)
        {
            population = new Genome<T>[populationSize];
            fitnesses = new float[populationSize];

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
                fitnesses[i] = fitness;
                fitnessSum += fitness;
            }

            Array.Sort(population, (x, y) => x.Fitness.CompareTo(y.Fitness));
            Generation<T> data = new Generation<T>(population);
            records.Add(data);
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
