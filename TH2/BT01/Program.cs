using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

// column widths
const int colType = -8;
const int colBytes = -14;
const int colMin = -28;
const int colMax = -28;

void Line() => Console.WriteLine(new string('-', 80));

void Header()
{
    Line();
    Console.WriteLine(
        $"{"Type",colType}{"Byte(s) of memory",colBytes}{"Min",colMin}{"Max",colMax}");
    Line();
}

// Format helper for any numeric value type
string F<T>(T value) where T : struct, IFormattable => value switch
{
    float f => f.ToString("G", CultureInfo.InvariantCulture),
    double d => d.ToString("G", CultureInfo.InvariantCulture),
    decimal m => m.ToString(CultureInfo.InvariantCulture),
    _ => ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture)
};

// Generic row printer (no 'object' involved)
void Row<T>(string type, int bytes, T min, T max) where T : struct, IFormattable =>
    Console.WriteLine($"{type,colType}{bytes,colBytes}{F(min),colMin}{F(max),colMax}");

Header();

// signed/unsigned integers
Row("sbyte", sizeof(sbyte), sbyte.MinValue, sbyte.MaxValue);
Row("byte", sizeof(byte), byte.MinValue, byte.MaxValue);
Row("short", sizeof(short), short.MinValue, short.MaxValue);
Row("ushort", sizeof(ushort), ushort.MinValue, ushort.MaxValue);
Row("int", sizeof(int), int.MinValue, int.MaxValue);
Row("uint", sizeof(uint), uint.MinValue, uint.MaxValue);
Row("long", sizeof(long), long.MinValue, long.MaxValue);
Row("ulong", sizeof(ulong), ulong.MinValue, ulong.MaxValue);

// floating point & decimal
Row("float", sizeof(float), float.MinValue, float.MaxValue);
Row("double", sizeof(double), double.MinValue, double.MaxValue);
Row("decimal", sizeof(decimal), decimal.MinValue, decimal.MaxValue);

Line();
