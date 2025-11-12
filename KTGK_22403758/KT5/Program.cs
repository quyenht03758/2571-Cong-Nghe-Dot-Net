/*
 * KT5 - Student Information Display
 * This program uses the Student class from LibBT01 library to display student information
 * Note: Using English class names as per requirements mapping
 * Features: Display initial students, add new students interactively
 */

using System;
using System.Collections.Generic;
using System.Text;
using LibBT01;

namespace KT5;

class Program
{
    static void Main(string[] args)
    {
        // Set console encoding to UTF-8 to support Vietnamese characters in data
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                STUDENT INFORMATION MANAGEMENT SYSTEM                       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Create list of students with initial data from requirements
        // Data format: StudentId (MSSV), LastName (HoSV), FirstName (TenSV), DateOfBirth (NgaySinh)
        List<Student> students = new List<Student>
        {
            new Student("221477", "Cao Nhat", "Ban", new DateTime(2005, 12, 15)),
            new Student("222182", "Ong Huynh", "Huy", new DateTime(2005, 5, 15)),
            new Student("222630", "Nguyen Thoai", "Vy", new DateTime(2005, 4, 23))
        };

        bool exit = false;
        while (!exit)
        {
            // Display current student list
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Current Student List:");
            Console.ResetColor();
            DisplayStudentTable(students.ToArray());

            // Display statistics
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Total students: {students.Count}");
            Console.ResetColor();

            // Display menu
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                              MENU OPTIONS                                  ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine("  1. Add new student");
            Console.WriteLine("  2. Exit program");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your choice (1-2): ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNewStudent(students);
                    break;
                case "2":
                    exit = true;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid choice! Please enter 1 or 2.");
                    Console.ResetColor();
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    break;
            }

            Console.Clear();
            // Redisplay header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                STUDENT INFORMATION MANAGEMENT SYSTEM                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Exit message
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Thank you for using Student Information Management System!");
        Console.ResetColor();
    }

    /// <summary>
    /// Adds a new student to the list by prompting user for input
    /// </summary>
    /// <param name="students">The list of students to add to</param>
    static void AddNewStudent(List<Student> students)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════");
        Console.WriteLine("                           ADD NEW STUDENT                                  ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            // Get Student ID
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Student ID (MSSV): ");
            Console.ResetColor();
            string? studentId = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                throw new ArgumentException("Student ID cannot be empty!");
            }

            // Get Last Name
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Last Name (HoSV): ");
            Console.ResetColor();
            string? lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last Name cannot be empty!");
            }

            // Get First Name
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter First Name (TenSV): ");
            Console.ResetColor();
            string? firstName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First Name cannot be empty!");
            }

            // Get Date of Birth
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Date of Birth (dd/MM/yyyy): ");
            Console.ResetColor();
            string? dobInput = Console.ReadLine();
            DateTime dateOfBirth = DateTime.ParseExact(dobInput ?? "", "dd/MM/yyyy", null);

            // Create new student and add to list
            Student newStudent = new Student(studentId, lastName, firstName, dateOfBirth);
            students.Add(newStudent);

            // Success message
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Student added successfully!");
            Console.ResetColor();
        }
        catch (FormatException)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Invalid date format! Please use dd/MM/yyyy format.");
            Console.ResetColor();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Displays student information in a formatted table with colors
    /// </summary>
    /// <param name="students">Variable number of student objects to display</param>
    static void DisplayStudentTable(params Student[] students)
    {
        // Define column widths for proper alignment
        int mssvWidth = 12;
        int hoWidth = 22;
        int tenWidth = 20;
        int dateWidth = 18;
        int totalWidth = mssvWidth + hoWidth + tenWidth + dateWidth + 7; // +7 for separators

        // Display table top border
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("┌" + new string('─', mssvWidth) + "┬" +
                         new string('─', hoWidth) + "┬" +
                         new string('─', tenWidth) + "┬" +
                         new string('─', dateWidth) + "┐");
        Console.ResetColor();

        // Display table header
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("│");
        Console.Write(CenterText("Student ID", mssvWidth));
        Console.Write("│");
        Console.Write(CenterText("Last Name", hoWidth));
        Console.Write("│");
        Console.Write(CenterText("First Name", tenWidth));
        Console.Write("│");
        Console.Write(CenterText("Date of Birth", dateWidth));
        Console.WriteLine("│");
        Console.ResetColor();

        // Display header separator
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("├" + new string('─', mssvWidth) + "┼" +
                         new string('─', hoWidth) + "┼" +
                         new string('─', tenWidth) + "┼" +
                         new string('─', dateWidth) + "┤");
        Console.ResetColor();

        // Display each student's data
        foreach (var student in students)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("│ ");
            Console.Write(PadRight(student.StudentId, mssvWidth - 2));
            Console.Write(" │ ");
            Console.Write(PadRight(student.LastName, hoWidth - 2));
            Console.Write(" │ ");
            Console.Write(PadRight(student.FirstName, tenWidth - 2));
            Console.Write(" │ ");
            Console.Write(CenterText(student.DateOfBirth.ToString("dd/MM/yyyy"), dateWidth));
            Console.WriteLine("│");
            Console.ResetColor();
        }

        // Display table bottom border
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└" + new string('─', mssvWidth) + "┴" +
                         new string('─', hoWidth) + "┴" +
                         new string('─', tenWidth) + "┴" +
                         new string('─', dateWidth) + "┘");
        Console.ResetColor();
    }

    /// <summary>
    /// Centers text within a specified width
    /// </summary>
    /// <param name="text">The text to center</param>
    /// <param name="width">The total width</param>
    /// <returns>Centered text with padding</returns>
    static string CenterText(string text, int width)
    {
        if (text.Length >= width)
            return text.Substring(0, width);

        int totalPadding = width - text.Length;
        int leftPadding = totalPadding / 2;
        int rightPadding = totalPadding - leftPadding;

        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
    }

    /// <summary>
    /// Pads text to the right to fit specified width
    /// </summary>
    /// <param name="text">The text to pad</param>
    /// <param name="width">The total width</param>
    /// <returns>Right-padded text</returns>
    static string PadRight(string text, int width)
    {
        if (text.Length >= width)
            return text.Substring(0, width);

        return text + new string(' ', width - text.Length);
    }
}
