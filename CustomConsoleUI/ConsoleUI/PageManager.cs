using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomConsoleUI.ConsoleUI
{
	internal class PageManager : IDisposable
	{
		// Object fields
		private readonly string title = "Interface_0";
		private readonly ConsoleColor specialBackground;
		private readonly ConsoleColor specialForeground;

		// Old configurations
		private static string LastTitle;
		private static ConsoleColor LastBackground;
		private static ConsoleColor LastForeground;

		// Public constructor for general customizability
		public PageManager(string title, ConsoleColor background, ConsoleColor foreground)
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
		private PageManager(ConsoleColor background, ConsoleColor foreground)
		{
			LastBackground = Console.BackgroundColor;
			LastForeground = Console.ForegroundColor;
			this.specialBackground = background;
			this.specialForeground = foreground;
		}

		// Constructor that takes a title and assigns it to the object
		// Last given console title is stored
		public PageManager(string title) : this(Console.BackgroundColor, Console.ForegroundColor)
		{
			LastTitle = Console.Title;
			this.title = title;

			this.Initialize();
		}

		// Constructor that stores last given title
		// The title is given to the new object
		public PageManager() : this(Console.BackgroundColor, Console.ForegroundColor)
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
        public static void Revert()
        {
            Console.Title = LastTitle;
            Console.BackgroundColor = LastBackground;
            Console.ForegroundColor = LastForeground;
        }

		#region "Cursor Visibility Control"
		public static void CursorOff()
			=> Console.CursorVisible = (Console.CursorVisible) ? false : Console.CursorVisible;

		public static void CursorOn()
			=> Console.CursorVisible = (!Console.CursorVisible) ? true : Console.CursorVisible;
		#endregion

		#region "User Input Handling"
		/// <summary>
		/// Waits for any key press
		/// </summary>
		/// <param name="intercept"></param>
		/// <returns>Returns a <b>ConsoleKeyInfo</b> object</returns>
		public static ConsoleKey KeyInput(bool intercept = true)
			=> Console.ReadKey(intercept).Key;

		private static T ReadInput<T>() where T : IConvertible
			=> (T)Convert.ChangeType(Console.ReadLine(), typeof(T));

		// Returns user input
		// Prints text at the beginning
		public static T Read<T>(string text) where T : IConvertible
		{
			T val;
			ConsoleRender.Write(text);
			CursorOn();

			val = ReadInput<T>();
			CursorOff();

			return val;
		}

		// Returns user input
		public static T Read<T>() where T : IConvertible
		{
			T val;
			CursorOn();
			val = ReadInput<T>();

			CursorOff();
			return val;
		}

		// Stores user input into reference
		// Prints text at the beginning
		public static void Read<T>(ref T val, string text) where T : IConvertible
		{
			ConsoleRender.Write(text);
			CursorOn();
			val = ReadInput<T>();
			CursorOff();
		}

		// Stores user input into reference
		public static void Read<T>(ref T val) where T : IConvertible
		{
			CursorOn();
			val = ReadInput<T>();
			CursorOff();
		}
		#endregion

		public override string ToString()
			=> $"{this.title}";

        public void Dispose()
			=> Navigation.depth--;
	}
}
