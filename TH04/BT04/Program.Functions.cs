namespace BT04
{
    internal static class Functions
    {
        /// <summary>
        /// Sum of two digits of a two-digit number (10..99).
        /// Example: 45 → 4 + 5 = 9
        /// </summary>
        public static int SumTwoDigits(int number)
        {
            number = Math.Abs(number);
            if (number < 10 || number > 99)
                throw new ArgumentOutOfRangeException(nameof(number), "Input must be a two-digit number (10..99).");

            int tens = number / 10;
            int ones = number % 10;
            return tens + ones;
        }

        /// <summary>
        /// (Optional) Sum of all digits for any integer.
        /// </summary>
        public static int SumAllDigits(int number)
        {
            number = Math.Abs(number);
            int sum = 0;
            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }
            return sum;
        }
    }
}
