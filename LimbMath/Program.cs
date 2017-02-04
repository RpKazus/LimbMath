using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LimbMath
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите своё выражение:");
                string message = Console.ReadLine();
                //Fraction f1 = new Fraction(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
                Fraction.MakeType = (int)Fraction.fraction_type.AbbreviatedAndExtracted;
                //Fraction f2 = new Fraction(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
                Limb lmb = new Limb() {MathName = "*", Values = new List<Limb>() { new Limb() {MathName = "2"}, new Limb() { MathName = "x" } } };
                DateTime date = DateTime.Now;
                message = message.Replace('.', ',');
                Limb nl = Limb.ConvertTextToLimb(message, 0);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(DateTime.Now - date);
                date = DateTime.Now;
                nl = nl.OpenBrackets();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(DateTime.Now - date);
                date = DateTime.Now;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(nl.ToString().Replace(',', '.'));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(DateTime.Now - date);
                Console.ReadLine();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;

                //string str = (f1).ToLimb().ToString();
                //Console.WriteLine(str);
            }
        }
    }
}

