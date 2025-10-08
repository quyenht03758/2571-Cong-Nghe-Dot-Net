class Program
{
    static void Main()
    {
        int rows = 5; // fixed rows, as in the exercise

        for (int i = 1; i <= rows; i++)
        {
            // print spaces
            for (int j = 1; j <= rows - i; j++)
            {
                Console.Write(" ");
            }

            // print stars
            for (int k = 1; k <= (2 * i - 1); k++)
            {
                Console.Write("*");
            }

            Console.WriteLine();
        }
    }
}
