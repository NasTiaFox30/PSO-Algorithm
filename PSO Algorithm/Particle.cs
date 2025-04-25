namespace PSO_Algorithm
{
    /// <summary>
    /// Reprezentuje cząstkę w algorytmie PSO.
    /// </summary>
    public class Particle
    {
        public double[] Position { get; set; }  //pozycja cząstki
        public double[] Velocity { get; set; }  //prędkość cząstki
        public double[] BestPosition { get; set; }  //najlepsza pozycja cząstki
        public double BestFitness { get; set; }     //najlepsza wartość funkcji celu cząstki

        public Particle(int dimensions)
        {
            Position = new double[dimensions];
            Velocity = new double[dimensions];
            BestPosition = new double[dimensions];
            BestFitness = double.MaxValue;
        }
     
    }
}
