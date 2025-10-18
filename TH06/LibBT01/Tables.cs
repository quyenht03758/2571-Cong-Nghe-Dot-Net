namespace LibBT01
{
    // Generic key-value printable table
    public interface IKeyValueTable<K, V>
    {
        string Title { get; }
        void Add(K key, V value);
        bool TryGet(K key, out V value);
        void PrintTable();
    }

    public abstract class KeyValueTableBase<K, V> : IKeyValueTable<K, V>
    {
        protected readonly Dictionary<K, V> Data = new();
        public abstract string Title { get; }

        public virtual void Add(K key, V value) => Data[key] = value;
        public virtual bool TryGet(K key, out V value) => Data.TryGetValue(key, out value);

        protected virtual IEnumerable<KeyValuePair<K, V>> Entries() => Data;

        public virtual void PrintTable()
        {
            ConsoleTable.Print(
                Title,
                new[] { "Key", "Value" },
                Entries().Select(kv => new[] { kv.Key?.ToString() ?? "", kv.Value?.ToString() ?? "" })
            );
        }
    }

    // Countries → Cities example
    public class CountryCitiesTable : KeyValueTableBase<string, string>
    {
        public override string Title => "COUNTRY - CITIES TABLE";

        public CountryCitiesTable()
        {
            Add("UK", "London, Manchester, Birmingham");
            Add("USA", "Chicago, New York, Washington");
            Add("India", "Mumbai, New Delhi, Pune");
        }
    }

    // Sorted variant (polymorphism)
    public class SortedCountryCitiesTable : CountryCitiesTable
    {
        protected override IEnumerable<KeyValuePair<string, string>> Entries()
            => Data.OrderBy(kv => kv.Key);
    }
}
