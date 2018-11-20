using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Default.Interpreter.Calc;

namespace Interpreter.Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            String runClass = "StandardExpression";

            Console.WriteLine("Test " + runClass);
            Console.WriteLine("====================================");
            switch (runClass)
            {
                case "StandardExpression": new CalculatorClient().Run(); break;
            }

            Console.ReadKey();
        }
    }
}
