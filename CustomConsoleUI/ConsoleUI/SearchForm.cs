using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI.ConsoleUI
{
	public delegate void EntityRenderDelegate<T>(List<T> lines);

	internal class SearchForm
	{
		public string PageDescription;
		private PageManager Page;
		private List<string> Queries;

		private SearchForm()
		{
			this.Page = new PageManager($"Page {Navigation.depth}");
			this.Queries = new List<string>();
		}

		public SearchForm(string Title, string Description) : this()
		{
			this.Page = new PageManager(Title);
			this.PageDescription = Description;
		}

		public SearchForm(string Description) : this()
		{
			this.Page = new PageManager($"Page {Navigation.depth}");
			this.PageDescription = Description;
		}

		public void AddQuery(string item)
			=> this.Queries.Add(item);

		public void AddQuery(List<string> items)
			=> this.Queries.AddRange(items);
		
		// INCOMPLETE
		// FUNCTIONALITY OF THIS CLASS IS UNSTABLE
		private void SearchQuery(string Description, List<string> strings)
		{
			ConsoleKey InputKey;
			Navigation.StartNew();
			string SelectedQuery = strings.First();

			string UserInputConcat = "";
			List<string> FilteredStrings = new List<string>();

			while (true)
			{
				ConsoleRender.ClearInterface();
				ConsoleRender.SetCursorPosition(0, 0);

				ConsoleRender.WriteLine(Description + $"\nDepth: {Navigation.depth}");
				ConsoleRender.Write("Query: ");
				ConsoleRender.Write(UserInputConcat + "\n", RenderMode.highlight);

				ConsoleRender.CaptureRowPosition();

				FilteredStrings = strings.FindAll(element => element.ToLower().Contains(UserInputConcat));
				Navigation.UpdateLimit(FilteredStrings.Count);
				Console.WriteLine(Navigation.position);
				Console.WriteLine(SelectedQuery);

				Console.WriteLine();
				ConsoleRender.RenderOptionsRows(FilteredStrings);

				InputKey = PageManager.KeyInput(true);
				if (InputKey.ToString().Length == 1) UserInputConcat += InputKey;
				else if (InputKey == ConsoleKey.Spacebar) UserInputConcat += " ";
				else if (InputKey == ConsoleKey.Enter) SelectedQuery = FilteredStrings[Navigation.position];
				else if ((InputKey == ConsoleKey.Backspace) && UserInputConcat.Any())
					UserInputConcat = UserInputConcat.Remove(UserInputConcat.Length - 1);
				else Navigation.Cycle(InputKey);
				UserInputConcat = UserInputConcat.ToLower();

				ConsoleRender.ClearFromLastRow();
			}
		}

		public void Start()
			=> this.SearchQuery(this.PageDescription, this.Queries);
	}
}
