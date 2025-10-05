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

        static ReturnAction SubPage1()
        {
            using (ChoiceForm subpage1 = new ChoiceForm("Sub_1", "Second floor\nFirst room"))
            {
                subpage1.AddAction("Hello", helloWorld);
                subpage1.AddAction("Hello", helloWorld);
                subpage1.AddAction(new Option());

                subpage1.StartPage(ChoicePattern.Columns);
            }

            return ReturnAction.Stay;
        }

        static ReturnAction SubPage0()
        {
            using (ChoiceForm subpage0 = new ChoiceForm("Sub_0", "Second floor"))
            {
                subpage0.AddAction("Hello", helloWorld);
                subpage0.AddAction("Sub_1", SubPage1);

                subpage0.StartPage(ChoicePattern.Columns);
            }

            return ReturnAction.Stay;
        }

        static void mainpage()
        {
            using (ChoiceForm main = new ChoiceForm("Main Page"))
            {
                main.AddAction("Hello", helloWorld);
                main.AddAction("Sub_0", SubPage0);

                main.StartPage(ChoicePattern.Columns);
            }
        }

        static void Main(string[] args)
        {
            mainpage();
        }
    }
}
