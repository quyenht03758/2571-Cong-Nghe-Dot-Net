namespace LibBT01
{
    public class Student : PersonBase
    {
        public string StudentId { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public DateTime BirthDate { get; set; }

        public override string FullName => $"{LastName} {FirstName}".Trim();

        // Table row helper
        public string[] ToRow(string dateFmt = "MM/dd/yyyy") =>
            new[] { StudentId, LastName, FirstName, BirthDate.ToString(dateFmt) };

        // Pretty one-line console output
        public override void DisplayInfo()
        {
            Console.WriteLine($"{StudentId,-10} {FullName,-25} {BirthDate:MM/dd/yyyy}");
        }
    }
}
