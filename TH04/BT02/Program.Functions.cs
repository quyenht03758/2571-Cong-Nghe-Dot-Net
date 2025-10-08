namespace BT02
{
    internal static class Functions
    {
        /// <summary>
        /// Prints the multiplication table for a single number n (1..9).
        /// Example: MultiplicationTable(8)
        /// </summary>
        public static void MultiplicationTable(int n)
        {
            if (n < 1 || n > 9)
                throw new ArgumentOutOfRangeException(nameof(n), "Input must be between 1 and 9.");

            Console.WriteLine($"\nMultiplication Table of {n}");
            Console.WriteLine(new string('-', 32));
            for (int i = 1; i <= 9; i++)
            {
                Console.WriteLine($"{n,2} × {i,2} = {n * i,3}");
            }
        }
    }
}
