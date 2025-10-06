using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI.ConsoleUI
{
	// For render style
	internal delegate void RenderDelegate<T>(List<T> lines);

	public enum ChoicePattern
	{
		Rows,
		Columns
	}

	internal class ChoiceForm : IDisposable
	{
		public string PageDescription;
		private PageStyle PageConsole;
		private List<Option> options;

		private ChoiceForm()
		{
			this.PageConsole = new PageStyle("Page " + $"{Navigation.depth}");
			this.options = new List<Option>();
		}

		public ChoiceForm(string Title, string Description) : this()
		{
			this.PageConsole = new PageStyle(Title);
			this.PageDescription = Description;
		}

		public ChoiceForm(string Description) : this()
		{
			this.PageConsole = new PageStyle();
			this.PageDescription = Description;
		}

		// Default empty action
		public void AddAction()
			=> this.options.Add(new Option());

		// Option action functions
		public void AddAction(string DisplayName, OptionDelegate ActionDelegate)
			=> this.options.Add(new Option(DisplayName, ActionDelegate));

		public void AddAction(Option option)
			=> this.options.Add(option);
		
		public void AddAction(List<Option> options)
			=> options.ForEach(option => this.options.Add(option));

		public List<Option> GetOptions()
			=> this.options;

		public void OverrideOptions(List<Option> OptionsList)
			=> OptionsList.CopyTo(this.options.ToArray());
		
		public void DeleteOption(int index)
			=> this.options.RemoveAt(index);
		// Option action functions

		private void CycleSelection(string description, List<Option> options, ChoicePattern style)
		{
			// Initialization
			ConsoleKey inputKey;
			OptionDelegate runAction;
			RenderDelegate<Option> renderManager = delegate { throw new Exception("Cycle terminated at startup"); };
			ReturnAction returnAction;

			// Resetting position to zero
			Navigation.StartNew();

			// One-time assignment of delegate
			switch(style)
			{
				case ChoicePattern.Rows:
					renderManager = ConsoleRender.RenderOptionsRows;
					break;
				case ChoicePattern.Columns:
					renderManager = ConsoleRender.RenderOptionsCols;
					break;
			}

			// Main rendering cycle loop
			while (true)
			{
				// Updates navigation attributes
				Navigation.UpdateLimit(options.Count);

				// Rendering block
				ConsoleRender.ClearInterface();
				ConsoleRender.SetCursorPosition(0, 0);
				
				ConsoleRender.WriteLine(description + $"\nDepth: {Navigation.depth}");
				renderManager(options);

				// Capturing cursor row position after render
				// Used in cleaning up the interface
				ConsoleRender.CaptureRowPosition();

				// Defaulting action to skip
				// Getting key input from user
				runAction = Navigation.Stay;
				inputKey = PageStyle.KeyInput();

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
				ConsoleRender.ClearFromLastRow();
			}

			PageStyle.Revert();       // Revert console style after exit
		}

		public void Start(ChoicePattern pattern = ChoicePattern.Rows)
			=> this.CycleSelection(this.PageDescription, this.options, pattern);

		public void Dispose()
		{
			this.PageDescription = null;
			this.PageConsole.Dispose();
			this.PageConsole = null;
			this.options.Clear();
			this.options = null;
		}
	}
}
