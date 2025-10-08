class Program
{
    static void Main()
    {
        Console.Write("Enter number of elements: ");
        int n = int.Parse(Console.ReadLine() ?? "0");

        int[] A = new int[n];
        int[] B = new int[n];
        int[] C = new int[n];

        Console.WriteLine("Enter elements of array A:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"A[{i}] = ");
            A[i] = int.Parse(Console.ReadLine() ?? "0");
        }

        Console.WriteLine("Enter elements of array B:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"B[{i}] = ");
            B[i] = int.Parse(Console.ReadLine() ?? "0");
        }

        for (int i = 0; i < n; i++)
        {
            C[i] = A[i] + B[i];
        }

        Console.WriteLine("Result of A + B:");
        for (int i = 0; i < n; i++)
        {
            Console.Write(C[i] + " ");
        }
    }
}
