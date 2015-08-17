using System;

/*
    [1] http://www.generation5.org/content/2003/gahelloworld.asp
    [2] http://jacobconradmartin.com/experiments/evolution/implementation/c/
    [3] http://www.puremango.co.uk/2010/12/genetic-algorithm-for-hello-world/ (met uitleg)

    start with random population
    loop:
	    1. sort by best fit (0 = exact fit, the higher the number less fit it is), if that one is 0 then break (we're done)
	    2. print best fit (value and fit score)
	    3. use best fits to generate (mate) several random ones (offspring), also include the current one (since if it is better, we want random strings from that one again)
		    mate bij "hello world!" is offspring = parent1.substring(0, randomPosX) + parent2.substring(randomPosX)
    done

    more explanation:
	    3. make a selection of the top ones and mate them add the offspring to the new population pool
		    from link [2]:
		    for (int g = 0; g < MAX_GENERATIONS + 1; g++) {
	                sort_by_fitness(population, bins, bin_count);
	                select_breeding_pool(bins, bin_count, breeding_pool);
	                random_population_from_pool(population, breeding_pool);
            	    }
		    select_breeding_pool
			    Select individuals from population for admission to breeding pool, based on fitness
	    		    We start by selecting from the bin with the most matches and work down,
    			    picking randomly from a bin if there are more individuals in the bin than we need.
		    random_population_from_pool
			    Repeatedly select different members from the breeding pool at random and mate them to
			    produce a new individual in the population[] array, stopping when that array is full
				    Pick one individual from breeding pool
				    Select another item, ensuring it's not the same one
				    Mate them and add the offspring to the new population pool 
 */
namespace GeneticAlgoTestApp {
    public class GeneticHelloWorld {
        private struct Chromosome {
            public char[] data;
            public int fitness;
        }
        private readonly char[] Target = "Hello world!".ToCharArray();
        private readonly int ChromosomeLength;
        private const decimal TopPercentage = 0.1m; // percentage / 100: with how many from the population will we go to the next round (the elites)?
        private const int MaxIterations = 20000;
        private const decimal MutationRate = 0.25m; // percentage / 100: how many children wil mutate a little?
        private Chromosome[] population = new Chromosome[1000];
        private Random random = new Random();
        private const bool UseBestParentIfOffspringIsWorse = false;

        public GeneticHelloWorld() {
            ChromosomeLength = Target.Length;
            CreateStartPopulation();
        }

        public void Run() {
            for (int i = 0; i < MaxIterations; i++) {
                CalculateFitnesses();
                SortPopulation();

                int fitness = population[0].fitness;
                Console.WriteLine(string.Format("{0}\tBest: {1} ({2})", i, new string(population[0].data), fitness));
                if (fitness == 0) break;

                Chromosome[] elites = GetTop();
                CreateNewPopulationFromTop(elites);
            }
        }

        private void CalculateFitnesses() {
            for (int i = 0; i < population.Length; i++) {
                population[i].fitness = CalculateFitness(population[i]);
            }
        }

        // note, this is elitism, and might not give the best result since the population will not be very diverse, but in this case (with a static target) it will work
        private void CreateNewPopulationFromTop(Chromosome[] elites) {
            int rowsPerParent = (int)Math.Ceiling((decimal)population.Length / (decimal)elites.Length); // note: can be a little bit more, since it rounds up
            int parent2Index = -1;
            for (int i = 0; i < population.Length; i++) {
                int parent1Index = i % elites.Length;
                if (i % rowsPerParent == 0) parent2Index++;
                population[i].data = GetOffspring(elites[parent1Index], elites[parent2Index]);
            }
        }

        private char[] GetOffspring(Chromosome parent1, Chromosome parent2) {
            char[] offspring = MateParents(ref parent1, ref parent2);

            // random mutation
            if (random.Next(100) < (int)(100.0m * MutationRate)) {
                MutateChromosome(ref offspring);
            }

            if (UseBestParentIfOffspringIsWorse) {
                return GetBestChromosome(parent1, parent2, offspring);
            } else {
                return offspring;
            }
        }

        private char[] GetBestChromosome(Chromosome parent1, Chromosome parent2, char[] offspring) {
            int fitnessOffspring = CalculateFitness(new Chromosome() { data = offspring });
            if (parent1.fitness < fitnessOffspring && parent1.fitness < parent2.fitness) {
                return parent1.data;
            } else if (parent2.fitness < fitnessOffspring) {
                return parent2.data;
            }

            return offspring;
        }

        private void MutateChromosome(ref char[] offspring) {
            int randomPos = random.Next(ChromosomeLength);
            // alter the character slightly (this makes more sense than altering it at random since the fitness function uses the char difference as a metric)
            char c = (char)((int)offspring[randomPos] + (random.Next(2) % 2 == 0 ? 1 : -1));
            if (c < 32) c = (char)(98 + 32);
            else if (c > 98 + 32) c = (char)32;
            offspring[randomPos] = c;
        }

        private char[] MateParents(ref Chromosome parent1, ref Chromosome parent2) {
            char[] offspring = new char[ChromosomeLength];
            int randomPos = random.Next(ChromosomeLength);
            Array.Copy(parent1.data, offspring, randomPos);
            Array.Copy(parent2.data, randomPos, offspring, randomPos, ChromosomeLength - randomPos);
            return offspring;
        }

        private Chromosome[] GetTop() {
            Chromosome[] elites = new Chromosome[(int)Math.Ceiling((decimal)population.Length * TopPercentage)];
            Array.Copy(population, elites, elites.Length);
            return elites;
        }

        private int CalculateFitness(Chromosome item) {
            int fitness = 0;

            char[] data = item.data;
            for (int i = 0; i < ChromosomeLength; i++) {
                fitness += Math.Abs((int)data[i] - (int)Target[i]);
            }

            return fitness;
        }

        private void SortPopulation() {
            Array.Sort(population, delegate(Chromosome item1, Chromosome item2) {
                return item1.fitness - item2.fitness;
            });
        }

        private void CreateStartPopulation() {
            for (int i = 0; i < population.Length; i++) {
                population[i].data = CreateRandomChromosomeData(random, ChromosomeLength);
            }
        }

        private char[] CreateRandomChromosomeData(Random random, int length) {
            char[] result = new char[length];

            for (int i = 0; i < length; i++) {
                result[i] = (char)(random.Next(90) + 32);
            }

            return result;
        }
    }
}
