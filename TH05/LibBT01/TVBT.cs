// Practical Exercise 05 - Library for Book Management
// This file contains the TVBT class, which includes properties, methods, and utilities for managing book information.

namespace LibBT01
{
    public class TVBT(string bookID, string title, int yearPublished, string publisher, int pageCount)
    {
        // Properties
        public string BookID { get; set; } = bookID;
        public string Title { get; set; } = title;
        public int YearPublished { get; set; } = yearPublished;
        public string Publisher { get; set; } = publisher;
        public int PageCount { get; set; } = pageCount;

        // Method to display book information
        public void DisplayInformation()
        {
            Console.WriteLine("\n--- Book Information ---");
            Console.WriteLine($"Book ID: {BookID}");
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Year: {YearPublished}");
            Console.WriteLine($"Publisher: {Publisher}");
            Console.WriteLine($"Pages: {PageCount}");
        }

        // Enumeration for days of the week
        // This enum represents the days from Sunday to Saturday
        public enum Days
        {
            Sunday, // Represents Sunday
            Monday, // Represents Monday
            Tuesday, // Represents Tuesday
            Wednesday, // Represents Wednesday
            Thursday, // Represents Thursday
            Friday, // Represents Friday
            Saturday // Represents Saturday
        }

        // Method to get family data
        // Returns a dictionary containing family members and their children
        public static Dictionary<string, List<string>> GetFamilyData()
        {
            return new Dictionary<string, List<string>>
            {
                { "Trung", new List<string> {} },
                { "Hung", new List<string> { "Loan", "Kim" } },
                { "Tuan", new List<string> { } }
            };
        }

        // Method for parameter passing example
        // Demonstrates the use of ref and out parameters
        public static void PassParameters(int x, ref int y, out int z)
        {
            y = x * y;
            z = x * x;
        }
    }
}
