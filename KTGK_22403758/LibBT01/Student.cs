/*
 * LibBT01 - Student Class Library
 * This library contains the Student class with properties and methods
 * Note: Vietnamese attribute names from requirements are mapped as follows:
 *   - MSSV (Mã Số Sinh Viên) → StudentId
 *   - HoSV (Họ Sinh Viên) → LastName
 *   - TenSV (Tên Sinh Viên) → FirstName
 *   - NgaySinh → DateOfBirth
 */

using System;

namespace LibBT01
{
    /// <summary>
    /// Represents a student with basic information
    /// Vietnamese equivalent: SinhVien class from requirements
    /// </summary>
    public class Student
    {
        // Properties
        /// <summary>
        /// Student ID
        /// Vietnamese equivalent: MSSV (Mã Số Sinh Viên)
        /// </summary>
        public string StudentId { get; set; }

        /// <summary>
        /// Student's last name
        /// Vietnamese equivalent: HoSV (Họ Sinh Viên)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Student's first name
        /// Vietnamese equivalent: TenSV (Tên Sinh Viên)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Student's date of birth
        /// Vietnamese equivalent: NgaySinh
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Default constructor - initializes with empty values
        /// </summary>
        public Student()
        {
            StudentId = "";
            LastName = "";
            FirstName = "";
            DateOfBirth = DateTime.Now;
        }

        /// <summary>
        /// Parameterized constructor - initializes with provided values
        /// </summary>
        /// <param name="studentId">Student ID (MSSV)</param>
        /// <param name="lastName">Student's last name (HoSV)</param>
        /// <param name="firstName">Student's first name (TenSV)</param>
        /// <param name="dateOfBirth">Student's date of birth (NgaySinh)</param>
        public Student(string studentId, string lastName, string firstName, DateTime dateOfBirth)
        {
            StudentId = studentId;
            LastName = lastName;
            FirstName = firstName;
            DateOfBirth = dateOfBirth;
        }

        /// <summary>
        /// Writes student information to console with formatted output
        /// </summary>
        public void WriteToConsole()
        {
            Console.WriteLine($"{StudentId,-10} {LastName,-20} {FirstName,-10} {DateOfBirth:dd/MM/yyyy}");
        }
    }
}
