Console.WriteLine("CN   Hai  Ba   Tu   Nam  Sau  Bay");

int day = 1;
for (int week = 0; week < 5; week++)
{
    for (int i = 0; i < 7; i++)
    {
        if (day <= 30)
        {
            Console.Write($"{day,2}   "); // in số với độ rộng 2, thêm khoảng cách
            day++;
        }
    }
    Console.WriteLine();
}