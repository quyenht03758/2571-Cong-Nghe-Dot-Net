class Program
{
    static void Main()
    {
        int[] numbers = new int[10];
        int odd = 0, even = 0, divisibleBy3 = 0;

        Console.WriteLine("Enter 10 integers:");
        for (int i = 0; i < 10; i++)
        {
            numbers[i] = int.Parse(Console.ReadLine() ?? "0");

            if (numbers[i] % 2 == 0) even++;
            else odd++;

            if (numbers[i] % 3 == 0) divisibleBy3++;
        }

        Console.WriteLine($"Odd numbers: {odd}");
        Console.WriteLine($"Even numbers: {even}");
        Console.WriteLine($"Divisible by 3: {divisibleBy3}");
    }
}
