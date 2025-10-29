using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI.ConsoleUI.Form
{
	// For render style
	internal delegate void RenderDelegate<T>(List<T> lines);

	public enum ChoicePattern
	{
		Rows,
		Columns
	}

	public class ChoiceForm : IDisposable
	{
		public string PageDescription;
		private PageManager Page;
		private List<Option> options;
		private ChoicePattern renderMode;

		private ChoiceForm()
		{
			this.Page = new PageManager($"Page {Navigation.depth}");
			this.options = new List<Option>();
		}

		public ChoiceForm(string Title, string Description) : this()
		{
			this.Page = new PageManager(Title);
			this.PageDescription = Description;
		}

		public ChoiceForm(string Description) : this()
		{
			this.Page = new PageManager($"Page {Navigation.depth}");
			this.PageDescription = Description;
		}

		public void Rows()
			=> this.renderMode = ChoicePattern.Rows;

		public void Columns()
			=> this.renderMode = ChoicePattern.Columns;

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
			ConsoleKey KeyInput;
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
				ConsoleRender.ClearAll();
				
				ConsoleRender.WriteLine(description + $"\nDepth: {Navigation.depth}");
				renderManager(options);

				// Capturing cursor row position after render
				// Used in cleaning up the interface
				ConsoleRender.CaptureRowPosition();

				// Defaulting action to skip
				// Getting key input from user
				runAction = Navigation.Stay;
				KeyInput = PageManager.KeyInput().Key;

				switch (KeyInput)
				{
					case ConsoleKey.Enter:
						runAction = options[Navigation.position].optionDel;
						break;
					case ConsoleKey.Backspace:
						runAction = Navigation.Back;
						break;
					default:
						Navigation.Cycle(KeyInput);
						break;
				}

				// May cause runtime issue
				// Developer must add a way to escape loops
				returnAction = runAction();

				switch (returnAction)
				{
					case ReturnAction.Exit:
						Option.exit();
						break;
					case ReturnAction.Throw:
						Option.ThrowException();
						break;
					case ReturnAction.Break:
						goto Breakout;
				}

				// Final Drawing
				ConsoleRender.ClearFromLastRow();
			}

			Breakout:
			Navigation.Reset();
			PageManager.Revert();       // Revert console style after exit
		}

		public void Start(ChoicePattern pattern = ChoicePattern.Rows)
			=> this.CycleSelection(this.PageDescription, this.options, pattern);

		public void Dispose()
		{
			this.Start(this.renderMode);
			this.PageDescription = null;
			this.Page.Dispose();
			this.Page = null;
			this.options.Clear();
			this.options = null;
		}
	}
}
