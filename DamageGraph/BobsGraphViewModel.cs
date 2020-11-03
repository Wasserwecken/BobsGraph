using BobsBuddy.Simulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BobsGraphPlugin
{
    public class BobsGraphViewModel : INotifyPropertyChanged
    {
        public enum DiagramType
        {
            Distribution,
            Probabilities
        }

        public Visibility TieLineVisibility { get; private set; }
        public Visibility MinMaxDamageVisible { get; private set; }
        public Visibility EqualDamageVisible { get; private set; }

        public int MaxDamage { get; private set; }
        public int MinDamage { get; private set; }
        public int DamageSpan { get; private set; }

        public double TieLinePositionX1 { get; private set; }
        public double TieLinePositionX2 { get; private set; }
        public double TieLinePositionY1 { get; private set; }
        public double TieLinePositionY2 { get; private set; }
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

        public DiagramType ActiveDiagramType { get; private set; }

        private TestOutput _originalData;
        private Dictionary<int, float> _damageDistribution;
        private Dictionary<float, float> _normalizedDamageDistribution;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        public BobsGraphViewModel()
        {
            ActiveDiagramType = DiagramType.Probabilities;
            GraphPoints = new PointCollection();
            _damageDistribution = new Dictionary<int, float>();
            _normalizedDamageDistribution = new Dictionary<float, float>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            MaxDamage = 0;
            MinDamage = 0;
            DamageSpan = 0;

            _damageDistribution.Clear();
            _normalizedDamageDistribution.Clear();

            _tieColorBase = Color.FromArgb(255, 189, 190, 191);
            _takeDamageColorBase = Color.FromArgb(255, 198, 110, 110);
            _dealDamageColorBase = Color.FromArgb(255, 138, 198, 110);

            EqualDamageColor = _tieColorBase;
            MinDamageColor = _takeDamageColorBase;
            MaxDamageColor = _dealDamageColorBase;
        }

        public void Prepare(TestOutput simulationResult)
        {
            Clear();

            _originalData = simulationResult;
            MaxDamage = simulationResult.result.Max(trace => trace.damage);
            MinDamage = simulationResult.result.Min(trace => trace.damage);
            DamageSpan = MaxDamage - MinDamage;

            if (MinDamage == MaxDamage)
            {
                _damageDistribution.Add(MinDamage, 1f);
                _normalizedDamageDistribution.Add(0.5f, 1f);

                if (MinDamage > 0)
                    MinDamageColor = _dealDamageColorBase;

                else if (MinDamage < 0)
                    MaxDamageColor = _takeDamageColorBase;

                TieLineVisibility = Visibility.Hidden;
                MinMaxDamageVisible = Visibility.Hidden;
                EqualDamageVisible = Visibility.Visible;
            }
            else
            {
                _damageDistribution = simulationResult.result
                    .GroupBy(trace => trace.damage)
                    .OrderBy(group => group.Key)
                    .ToDictionary(group => group.Key, group => group.Count() / (float)simulationResult.result.Count);

                _normalizedDamageDistribution = _damageDistribution
                    .ToDictionary(item => Remap(item.Key, MinDamage, MaxDamage, 0, 1), item => item.Value);

                if (MinDamage > 0 || Math.Abs(MinDamage) < MaxDamage)
                    MinDamageColor = Lerp(_takeDamageColorBase, _dealDamageColorBase, Remap(MinDamage, -MaxDamage, MaxDamage, 0, 1));

                else if (MaxDamage < 0 || Math.Abs(MinDamage) > MaxDamage)
                    MaxDamageColor = Lerp(_takeDamageColorBase, _dealDamageColorBase, Remap(MaxDamage, MinDamage, -MinDamage, 0, 1));

                TieLineVisibility = MinDamage > 0 || MaxDamage < 0 ? Visibility.Hidden : Visibility.Visible;
                MinMaxDamageVisible = Visibility.Visible;
                EqualDamageVisible = Visibility.Hidden;
            }

            TieLineBrush = new SolidColorBrush(_tieColorBase);
            EqualDamageBrush = new SolidColorBrush(EqualDamageColor);
            MinDamageBrush = new SolidColorBrush(MinDamageColor);
            MaxDamageBrush = new SolidColorBrush(MaxDamageColor);
        }

        public void Show(float graphWidth, float graphHeight, float lineWidth)
        {
            switch (ActiveDiagramType)
            {
                case DiagramType.Distribution:
                    ShowDistribution(graphWidth, graphHeight, lineWidth);
                    break;

                case DiagramType.Probabilities:
                    ShowProbabilities(graphWidth, graphHeight, lineWidth);
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        public void ShowNextDiagram(float graphWidth, float graphHeight, float lineWidth)
        {
            ActiveDiagramType = (DiagramType)(((int)ActiveDiagramType + 1) % Enum.GetNames(typeof(DiagramType)).Length);
            Show(graphWidth, graphHeight, lineWidth);
        }

        private void ShowDistribution(float graphWidth, float graphHeight, float lineWidth)
        {
            var lineWidthHalf = lineWidth / 2;
            var horizontal = 0d;
            var vertical = 0d;

            TieLinePositionX1 = 0;
            TieLinePositionX2 = graphWidth;
            TieLinePositionY1 = (Remap(0, MinDamage, MaxDamage, 1, 0) * graphHeight - lineWidthHalf) + lineWidthHalf;
            TieLinePositionY2 = TieLinePositionY1;

            GraphPoints = new PointCollection();
            foreach (var item in _normalizedDamageDistribution)
            {
                vertical = Remap(item.Key, 0, 1, graphHeight - lineWidthHalf, lineWidthHalf);
                GraphPoints.Add(new Point(horizontal, vertical));

                horizontal += item.Value * graphWidth;
                GraphPoints.Add(new Point(horizontal, vertical));
            }

            TakeLethalAreaWidth = graphWidth * _originalData.myDeathRate;
            DealLethalAreaWidth = graphWidth * _originalData.theirDeathRate;
        }

        private void ShowProbabilities(float graphWidth, float graphHeight, float lineWidth)
        {
            var lineWidthHalf = lineWidth / 2;
            var horizontal = 0d;
            var vertical = 0d;
            var maxProbability = _normalizedDamageDistribution.Max(f => f.Value);
            var widthPerEntry = 1f / (DamageSpan + 1);

            TieLinePositionY1 = 0;
            TieLinePositionY2 = graphHeight;
            TieLinePositionX1 = Remap(0, MinDamage, MaxDamage, 0, 1) * graphWidth;
            TieLinePositionX2 = TieLinePositionX1;

            GraphPoints = new PointCollection();
            for (int i = MinDamage; i <= MaxDamage; i++)
            {
                _damageDistribution.TryGetValue(i, out float percentage);

                vertical = Remap(percentage, 0, maxProbability, graphHeight - lineWidthHalf, lineWidthHalf);
                GraphPoints.Add(new Point(horizontal, vertical));

                horizontal += widthPerEntry * graphWidth;
                GraphPoints.Add(new Point(horizontal, vertical));
            }

            if (DamageSpan == 0)
            {
                TakeLethalAreaWidth = _originalData.friendlyHealth + MinDamage < 0 ? graphWidth : 0;
                DealLethalAreaWidth = _originalData.opponentHealth + MaxDamage < 0 ? graphWidth : 0;
            }
            else
            {
                TakeLethalAreaWidth = Math.Abs(Math.Min(0, MinDamage + _originalData.friendlyHealth)) * widthPerEntry;
                DealLethalAreaWidth = Math.Max(0, MaxDamage - _originalData.opponentHealth) * widthPerEntry;
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
