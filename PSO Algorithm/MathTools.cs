using System;
namespace PSO_Algorithm
{
    /// <summary>
    /// Narzędzia matematyczne
    /// </summary>
    public static class MathTools
    {

        /// <summary>
        /// Funkcja Rastrigina:
        /// f(x) = 10N + Σ [x_i² - 10·cos(2π·x_i)]
        /// -->
        /// (do oceniania jakości pozycji cząstki)
        /// </summary>
        public static double RastriginFunction(double[] x)
        {
            double sum = 10 * x.Length;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * x[i] - 10 * Math.Cos(2 * Math.PI * x[i]);
            }
            return sum;
        }

        /// <summary>
        /// mapowanie wartości z jednego zakresu do drugiego
        /// (masztabowanie)
        /// </summary>
        public static double Map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }

    }
}
