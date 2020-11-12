using BobsBuddy.Simulation;
using BobsGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BobsGraphPlugin
{
    public class BobsGraphViewModel : INotifyPropertyChanged
    {
        public int MinDamage { get; private set; }
        public int MaxDamage { get; private set; }
        public int DamageSpan => MaxDamage - MinDamage + 1;
        public double MaxProbability { get; private set; }

        public Visibility TieLineVisibility { get; private set; }
        public Visibility MinMaxDamageVisible { get; private set; }
        public Visibility EqualDamageVisible { get; private set; }

        public double TieLineThickness { get; private set; }
        public double TieLinePositionX { get; private set; }
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

        private TestOutput _data;

        private Dictionary<int, double> _damageDistribution;

        private double _graphWidth;
        private double _graphHeight;
        private double _lineWidth;

        private Color _tieColorBase;
        private Color _takeDamageColorBase;
        private Color _dealDamageColorBase;

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _tieColorBase = Color.FromArgb(255, 189, 190, 191);
            _takeDamageColorBase = Color.FromArgb(255, 198, 110, 110);
            _dealDamageColorBase = Color.FromArgb(255, 138, 198, 110);

            EqualDamageColor = _tieColorBase;
            MinDamageColor = _takeDamageColorBase;
            MaxDamageColor = _dealDamageColorBase;

            GraphPoints = new PointCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphWidth"></param>
        /// <param name="graphHeight"></param>
        /// <param name="lineWidth"></param>
        public void UpdateSize(double graphWidth, double graphHeight, double lineWidth)
        {
            _graphWidth = graphWidth;
            _graphHeight = graphHeight;
            _lineWidth = lineWidth;

            if (_data != null)
            {
                UpdateData(_data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void UpdateData(TestOutput data)
        {
            _data = data;

            Clear();
            EvaluateData();
            SetColors();
            SetVisibility();
            SetGraphPoints();
            SetTieLethal();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        /// <summary>
        /// 
        /// </summary>
        private void EvaluateData()
        {
            MaxDamage = _data.result.Max(trace => trace.damage);
            MinDamage = _data.result.Min(trace => trace.damage);

            if (DamageSpan == 1)
            {
                _damageDistribution = new Dictionary<int, double>() { { MinDamage, 1f } };
                MaxProbability = 1f;
            }
            else
            {
                _damageDistribution = _data.result
                    .GroupBy(trace => trace.damage)
                    .OrderBy(group => group.Key)
                    .ToDictionary(group => group.Key, group => group.Count() / (double)_data.result.Count);
                MaxProbability = _damageDistribution.Max(item => item.Value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetGraphPoints()
        {
            var vertical = 0d;
            var horizontal = 0d;
            var lineWidthHalf = _lineWidth / 2;
            var widthPerEntry = 1f / DamageSpan;

            GraphPoints = new PointCollection();
            for (int i = MinDamage; i <= MaxDamage; i++)
            {
                _damageDistribution.TryGetValue(i, out double percentage);

                vertical = percentage.Remap(0, MaxProbability, _graphHeight - lineWidthHalf, lineWidthHalf);
                GraphPoints.Add(new Point(horizontal, vertical));

                horizontal += widthPerEntry * _graphWidth;
                GraphPoints.Add(new Point(horizontal, vertical));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetTieLethal()
        {
            TieLinePositionY1 = 0;
            TieLinePositionY2 = _graphHeight;
            TieLinePositionX = 0.Remap(MinDamage, MaxDamage + 1, 0, 1) * _graphWidth;
            TieLineThickness = _graphWidth / DamageSpan;
            TieLinePositionX += TieLineThickness * 0.5f;

            if (DamageSpan == 1)
            {
                TakeLethalAreaWidth = _data.friendlyHealth + MinDamage < 0 ? _graphWidth : 0;
                DealLethalAreaWidth = _data.opponentHealth - MaxDamage < 0 ? _graphWidth : 0;
            }
            else
            {
                var widthPerEntry = 1f / DamageSpan * _graphWidth;
                TakeLethalAreaWidth = Math.Abs(Math.Min(0, _data.friendlyHealth + MinDamage)) * widthPerEntry;
                DealLethalAreaWidth = Math.Abs(Math.Min(0, _data.opponentHealth - MaxDamage)) * widthPerEntry;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetVisibility()
        {
            if (DamageSpan == 1)
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
        private void SetColors()
        {
            if (DamageSpan == 1)
            {
                if (MinDamage > 0)
                {
                    MinDamageColor = _dealDamageColorBase;
                    EqualDamageColor = _dealDamageColorBase;
                }

                else if (MaxDamage < 0)
                {
                    MaxDamageColor = _takeDamageColorBase;
                    EqualDamageColor = _takeDamageColorBase;
                }
            }
            else
            {
                if (MinDamage > 0 || Math.Abs(MinDamage) < MaxDamage)
                {
                    var weight = MinDamage.Remap(-MaxDamage, MaxDamage, 0, 1);
                    MinDamageColor = weight.Lerp(_takeDamageColorBase, _dealDamageColorBase);
                }

                else if (MaxDamage < 0 || Math.Abs(MinDamage) > MaxDamage)
                {
                    var weight = MaxDamage.Remap(MinDamage, -MinDamage, 0, 1);
                    MaxDamageColor = weight.Lerp(_takeDamageColorBase, _dealDamageColorBase);
                }
            }

            TieLineBrush = new SolidColorBrush(Color.FromArgb(50, _tieColorBase.R, _tieColorBase.G, _tieColorBase.B));
            EqualDamageBrush = new SolidColorBrush(EqualDamageColor);
            MinDamageBrush = new SolidColorBrush(MinDamageColor);
            MaxDamageBrush = new SolidColorBrush(MaxDamageColor);
        }
    }
}
