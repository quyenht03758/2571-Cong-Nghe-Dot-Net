namespace LibBT01
{
    // Tiny helper to print nice console tables (no NuGet needed)
    public static class ConsoleTable
    {
        public static void Print(string? title, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
        {
            var data = rows.ToList();
            int cols = headers.Count;
            var widths = new int[cols];

            for (int c = 0; c < cols; c++)
            {
                int body = data.Count == 0 ? 0 : data.Max(r => r[c]?.Length ?? 0);
                widths[c] = Math.Max(headers[c].Length, body);
            }

            string line = "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";
            string Row(IReadOnlyList<string> r) =>
                "| " + string.Join(" | ", Enumerable.Range(0, cols).Select(c => (r[c] ?? "").PadRight(widths[c]))) + " |";

            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('=', Math.Max(34, title!.Length + 6)));
                Console.WriteLine($"  {title}");
                Console.WriteLine(new string('=', Math.Max(34, title.Length + 6)));
                Console.ResetColor();
            }

            Console.WriteLine(line);
            Console.WriteLine(Row(headers));
            Console.WriteLine(line);
            foreach (var r in data) Console.WriteLine(Row(r));
            Console.WriteLine(line);
        }
    }
}
