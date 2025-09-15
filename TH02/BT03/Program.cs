class Program
{
    static void Main()
    {
        Console.Write("Enter radius r: ");
        double r = double.Parse(Console.ReadLine() ?? "0");

        double area = Math.PI * r * r;
        Console.WriteLine($"Circle area = {area:F2}");
    }
}