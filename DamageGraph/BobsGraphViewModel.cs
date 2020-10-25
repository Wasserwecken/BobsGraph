using BobsBuddy.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BobsGraphPlugin
{
    public class BobsGraphViewModel
    {
        public Visibility MinMaxDamageVisible { get; private set; }
        public Visibility EqualDamageVisible { get; private set; }

        public int MaxDamage { get; private set; }
        public int MinDamage { get; private set; }
        public double TakeLethalAreaWidth { get; private set; }
        public double TakeDamageAreaWidth { get; private set; }
        public double DealDamageAreaWidth { get; private set; }
        public double DealLethalAreaWidth { get; private set; }

        public PointCollection DamagePoints { get; private set; }
        public List<Rectangle> DamageBars { get; private set; }

        public Point GradientPointStart { get; private set; }
        public Point GradientPointEnd { get; private set; }

        public Brush EqualDamageBrush { get; private set; }
        public Brush MinDamageBrush { get; private set; }
        public Brush MaxDamageBrush { get; private set; }
        public Color EqualDamageColor { get; private set; }
        public Color MinDamageColor { get; private set; }
        public Color MaxDamageColor { get; private set; }


        private Color _tieColorBase;
        private Color _takeDamageColorBase;
        private Color _dealDamageColorBase;
        private readonly List<DamageDistributionItem> _normalizedDamageDistribution;


        /// <summary>
        /// 
        /// </summary>
        public BobsGraphViewModel()
        {
            _tieColorBase = Color.FromArgb(255, 189, 190, 191);
            _takeDamageColorBase = Color.FromArgb(255, 198, 110, 110);
            _dealDamageColorBase = Color.FromArgb(255, 138, 198, 110);
        }

        /// <summary>
        /// 
        /// </summary>
        public BobsGraphViewModel(TestOutput simulationResult, double graphWidth, double graphHeight)
            : this()
        {
            _normalizedDamageDistribution = new List<DamageDistributionItem>();
            DamagePoints = new PointCollection();
            DamageBars = new List<Rectangle>();

            EvaluateData(simulationResult);
            SetColors();
            SetDamageBars(graphWidth, graphHeight);
            SetVisibilityData();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulationResult"></param>
        private void EvaluateData(TestOutput simulationResult)
        {
            MaxDamage = simulationResult.result.Max(trace => trace.damage);
            MinDamage = simulationResult.result.Min(trace => trace.damage);
            var maxPeak = Math.Max(Math.Abs(MinDamage), Math.Abs(MaxDamage));

            if (MinDamage == MaxDamage)
            {
                _normalizedDamageDistribution.Add(new DamageDistributionItem(0.5d, 1d));
            }
            else
            {
                simulationResult.result
                    .GroupBy(trace => trace.damage)
                    .Select(group =>
                    {
                        return new DamageDistributionItem(
                            Remap(group.Key, -maxPeak, maxPeak, 0, 1),
                            group.Count() / (float)simulationResult.result.Count
                        );
                    })
                    .OrderBy(item => item.Value)
                    .ToList()
                    .ForEach(item => _normalizedDamageDistribution.Add(item));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void SetDamageBars(double width, double height)
        {
            var barWidth = _normalizedDamageDistribution.Count() / width;
            
            var horizontalPosition = 0d;
            var verticalPosition = 0d;
            _normalizedDamageDistribution.ForEach(item =>
            {
                verticalPosition = (1 - item.Value) * height;
                DamagePoints.Add(new Point(horizontalPosition, verticalPosition));

                horizontalPosition += item.Percentage * width;
                DamagePoints.Add(new Point(horizontalPosition, verticalPosition));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetColors()
        {
            EqualDamageColor = _tieColorBase;
            MinDamageColor = _takeDamageColorBase;
            MaxDamageColor = _dealDamageColorBase;


            if (MinDamage == MaxDamage)
            {
                if (MinDamage > 0)
                    MinDamageColor = _dealDamageColorBase;

                else if (MinDamage < 0)
                    MaxDamageColor = _takeDamageColorBase;
            }
            else
            {
                if (MinDamage > 0 || Math.Abs(MinDamage) < MaxDamage)
                {
                    var weight = Remap(MinDamage, -MaxDamage, MaxDamage, 0, 1);
                    MinDamageColor = Lerp(_takeDamageColorBase, _dealDamageColorBase, weight);
                }

                else if (MaxDamage < 0 || Math.Abs(MinDamage) > MaxDamage)
                {
                    var weight = Remap(MaxDamage, MinDamage, -MinDamage, 0, 1);
                    MaxDamageColor = Lerp(_takeDamageColorBase, _dealDamageColorBase, weight);
                }
            }

            EqualDamageBrush = new SolidColorBrush(EqualDamageColor);
            MinDamageBrush = new SolidColorBrush(MinDamageColor);
            MaxDamageBrush = new SolidColorBrush(MaxDamageColor);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetVisibilityData()
        {
            if (MinDamage == MaxDamage)
            {
                MinMaxDamageVisible = Visibility.Hidden;
                EqualDamageVisible = Visibility.Visible;
            }
            else
            {
                MinMaxDamageVisible = Visibility.Visible;
                EqualDamageVisible = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private struct DamageDistributionItem
        {
            public double Value { get; set; }
            public double Percentage { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="percentage"></param>
            public DamageDistributionItem(double value, double percentage)
            {
                Value = value;
                Percentage = percentage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weigth"></param>
        /// <returns></returns>
        public static Color Lerp(Color a, Color b, float weigth)
        {
            return Color.FromArgb(
                (byte)(a.A * (1 - weigth) + b.A * weigth),
                (byte)(a.R * (1 - weigth) + b.R * weigth),
                (byte)(a.G * (1 - weigth) + b.G * weigth),
                (byte)(a.B * (1 - weigth) + b.B * weigth)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outLower"></param>
        /// <param name="outUpper"></param>
        /// <returns></returns>
        float Remap(float value, float inLower, float inUpper, float outLower, float outUpper)
        {
            value -= inLower;
            value *= outUpper - outLower;
            value /= inUpper - inLower;
            value += outLower;

            return value;
        }
    }
}
