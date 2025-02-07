using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the number a = ");
            if (!Int32.TryParse(Console.ReadLine(), out var a))
            {
                PrintError("Not a number");
                return;
            }

            Console.Write("Enter the number b = ");
            if (!Int32.TryParse(Console.ReadLine(), out var b))
            {
                PrintError("Not a number");
                return;
            }

            Console.Write("Enter the operation ");
            var symbol = Console.ReadLine();
            var boolVar = true;
            if (symbol.Length == 0 || symbol.Length > 1 && !boolVar)
            {
                PrintError("Wrong sign!");
                return;
            }

            switch (symbol[0])
            {
                case '&':
                    PrintResult(a, b, '&', a & b);
                    break;
                case '|':
                    PrintResult(a, b, '|', a | b);
                    break;
                case '^':
                    PrintResult(a, b, '^', a ^ b);
                    break;
                default:
                    PrintError("Wrong sign!");
                    break;
            }
        }

        static void PrintResult(int a, int b, char operation, int result)
        {
            Console.WriteLine();
            Console.WriteLine($"Result of a {operation} b");
            Console.WriteLine($"Decimal: {Convert.ToString(result, toBase: 10)}");
            Console.WriteLine($"Binary: {Convert.ToString(result, toBase: 2)}");
            Console.WriteLine($"Hexadecimal: {Convert.ToString(result, toBase: 16)}");
        }
        static void PrintError(string errror)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errror);
            Console.ResetColor();
        }
    }
}
