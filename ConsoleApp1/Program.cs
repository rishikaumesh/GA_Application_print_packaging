
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmExample
{
    class Program
    {
        static Random random = new Random();

        static List<Order> OrderList = new List<Order>
        {
            new Order { OrderId = 1, Quantity = 7000, StNumber = 1 },
            new Order { OrderId = 2, Quantity = 7000, StNumber = 2 },
            new Order { OrderId = 2, Quantity = 7000, StNumber = 2 },
            new Order { OrderId = 2, Quantity = 7000, StNumber = 2 },
            new Order { OrderId = 2, Quantity = 7000, StNumber = 2 },
            new Order { OrderId = 1, Quantity = 7000, StNumber = 1 },
            new Order { OrderId = 2, Quantity = 7000, StNumber = 2 },
            new Order { OrderId = 1, Quantity = 7000, StNumber = 1 },
            new Order { OrderId = 1, Quantity = 7000, StNumber = 1 },
            new Order { OrderId = 1, Quantity = 7000, StNumber = 1 },
            new Order { OrderId = 1, Quantity = 7000, StNumber = 1 },


        };

        static int NumStationsPerSheet = 8;

        static int PopulationSize = 3000;
        static double MutationRate = 0.1;
        static int NumGenerations = 100;

        static void Main(string[] args)
        {
            random = new Random();

            List<List<Order>> population = InitializePopulation();

            int generation;
            double previousBestFitness = double.NegativeInfinity;
            int consecutiveGenerationsWithoutImprovement = 0;
            double convergenceThreshold = 0.001; // Adjust this threshold as needed
            int maxConsecutiveGenerations = 20; // Adjust as needed

            List<Order> bestCandidate = null; // Declare the variable outside the loop


            for (generation = 0; generation < NumGenerations; generation++)
            {
                //Console.WriteLine($"Generation {generation + 1}");
                List<List<Order>> selectedParents = SelectParents(population);

                List<List<Order>> newPopulation = new List<List<Order>>();
                for (int i = 0; i < PopulationSize; i++)
                {
                    List<Order> parent1 = selectedParents[random.Next(selectedParents.Count)];
                    List<Order> parent2 = selectedParents[random.Next(selectedParents.Count)];

                    int crossoverPoint = random.Next(1, NumStationsPerSheet);
                    //int crossoverPoint = random.Next(1, Math.Min(parent1.Count, parent2.Count));
                    List<Order> child = parent1.Take(crossoverPoint).Concat(parent2.Skip(crossoverPoint)).ToList();

                    if (random.NextDouble() < MutationRate)
                    {
                        int mutationPoint = random.Next(NumStationsPerSheet);

                        if (mutationPoint < child.Count) // Ensure mutation point is within valid range for child list
                        {
                            int orderIndex = random.Next(OrderList.Count); // Generate a valid index for OrderList
                            child[mutationPoint] = OrderList[orderIndex];
                        }
                    }


                    newPopulation.Add(child);
                }

                population = newPopulation;

                bestCandidate = population
                   .Select(candidate => new { Candidate = candidate, Fitness = CalculateFitness(candidate) })
                   .OrderByDescending(x => x.Fitness)
                   .First().Candidate;

                // Calculate the best candidate after generating the new population
                double currentBestFitness = CalculateFitness(bestCandidate);

                if (currentBestFitness - previousBestFitness <= convergenceThreshold)
                {
                    consecutiveGenerationsWithoutImprovement++;
                }
                else
                {
                    consecutiveGenerationsWithoutImprovement = 0;
                }

                previousBestFitness = currentBestFitness;

                if (consecutiveGenerationsWithoutImprovement >= maxConsecutiveGenerations)
                {
                    Console.WriteLine($"Converged after {generation + 1} generations.");
                    break;
                }
            }
            Console.WriteLine($"Total generations needed: {generation + 1}");

            Console.WriteLine("Best Candidate Sheet:");
            foreach (var order in bestCandidate)
            {
                Console.WriteLine($"Order {order.OrderId} - Quantity: {order.Quantity} - ST Number: {order.StNumber}");
            }
            Console.ReadKey();
        }
   
        static List<List<Order>> InitializePopulation()
        {
            List<List<Order>> population = new List<List<Order>>();
            for (int i = 0; i < PopulationSize; i++)
            {
                List<Order> candidate = OrderList.OrderBy(x => random.Next()).Take(NumStationsPerSheet).ToList();
                population.Add(candidate);
            }
            return population;
        }

        static List<List<Order>> SelectParents(List<List<Order>> population)
        {
            List<List<Order>> selectedParents = new List<List<Order>>();
            for (int i = 0; i < PopulationSize; i++)
            {
                List<Order> parent = population[random.Next(population.Count)];
                selectedParents.Add(parent);
            }
            return selectedParents;
        }

        // Inside the CalculateFitness method
        static double CalculateFitness(List<Order> candidate)
        {
            int totalQuantity = candidate.Sum(order => order.Quantity);
            int maxStationNumber = candidate.Max(order => order.StNumber);

            // You can define your fitness function based on these values or any other criteria you have
            // For this example, let's create a simple fitness function that takes both total quantity and max station number into account
            double fitness = totalQuantity * (1.0 / (maxStationNumber + 1));

            return fitness;
        }

    }

    class Order
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public int StNumber { get; set; }
    }
}