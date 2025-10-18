namespace LibBT01
{
    // Employee implements IPersonInfo via PersonBase + IComparable for natural sort
    public class Employee : PersonBase, IComparable<Employee>
    {
        public string? Name { get; set; }
        public string NullLabel { get; set; } = "Employee";

        public override string FullName => Name ?? $"<null> {NullLabel}";

        public virtual string? Department => null; // only managers use this

        public override void DisplayInfo()
        {
            // Default display: just FullName
            Console.WriteLine(FullName);
        }

        // Sort: names A→Z; nulls after names; if both null, by NullLabel
        public int CompareTo(Employee? other)
        {
            if (other is null) return 1;

            bool aNull = Name is null;
            bool bNull = other.Name is null;

            if (!aNull && !bNull) return string.Compare(Name, other.Name, StringComparison.Ordinal);
            if (aNull && !bNull) return 1;
            if (!aNull && bNull) return -1;

            return string.Compare(NullLabel, other.NullLabel, StringComparison.Ordinal);
        }
    }

    public class Manager : Employee
    {
        public override string? Department { get; }
        public Manager(string? name, string? dept)
        {
            Name = name;
            Department = dept;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine(Name is null
                ? $"<null> {NullLabel}"
                : $"{Name} (Dept: {Department ?? "<none>"})");
        }
    }

    // Alternate comparer to demonstrate IComparer<T>
    public sealed class EmployeeComparer : IComparer<Employee>
    {
        public int Compare(Employee? x, Employee? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            bool xNull = x.Name is null;
            bool yNull = y.Name is null;

            if (!xNull && !yNull) return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (xNull && !yNull) return 1;
            if (!xNull && yNull) return -1;

            return string.Compare(x.NullLabel, y.NullLabel, StringComparison.Ordinal);
        }
    }
}
