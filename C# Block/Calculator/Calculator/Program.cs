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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not a number");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter the number b = ");
            if (!Int32.TryParse(Console.ReadLine(), out var b))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not a number");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter the operation ");
            var symbol = Console.ReadLine();
            var boolVar = true;
            if (symbol.Length == 0 || symbol.Length > 1 && !boolVar)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Wrong sign!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine();

            switch (symbol[0])
            {
                case '&':
                    resultInConsole(a, b, '&', a & b);
                    break;
                case '|':
                    resultInConsole(a, b, '|', a | b);
                    break;
                case '^':
                    resultInConsole(a, b, '^', a ^ b);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong sign!");
                    Console.ResetColor();
                    break;
            }
        }

        static void resultInConsole(int a, int b, char operation, int result)
        {
            Console.WriteLine($"Result of a {operation} b");
            Console.WriteLine($"Decimal: {Convert.ToString(result, toBase: 10)}");
            Console.WriteLine($"Binary: {Convert.ToString(result, toBase: 2)}");
            Console.WriteLine($"Hexadecimal: {Convert.ToString(result, toBase: 16)}");
        }
    }
}
