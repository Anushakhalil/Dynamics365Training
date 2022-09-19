using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;


namespace Assignmentapp{
    class Atm{
        static void Main() {

            String[] rows;
            String[] cols;

            String path = @"C:\Users\Anusha.Khalil\Desktop\Book1";
            Int32 index = 1;

            rows = File.ReadAllLines(path);
            while (index < rows.Length) {
                cols = rows[index].Split(",");

                Console.WriteLine("Name:" + cols[0] + "\nRoll-No: " + cols[1] + "\nAge: " + cols[2]+ "\n");
                index++;

            }
            Console.ReadKey();
        }
    }
}