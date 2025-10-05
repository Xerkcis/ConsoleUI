using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomConsoleUI
{
	internal class ConsoleUI : IDisposable
	{
		// Object fields
		private readonly string title = "Interface_0";
		private readonly ConsoleColor specialBackground;
		private readonly ConsoleColor specialForeground;

		// Old configurations
		private static string LastTitle;
		private static ConsoleColor LastBackground;
		private static ConsoleColor LastForeground;

		// For inversion of colors when highlighting
        private static bool Inverted = false;

		// Public constructor for general customizability
		public ConsoleUI(string title, ConsoleColor background, ConsoleColor foreground)
		{
			LastTitle = Console.Title;
			LastBackground = Console.BackgroundColor;
			LastForeground = Console.ForegroundColor;

			this.title = title;
			this.specialBackground = background;
			this.specialForeground = foreground;

			this.Initialize();
		}

		// Private constructor for color customizability only
		// Used for the (take-only-title) constructors
		private ConsoleUI(ConsoleColor background, ConsoleColor foreground)
		{
			LastBackground = Console.BackgroundColor;
			LastForeground = Console.ForegroundColor;
			this.specialBackground = background;
			this.specialForeground = foreground;
		}

		// Constructor that takes a title and assigns it to the object
		// Last given console title is stored
		public ConsoleUI(string title) : this(Console.BackgroundColor, Console.ForegroundColor)
		{
			LastTitle = Console.Title;
			this.title = title;

			this.Initialize();
		}

		// Constructor that stores last given title
		// The title is given to the new object
		public ConsoleUI() : this(Console.BackgroundColor, Console.ForegroundColor)
		{
			LastTitle = Console.Title;
			this.title = LastTitle;

			this.Initialize();
		}

		// Initialize current page
		private void Initialize()
		{
			Console.Title = this.title;
			Console.ForegroundColor = this.specialForeground;
			Console.BackgroundColor = this.specialBackground;
			Console.CursorVisible = false;
		}

        // Revert to last page style
        public static void RevertPage()
        {
            Console.Title = LastTitle;
            Console.BackgroundColor = LastBackground;
            Console.ForegroundColor = LastForeground;
        }

        // Swaps background and foreground colors
        public static void InvertColors()
		{
			ConsoleColor temp = Console.BackgroundColor;

			Console.BackgroundColor = Console.ForegroundColor;
			Console.ForegroundColor = temp;

			Inverted = !Inverted;
		}

		/// <summary>
		/// Waits for any key press
		/// </summary>
		/// <param name="intercept"></param>
		/// <returns>Returns a <b>ConsoleKeyInfo</b> object</returns>
		public static ConsoleKeyInfo AwaitInput(bool intercept = true)
			=> Console.ReadKey(intercept);

        /// <summary>
        /// Manages user input for cycling and execution of options
        /// </summary>
        /// <param name="description"></param>
        /// <param name="options"></param>
        public static void CycleSelection(string description, List<Option> options, RenderPattern style = RenderPattern.Rows)
        {
            // Initialization
            ConsoleKey inputKey;
            OptionDelegate runAction;
            ReturnAction returnAction;

            // Resetting position to zero
            Navigation.reset();

            // Main rendering cycle loop
            while (true)
            {
                // Continous reconfiguration of limit
                Navigation.setLimit(options.Count);

                // Rendering block
                ConsoleRender.ClearInterface();
                ConsoleRender.SetCursorPosition(0, 0);

                ConsoleRender.WriteLine(description + $"\nDepth: {Navigation.depth}");
                switch (style)
                {
                    case RenderPattern.Rows:
                        ConsoleRender.RenderOptionsRows(options);
                        break;
                    case RenderPattern.Columns:
                        ConsoleRender.RenderOptionsCols(options);
                        break;
                }

                // Capturing cursor row position after render
                // Used in cleaning up the interface
                ConsoleRender.CaptureRowPosition();

                // Defaulting action to skip
                // Getting key input from user
                runAction = Navigation.Stay;
                inputKey = AwaitInput().Key;

                // Key decision making
                if (inputKey == ConsoleKey.Enter) runAction = options[Navigation.position].optionDel;
                else if ((inputKey == ConsoleKey.Backspace)) runAction = Navigation.Back;
                else Navigation.Cycle(inputKey);

                // May cause runtime issue
                // Developer must add a way to escape loops
                returnAction = runAction();

                // Extra layer of control
                if (returnAction == ReturnAction.Exit) Option.exit();
                else if (returnAction == ReturnAction.Break) break;
                else if (returnAction == ReturnAction.Throw) Option.ThrowException(); // For any errors

                // Final Drawing
                ConsoleRender.ClearFromLastPosition();
            }

            RevertPage();       // Revert console style after exit
        }

        public void Dispose()
			=> Navigation.depth--;
	}
}
