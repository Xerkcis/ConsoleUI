using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI
{
	internal static class Navigation
	{
		public static int depth = 0;
		public static int position = 0;
		private static int indexLimit = 0;

		// Resets navigation 
		public static void StartNew()
		{
			depth++;
			position = 0;
			indexLimit = 0;
		}

        // Updates navigation attributes
        public static void UpdateLimit(int listCount)
		{
			indexLimit = listCount-1;

			if (indexLimit < position) position = 0;
		}

		public static ReturnAction Stay()
			=> ReturnAction.Stay;

		public static ReturnAction Back()
		{
			position = 0;

			if (depth == 1) return ReturnAction.Exit;
			else return ReturnAction.Break;
		}

		public static bool Cycle(ConsoleKey direction)
		{
			if ((direction == ConsoleKey.UpArrow) || (direction == ConsoleKey.LeftArrow))
			{
				if (position <= 0)
				{
					position = indexLimit;
					return false;
				}

				position--;
				return true;
			}
			
			if ((direction == ConsoleKey.DownArrow) || (direction == ConsoleKey.RightArrow))
			{
                if (position >= indexLimit)
                {
                    position = 0;
                    return false;
                }

                position++;
                return true;
            }

			return false;
        }
	}
}
