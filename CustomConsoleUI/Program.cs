using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomConsoleUI.ConsoleUI;

namespace CustomConsoleUI
{
    internal class Program
    {
		#region "Reusable Static Functions"
		static void endOfAction()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static ReturnAction PrintForever()
        {
            for (int i = 0; i < Console.WindowHeight-1; i++)
            {
                Console.WriteLine(new string('-', Console.WindowWidth-1));
            }

            endOfAction();
            return ReturnAction.Stay;
        }

        static ReturnAction helloWorld()
        {
            Console.WriteLine("Hello, World!");
            endOfAction();

            return ReturnAction.Stay;
        }

		#endregion

		#region "Pages and subpages"
		static void mainpage()
        {
            using (ChoiceForm main = new ChoiceForm("Main Page"))
            {
                main.AddAction("Hello", helloWorld);
                main.AddAction("Sub_0", SubPage0);
            
                main.Start(ChoicePattern.Columns);
            }
        }

        static ReturnAction SubPage0()
        {
            using (ChoiceForm subpage0 = new ChoiceForm("Sub_0", "Second floor"))
            {
                subpage0.AddAction("Hello", helloWorld);
                subpage0.AddAction("Sub_1", SubPage1);

                subpage0.Start(ChoicePattern.Columns);
            }

            return ReturnAction.Stay;
        }

        static ReturnAction SubPage1()
        {
            using (ChoiceForm subpage1 = new ChoiceForm("Sub_1", "Second floor\nFirst room"))
            {
                subpage1.AddAction("Hello", helloWorld);
                subpage1.AddAction("Hello", helloWorld);
                subpage1.AddAction("Test", SearchQuery0);

                subpage1.Start(ChoicePattern.Columns);
            }

            return ReturnAction.Stay;
        }
		#endregion

		static void Main(string[] args)
        {
            mainpage();
		}

        static ReturnAction queryAction(string test)
        {
            Console.Clear();
            Console.WriteLine(test);
            endOfAction();

            return ReturnAction.Stay;
        }

        static ReturnAction SearchQuery0()
        {
            using (SearchForm<string> form1 = new SearchForm<string>("Test Search 1", "First Floor"))
            {
				form1.action = delegate (string selected) {
                    Console.Clear();
                    Console.WriteLine(selected);
                    endOfAction();

                    return ReturnAction.Stay;
                };
				form1.AddQuery(
					new List<string>
					{
					"Omar",
					"Mohamed",
					"Ahmed",
					"Jasmine",
					"Tina",
					"Maxin",
					"Svetlana",
					"Olga",
					"Oregano",
					"Michael",
					"Marvin",
					"Vladimir"
					});

				form1.Start();
			}

            return ReturnAction.Stay;
        }
    }
}
