using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class Generation<T>
    {
        Genome<T>[] population;

        public Genome<T>[] Population
        {
            get { return population; }
        }

        public Generation(Genome<T>[] population)
        {
            this.population = population;
        }

        public Genome<T> GetFittest()
        {
            return population[population.Length - 1];
        }
    }
}
