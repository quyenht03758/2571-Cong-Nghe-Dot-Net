using System;

namespace LibBT01
{
    // Optional shared base for people
    public abstract class PersonBase : IPersonInfo
    {
        public abstract string FullName { get; }

        public virtual void DisplayInfo()
        {
            Console.WriteLine(FullName);
        }
    }
}
