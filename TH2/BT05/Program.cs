class Program
{
    static void Main()
    {
        Console.Write("Enter side a: ");
        double a = double.Parse(Console.ReadLine() ?? "0");

        double totalDiagonal = 2 * a * Math.Sqrt(2);
        Console.WriteLine($"Total length of 2 diagonals = {totalDiagonal:F2}");
    }
}
