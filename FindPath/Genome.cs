using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class Genome<T>
    {
        T[] genes;
        float fitness;

        public T[] Genes
        {
            get
            {
                return genes;
            }
            set
            {
                genes = value;
            }
        }

        public float Fitness
        {
            get
            {
                return fitness;
            }
            set
            {
                fitness = value;
            }
        }

        public Genome(T[] genes)
        {
            this.genes = genes;
        }

    }
}
