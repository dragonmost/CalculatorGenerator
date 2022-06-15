using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace CalculatorGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            if (args.Length == 0) args = new[] { "-a", "255", "255" };
#endif

            var provider = new CSharpCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters(new[] { "System.dll" });
#if DEBUG
            compilerParameters.OutputAssembly = "Calculator.dll";
#endif
            compilerParameters.GenerateInMemory = true;
            compilerParameters.CompilerOptions = "/optimize";
            CompilerResults result = provider.CompileAssemblyFromSource(compilerParameters, new[] { Source });

            if (result.Errors.HasErrors)
            {
                Console.WriteLine("Errors");
                Console.ReadKey();
                return;
            }

            Type calculatorType = result.CompiledAssembly.GetModules()[0].GetType("Calculator.Program");
            var calculator = Activator.CreateInstance(calculatorType);
            var mainMethod = calculatorType.GetMethod("Main");

            mainMethod.Invoke(calculator, new[] { args });
        }

        private static string Source
        {
            get
            {
                return @"using System;
namespace Calculator 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Warming up the vacuum tubes..."");
                WarmingUp();
                Console.WriteLine(""Ready to go!"");

                while (true)
                {
                    if (args == null || args.Length == 0)
                    {
                        args = Console.ReadLine().Split(' ');
                    }

                    int result;
                    switch(args[0].ToLower())
                    {
                        case ""--help"":
                        case ""-h"":
                            GetHelp();
                            break;
                        case ""--add"":
                        case ""-a"":
                            Console.Write(args[1].ToString() + "" + "" + args[2].ToString() + "" = "");
                            result = Addition(int.Parse(args[1]), int.Parse(args[2]));
                            Console.WriteLine(result == int.MinValue ? ""error"" : result.ToString());
                            break;
                        case ""--subtract"":
                        case ""-s"":
                            Console.Write(args[1].ToString() + "" - "" + args[2].ToString() + "" = "");
                            result = Subtraction(int.Parse(args[1]), int.Parse(args[2]));
                            Console.WriteLine(result == int.MinValue ? ""error"" : result.ToString());
                            break;
                        case ""--quit"":
                        case ""-q"":
                            return;
                    }

                    args = null;
                }
            }
            catch (Exception ex)
            {
                ConsoleColor defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = defaultColor;
            }
        }
        
        private static void WarmingUp()
        {
            Addition(0, 0);
            Subtraction(0, 0);
        }

        private static int Addition(int v1, int v2)
        {
            {0}
        }

        private static int Subtraction(int v1, int v2)
        {
            {1}
        }

        private static void GetHelp()
        {
            Console.Write(@""--help -h                 Show help
--add -a <v1, v2>         Addition
--subtract -s <v1, v2>    subtraction
"");
        }
    }
}".Replace("{0}", string.Join(Environment.NewLine + "\t\t", GenerateAddition().ToArray())).Replace("{1}", string.Join(Environment.NewLine + "\t\t", GenerateSubtraction().ToArray()));
            }
        }

        private static IEnumerable<string> GenerateAddition()
        {
            for (int v1 = 0; v1 <= byte.MaxValue; v1++)
            {
                for (int v2 = 0; v2 <= byte.MaxValue; v2++)
                {
                    yield return $"if (v1 == {v1} && v2 == {v2}) return {v1 + v2};";
                }
            }

            yield return $"else return int.MinValue;";
        }

        private static IEnumerable<string> GenerateSubtraction()
        {
            for (int v1 = 0; v1 <= byte.MaxValue; v1++)
            {
                for (int v2 = v1; v2 <= byte.MaxValue; v2++)
                {
                    yield return $"if (v1 == {v1} && v2 == {v2}) return {v1 - v2};";
                }
            }

            yield return $"else return int.MinValue;";
        }
    }
}
