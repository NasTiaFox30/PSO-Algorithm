using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Algorithm
{
    /// <summary>  
    /// Formularz do wizualizacji algorytmu optymalizacji rojem cząstek (PSO).  
    /// </summary>  
    public class PSOVisualization : Form
    {
        private readonly PSO _PSO;
        private int _currentIteration;
        private readonly System.Windows.Forms.Timer _timer;
        private readonly PictureBox _Box;
        private readonly Label _infoLabel, _infoSpeedLabel;
        private readonly TrackBar _speedTrackB;

        // Parametry funkcji Rastrigina w okresie [-5.12, 5.12]
        private const double MinX = -5.12;
        private const double MaxX = 5.12;

        private const double AnimationSteps = 15;

        public PSOVisualization()
        {
            //zmniejszanie migotania grafiki za pomocą podwójnego buforowania formularzy i kontrolek
            DoubleBuffered = true;
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            Text = "Wizualizacja Algorytmu PSO";
            // Parametry okna  
            ClientSize = new Size(1200, 900);
            StartPosition = FormStartPosition.CenterScreen;

            _PSO = new PSO(
                dimension: 2,
                minX: MinX,
                maxX: MaxX,
                maxIterations: 1000,
                populationSize: 50,

                // parametry PSO:  
                inertiaWeight: 0.8,
                cognitiveWeight: 1.5,
                socialWeight: 1.5,

                // funkcja matematyczna  
                fitnessFunction: MathTools.RastriginFunction
            );

            #region Inicjalizacja okna i elementów UI

            _Box = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                //DoubleBuffered = true
            };
            _Box.Paint += Box_Paint;

            _infoLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _speedTrackB = new TrackBar
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Minimum = 10,
                Maximum = 500,
                Value = 50, // początkowa prędkość  
                TickFrequency = 50,
                LargeChange = 50,
                SmallChange = 10
            };
            _speedTrackB.ValueChanged += SpeedTrackBar_ValueChanged;

            _infoSpeedLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = $"Prędkość animacji: {_speedTrackB.Value}ms"
            };

            // Dodawanie elementów do okna  
            Controls.Add(_Box);
            Controls.Add(_infoLabel);
            Controls.Add(_infoSpeedLabel);
            Controls.Add(_speedTrackB);

            #endregion

            // inicjowanie i uruchomienie algorytmu
            _timer = new System.Windows.Forms.Timer { Interval = _speedTrackB.Value };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// główny cykl algorytmu PSO
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_currentIteration < _PSO.MaxIterations)
            {
                _PSO.Run(_currentIteration);
                _currentIteration++;
                _infoLabel.Text = $"Iteracja: {_currentIteration}, Najlepsza wartość funkcji celu: {_PSO.GlobalBestFitness:F4}";
                _Box.Invalidate();

                // Sprawdzenie, czy cząstki są blisko siebie (są zielone)  
                int closeParticles = 0;
                double threshold = 0.05; // próg odległości  
                foreach (var particle in _PSO.Particles)
                {
                    double distance = CalculateDistance(particle.Position, _PSO.GlobalBestPosition);
                    if (distance < threshold)
                    {
                        closeParticles++;
                    }
                }

                if (closeParticles >= _PSO.PopulationSize)
                {
                    _timer.Stop();
                    _infoLabel.Text += " - Wszystkie cząstki zbieżne!";
                }

                _Box.Invalidate();
            }
            else
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// obliczanie odległości między dwiema pozycjami cząstek
        /// </summary>
        private double CalculateDistance(double[] position1, double[] position2)
        {
            if (position1 == null || position2 == null)
                return double.MaxValue;

            double sum = 0;
            for (int i = 0; i < position1.Length; i++)
            {
                sum += Math.Pow(position1[i] - position2[i], 2);
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// odswieżanie prędkości animacji
        /// </summary>
        private void SpeedTrackBar_ValueChanged(object sender, EventArgs e)
        {
            _timer.Interval = _speedTrackB.Value;
            _infoSpeedLabel.Text = $"Prędkość animacji: {_speedTrackB.Value}ms";
        }

        /// <summary>
        /// tworzenie przestrzeni vizualizacji algorytmu PSO
        /// </summary>
        private void Box_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var width = _Box.Width;
            var height = _Box.Height;

            //DrawFunctionContours(g, width, height);
            DrawParticles(g, width, height);
            DrawBestPosition(g, width, height);
        }

        /// <summary>
        /// rysowanie cząstek
        /// </summary>
        private void DrawParticles(Graphics g, int width, int height)
        {
            double threshold = 0.05;
            int greenParticles = 0;

            foreach (var particle in _PSO.Particles)
            {
                int x = (int)MathTools.Map(particle.Position[0], MinX, MaxX, 0, width);
                int y = (int)MathTools.Map(particle.Position[1], MinX, MaxX, 0, height);

                double distance = CalculateDistance(particle.Position, _PSO.GlobalBestPosition);
                if (distance < threshold)
                {
                    g.FillEllipse(Brushes.Green, x - 3, y - 3, 6, 6);
                    greenParticles++;
                }
                else
                {
                    g.FillEllipse(Brushes.Red, x - 3, y - 3, 6, 6);
                }
            }

            // Aktualizacja interfejsu użytkownika z informacją o zbieżności, jeśli to konieczne  
            if (greenParticles >= _PSO.PopulationSize && _timer.Enabled)
            {
                _infoLabel.Text = $"Iteracja: {_currentIteration}, Najlepsza wartość funkcji celu: {_PSO.GlobalBestFitness:F4} - Wszystkie cząstki zbieżne!";
            }
        }

        /// <summary>
        /// rysowanie najlepszej pozycji
        /// </summary>
        private void DrawBestPosition(Graphics g, int width, int height)
        {
            if (_PSO.GlobalBestPosition != null)
            {
                int bestX = (int)MathTools.Map(_PSO.GlobalBestPosition[0], MinX, MaxX, 0, width);
                int bestY = (int)MathTools.Map(_PSO.GlobalBestPosition[1], MinX, MaxX, 0, height);

                Rectangle rect = new Rectangle(bestX - 50, bestY - 50, 100, 100);
                // Pędzel z przezroczystością (50 / 255)  
                using (Brush semiTransparentBrush = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    g.FillEllipse(semiTransparentBrush, rect);
                }
                using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawEllipse(pen, rect);
                }
            }
        }

        /// <summary>
        /// rysowanie siatki
        /// </summary>
        private void DrawFunctionContours(Graphics g, int width, int height)
        {
            int steps = 50;
            double stepSize = (MaxX - MinX) / steps;

            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    double x = MinX + i * stepSize;
                    double y = MinX + j * stepSize;
                    double value = MathTools.RastriginFunction(new[] { x, y });

                    int colorValue = (int)MathTools.Map(value, 0, 50, 0, 255);
                    colorValue = Math.Clamp(colorValue, 0, 255);

                    var brush = new SolidBrush(Color.FromArgb(colorValue, colorValue, colorValue));

                    int rectX = (int)MathTools.Map(x, MinX, MaxX, 0, width);
                    int rectY = (int)MathTools.Map(y, MinX, MaxX, 0, height);
                    int rectSize = (int)(width / steps) + 1;

                    g.FillRectangle(brush, rectX, rectY, rectSize, rectSize);
                }
            }
        }

    }
}
