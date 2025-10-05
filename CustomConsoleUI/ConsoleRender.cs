using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI
{
	public enum RenderMode
	{
		normal,
		highlight
	}

	internal static class ConsoleRender
	{
		// Old cursor position
		private static int LastColumn = Console.CursorLeft;
		private static int LastRow = Console.CursorTop;
        private static bool inverted = false;

        private static void InvertColors()
        {
            ConsoleColor tempBackground = Console.BackgroundColor;

            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = tempBackground;

            inverted = !inverted;
        }

        public static void ClearInterface()
		{
			CaptureRowPosition();
			SetCursorPosition(0, 0);

			for (int i = 0; i < LastRow; i++)
				Console.WriteLine(new String(' ', Console.WindowWidth - 1));
		}

        // Captures last column position of console cursor
        public static void CaptureColumnPosition()
            => LastColumn = Console.CursorLeft;

        // Captures last row position of console cursor
        public static void CaptureRowPosition()
			=> LastRow = Console.CursorTop;

        // Clears from last row position until current
        public static void ClearFromLastRow()
		{
			int CurrentRow = Console.CursorTop;
			SetCursorPosition(0, LastRow);

			for (int i = LastRow; i < CurrentRow; i++)
				Console.WriteLine(new string(' ', Console.WindowWidth - 1));
		}

		public static void SetCursorPosition(int x, int y)
			=> Console.SetCursorPosition(x, y);

		public static void Write()
			=> Console.Write(" ");
		public static void Write<T>(T text)
			=> Console.Write($"{text}");

        public static void Write<T>(T text, RenderMode mode = RenderMode.normal)
        {
            switch (mode)
            {
                case RenderMode.normal:
                    Write(text);
                    break;
                case RenderMode.highlight:
                    InvertColors();
                    Write(text);
                    break;
            }

            if (inverted) InvertColors();
        }

		public static void WriteLine()
			=> Console.WriteLine();

        public static void WriteLine<T>(T text)
            => Console.WriteLine($"{text}");

        public static void WriteLine<T>(T text, RenderMode mode = RenderMode.normal)
        {
            switch (mode)
            {
                case RenderMode.normal:
                    WriteLine(text);
                    break;
                case RenderMode.highlight:
                    InvertColors();
                    WriteLine(text);
                    break;
            }

            if (inverted) InvertColors();
        }

        /// <summary>
        /// Renders Options in rows
        /// </summary>
        /// <param name="options"></param>
        public static void RenderOptionsRows<T>(List<T> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (Navigation.position == i) WriteLine(">" + options[i], RenderMode.highlight);
                else WriteLine(options[i]);
            }
        }

        /// <summary>
        /// Renders Options in Columns
        /// </summary>
        /// <param name="description"></param>
        /// <param name="options"></param>
        public static void RenderOptionsCols<T>(List<T> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (Navigation.position == i) ConsoleRender.Write(options[i], RenderMode.highlight);
                else Write(options[i]);
                if (i != options.Count - 1) Write(" || ");
                else WriteLine();
            }
        }
    }
}
