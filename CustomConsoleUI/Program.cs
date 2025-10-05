using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI
{
    internal class Program
    {
        static void endOfAction()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static ReturnAction helloWorld()
        {
            Console.WriteLine("Hello, World!");
            endOfAction();

            return ReturnAction.Stay;
        }

        static ReturnAction SubPage0()
        {
            using (Page subpage0 = new Page("Sub_0", "Second floor"))
            {
                subpage0.options = new List<Option>
                {
                    new Option("Hello", helloWorld),
                    new Option("Hello", helloWorld),
                    new Option("Sub1", SubPage1)
                };

                subpage0.RenderOptions(RenderPattern.Columns);
            }

            return ReturnAction.Stay;
        }

        static ReturnAction SubPage1()
        {
            using (Page subpage1 = new Page("Sub_1", "Second floor\nFirst room"))
            {
                subpage1.options = new List<Option>
                {
                    new Option("Hello", helloWorld),
                    new Option("Hello", helloWorld),
                    new Option()
                };

                subpage1.RenderOptions(RenderPattern.Columns);
            }

            return ReturnAction.Stay;
        }

        static void Main(string[] args)
        {
            using (Page mainpage = new Page("Test_1", "Hello"))
            {
                mainpage.options = new List<Option>
                {
                    new Option("Sub0_0", SubPage0),
                    new Option("Sub0_1", SubPage0),
                    new Option("Hello", helloWorld),
                    new Option("Sub0_2", SubPage0)
                };

                mainpage.RenderOptions(RenderPattern.Columns);
            }
        }
    }
}
