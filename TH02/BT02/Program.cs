class Program
{
    static void Main()
    {
        Console.Write("Enter first integer (a): ");
        string? inputA = Console.ReadLine();
        int a = int.Parse(inputA ?? "0");

        Console.Write("Enter second integer (b): ");
        string? inputB = Console.ReadLine();
        int b = int.Parse(inputB ?? "0");

        int sum = a + b;
        Console.WriteLine($"The sum of {a} and {b} is: {sum}");
    }
}