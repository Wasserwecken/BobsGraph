using BobsBuddy.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BobsGraphPlugin
{
    public class BobsGraphViewModel 
    {
        public int MaxDamage { get; private set; }
        public int MinDamage { get; private set; }
        public int DamageSpan { get; private set; }
        public double TieLinePosition { get; private set; }
        
        public Visibility TieLineVisibility { get; private set; }
        public PointCollection DamagePoints { get; private set; }

        public Point GradientPointStart { get; private set; }
        public Point GradientPointEnd { get; private set; }

        public Color MinDamageTextColor { get; private set; }
        public Color MaxDamageTextColor { get; private set; }
        public double TakeLethalAreaWidth { get; private set; }
        public double TakeDamageAreaWidth { get; private set; }
        public double DealDamageAreaWidth { get; private set; }
        public double DealLethalAreaWidth { get; private set; }

        private readonly List<DamageDistributionItem> _normalizedDamageDistribution;


        /// <summary>
        /// 
        /// </summary>
        public BobsGraphViewModel() { }

        /// <summary>
        /// 
        /// </summary>
        public BobsGraphViewModel(TestOutput simulationResult, double graphWidth, double graphHeight)
            : base()
        {
            _normalizedDamageDistribution = new List<DamageDistributionItem>();
            DamagePoints = new PointCollection();

            PerpareData(simulationResult);
            ScaleForUI(graphWidth, graphHeight);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulationResult"></param>
        private void PerpareData(TestOutput simulationResult)
        {
            MaxDamage = simulationResult.result.Max(trace => trace.damage);
            MinDamage = simulationResult.result.Min(trace => trace.damage);
            DamageSpan = MaxDamage - MinDamage;

            if (DamageSpan == 0)
            {
                TieLinePosition = 0.5d;
                _normalizedDamageDistribution.Add(new DamageDistributionItem(0.5d, 1d));
            }
            else
            {
                TieLinePosition = (0 - MinDamage) / (float)DamageSpan;
                TieLinePosition = Math.Max(0, Math.Min(1, TieLinePosition));

                simulationResult.result
                    .GroupBy(trace => trace.damage)
                    .Select(group =>
                    {
                        return new DamageDistributionItem(
                            (group.Key - MinDamage) / (float)DamageSpan,
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
        private void ScaleForUI(double width, double height)
        {
            TieLinePosition = (1 - TieLinePosition) * height;
            TieLineVisibility = MaxDamage >= 0 && MinDamage <= 0 ? Visibility.Visible : Visibility.Hidden;

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
    }
}
