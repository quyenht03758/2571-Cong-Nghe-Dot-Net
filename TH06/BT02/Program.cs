using LibBT01;

namespace BT02
{
    class Program
    {
        static void Main()
        {
            var students = new List<Student>
            {
                new() { StudentId="221477", LastName="Cao Nhat",     FirstName="Ban", BirthDate=new DateTime(2005,12,15) },
                new() { StudentId="222182", LastName="Ong Huynh",    FirstName="Huy", BirthDate=new DateTime(2005,05,15) },
                new() { StudentId="222630", LastName="Nguyen Thoai", FirstName="Vy",  BirthDate=new DateTime(2005,04,23) },
            };

            ConsoleTable.Print(
                "STUDENT LIST",
                new[] { "StudentID", "Last Name", "First Name", "Birth Date" },
                students.ConvertAll(s => s.ToRow("MM/dd/yyyy"))
            );

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
