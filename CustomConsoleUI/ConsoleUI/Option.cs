using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomConsoleUI.ConsoleUI
{
    public enum ReturnAction
    {
        Exit,
        Stay,   // This case is ignored
        Break,
        Throw
    }

    public delegate ReturnAction OptionDelegate();

    public class Option
    {
        private string name;
        public OptionDelegate optionDel;

        public static int idCount = 0;

        public static void exit()
        {
            Console.WriteLine();
            Environment.Exit(0);
        }

        public static void ThrowException()
            => throw new Exception($"Function delegate doesn't return type: {typeof(ReturnAction)}");

        public ReturnAction Throw()
            => throw new NotImplementedException($"Action field: \"{this.name}\" is not implemented");

        public Option(string name, OptionDelegate optionDelegate)
        {
            this.name = name;
            this.optionDel = optionDelegate;
        }

        public Option(string name)
        {
            this.name = name;
            this.optionDel = Throw;
        }

        public Option()
        {
            this.name = "NotImplemented";
            this.optionDel = this.Throw;
        }

        public override string ToString()
            => $"{this.name}";
    }
}
