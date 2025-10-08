namespace BT01
{
    internal static class Functions
    {
        /// <summary>
        /// Checks if 3 sides form a valid triangle (triangle inequality).
        /// </summary>
        public static bool IsValidTriangle(double a, double b, double c) =>
            a > 0 && b > 0 && c > 0 &&
            a + b > c && a + c > b && b + c > a;

        /// <summary>
        /// Perimeter: P = a + b + c
        /// </summary>
        public static double Perimeter(double a, double b, double c) => a + b + c;

        /// <summary>
        /// Area using Heron's formula:
        /// S = sqrt(p*(p-a)*(p-b)*(p-c)), where p = P/2
        /// </summary>
        public static double Area(double a, double b, double c)
        {
            if (!IsValidTriangle(a, b, c))
                throw new ArgumentException("Invalid triangle sides.");
            double p = (a + b + c) / 2.0;
            double inside = p * (p - a) * (p - b) * (p - c);
            if (inside < 0) inside = 0; // guard tiny negative by FP rounding
            return Math.Sqrt(inside);
        }
    }
}
