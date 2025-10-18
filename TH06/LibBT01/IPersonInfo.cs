namespace LibBT01
{
    // Common contract for all person-like entities
    public interface IPersonInfo
    {
        string FullName { get; }
        void DisplayInfo();
    }
}
