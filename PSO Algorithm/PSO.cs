using System;
using System.Collections.Generic;

namespace PSO_Algorithm
{
    /// <summary>
    /// koncepcja algorytmu PSO
    /// </summary>
    public class PSO
    {
        private readonly Random _random = new Random();
        public List<Particle> Particles { get; private set; }
        public double[] GlobalBestPosition { get; private set; }
        public double GlobalBestFitness { get; private set; } = double.MaxValue;

        public int Dimension { get; }
        public double MinX { get; }
        public double MaxX { get; }
        public int MaxIterations { get; }
        public int PopulationSize { get; }
        public double InertiaWeight { get; }
        public double CognitiveWeight { get; }
        public double SocialWeight { get; }

        public Func<double[], double> FitnessFunction { get; }

        public PSO(int dimension, double minX, double maxX, int maxIterations, int populationSize,
                   double inertiaWeight, double cognitiveWeight, double socialWeight,
                   Func<double[], double> fitnessFunction)
        {
            Dimension = dimension;
            MinX = minX;
            MaxX = maxX;
            MaxIterations = maxIterations;
            PopulationSize = populationSize;
            InertiaWeight = inertiaWeight;
            CognitiveWeight = cognitiveWeight;
            SocialWeight = socialWeight;
            FitnessFunction = fitnessFunction;

            InitializeParticles();
        }

        /// <summary>
        /// inicjalizacja cząstek w roju
        /// </summary>
        public void InitializeParticles()
        {
            Particles = new List<Particle>();

            for (int i = 0; i < PopulationSize; i++)
            {
                var particle = new Particle(Dimension);

                for (int j = 0; j < Dimension; j++)
                {
                    particle.Position[j] = MinX + (MaxX - MinX) * _random.NextDouble();
                    particle.Velocity[j] = (MinX - MaxX) + 2 * (MaxX - MinX) * _random.NextDouble();
                }

                Particles.Add(particle);
            }
        }

        /// <summary>
        /// Ocena przystosowania i aktualizacja najlepszych pozycji +
        /// Aktualizacja prędkości i pozycji
        /// </summary>
        public void Run(int currentIteration)
        {
            foreach (var particle in Particles)
            {
                double fitness = FitnessFunction(particle.Position);

                if (fitness < particle.BestFitness)
                {
                    particle.BestPosition = (double[])particle.Position.Clone();
                    particle.BestFitness = fitness;
                }

                if (fitness < GlobalBestFitness)
                {
                    GlobalBestPosition = (double[])particle.Position.Clone();
                    GlobalBestFitness = fitness;
                }
            }

            foreach (var particle in Particles)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    double r1 = _random.NextDouble();
                    double r2 = _random.NextDouble();

                    particle.Velocity[j] = InertiaWeight * particle.Velocity[j] +
                                          CognitiveWeight * r1 * (particle.BestPosition[j] - particle.Position[j]) +
                                          SocialWeight * r2 * (GlobalBestPosition[j] - particle.Position[j]);

                    particle.Position[j] += particle.Velocity[j];

                    if (particle.Position[j] < MinX)
                        particle.Position[j] = MinX;
                    if (particle.Position[j] > MaxX)
                        particle.Position[j] = MaxX;
                }
            }
        }
    }

}
