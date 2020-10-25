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
        /// <summary>
        /// 
        /// </summary>
        private struct DamageDistributionItem
        {
            public float Damage { get; set; }
            public float Percentage { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="damage"></param>
            /// <param name="percentage"></param>
            public DamageDistributionItem(float damage, float percentage)
            {
                Damage = damage;
                Percentage = percentage;
            }
        }

        public Visibility TieLineVisibility { get; private set; }
        public Visibility MinMaxDamageVisible { get; private set; }
        public Visibility EqualDamageVisible { get; private set; }

        public int MaxDamage { get; private set; }
        public int MinDamage { get; private set; }
        
        public double TieLinePosition { get; private set; }
        public double TakeLethalAreaWidth { get; private set; }
        public double TakeDamageAreaWidth { get; private set; }
        public double DealDamageAreaWidth { get; private set; }
        public double DealLethalAreaWidth { get; private set; }
        
        public PointCollection GraphPoints { get; private set; }

        public Brush TieLineBrush { get; private set; }
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
        public BobsGraphViewModel(TestOutput simulationResult, float graphWidth, float graphHeight, float lineWidth)
            : this()
        {
            _normalizedDamageDistribution = new List<DamageDistributionItem>();
            GraphPoints = new PointCollection();

            PrepareData(simulationResult);
            SetGraph(graphWidth, graphHeight, lineWidth);
            SetVisibility();
            SetColors();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulationResult"></param>
        private void PrepareData(TestOutput simulationResult)
        {
            MaxDamage = simulationResult.result.Max(trace => trace.damage);
            MinDamage = simulationResult.result.Min(trace => trace.damage);
            var maxPeak = Math.Max(Math.Abs(MinDamage), Math.Abs(MaxDamage));

            if (MinDamage == MaxDamage)
            {
                _normalizedDamageDistribution.Add(new DamageDistributionItem(0.5f, 1f));
            }
            else
            {
                simulationResult.result
                    .GroupBy(trace => trace.damage)
                    .Select(group =>
                    {
                        return new DamageDistributionItem(
                            Remap(group.Key, MinDamage, MaxDamage, 0, 1),
                            group.Count() / (float)simulationResult.result.Count
                        );
                    })
                    .OrderBy(item => item.Damage)
                    .ToList()
                    .ForEach(item => _normalizedDamageDistribution.Add(item));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void SetGraph(float width, float height, float lineWidth)
        {
            var lineHalfWidth = lineWidth / 2;
            var horizontal = 0d;
            var vertical = 0d;

            TieLinePosition = (Remap(0, MinDamage, MaxDamage, 1, 0) * height -lineHalfWidth) + lineHalfWidth;
            _normalizedDamageDistribution.ForEach(item =>
            {
                vertical = Remap(item.Damage, 0, 1, height -lineHalfWidth, lineHalfWidth);
                GraphPoints.Add(new Point(horizontal, vertical));

                horizontal += item.Percentage * width;
                GraphPoints.Add(new Point(horizontal, vertical));
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

            TieLineBrush = new SolidColorBrush(_tieColorBase);
            EqualDamageBrush = new SolidColorBrush(EqualDamageColor);
            MinDamageBrush = new SolidColorBrush(MinDamageColor);
            MaxDamageBrush = new SolidColorBrush(MaxDamageColor);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetVisibility()
        {
            if (MinDamage == MaxDamage)
            {
                TieLineVisibility = Visibility.Hidden;
                MinMaxDamageVisible = Visibility.Hidden;
                EqualDamageVisible = Visibility.Visible;
            }
            else
            {
                TieLineVisibility = MinDamage > 0 || MaxDamage < 0 ? Visibility.Hidden : Visibility.Visible;
                MinMaxDamageVisible = Visibility.Visible;
                EqualDamageVisible = Visibility.Hidden;
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
