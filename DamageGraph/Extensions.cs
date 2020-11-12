using System.Windows.Media;

namespace BobsGraph
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outLower"></param>
        /// <param name="outUpper"></param>
        /// <returns></returns>
        public static double Remap(this double value, double inLower, double inUpper, double outLower, double outUpper)
        {
            value -= inLower;
            value *= outUpper - outLower;
            value /= inUpper - inLower;
            value += outLower;

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outLower"></param>
        /// <param name="outUpper"></param>
        /// <returns></returns>
        public static float Remap(this int value, int inLower, int inUpper, int outLower, int outUpper)
        {
            float result = value;

            result -= inLower;
            result *= outUpper - outLower;
            result /= inUpper - inLower;
            result += outLower;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static Color Lerp(this float weight, Color a, Color b)
        {
            return Color.FromArgb(
                (byte)(a.A * (1 - weight) + b.A * weight),
                (byte)(a.R * (1 - weight) + b.R * weight),
                (byte)(a.G * (1 - weight) + b.G * weight),
                (byte)(a.B * (1 - weight) + b.B * weight)
            );
        }
    }
}
