using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI.ConsoleUI.Form
{
	public delegate ReturnAction QueryAction<T>(T query);

	public class SearchForm<T> : IDisposable
	{
		public string PageDescription;
		private PageManager Page;
		private List<T> Queries;
		public QueryAction<T> action;

		private SearchForm()
		{
			this.Page = new PageManager($"Page {Navigation.depth}");
			this.Queries = new List<T>();
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

		public void AddQuery(T item)
			=> this.Queries.Add(item);

		public void AddQuery(List<T> items)
			=> this.Queries.AddRange(items);
		
		// INCOMPLETE
		// FUNCTIONALITY OF THIS CLASS IS UNSTABLE
		private void SearchQuery(string Description, List<T> queries, QueryAction<T> queryAction)
		{
			ConsoleKeyInfo KeyInfo;
			ReturnAction retAction = ReturnAction.Stay;
			Navigation.StartNew();
			T SelectedQuery = queries.FirstOrDefault();

			string UserInput = "";
			List<T> FilteredStrings = new List<T>();

			while (true)
			{
				ConsoleRender.ClearAll();

				ConsoleRender.WriteLine(Description + $"\nDepth: {Navigation.depth}");
				ConsoleRender.Write("Query: ");
				ConsoleRender.Write(UserInput + "\n", RenderMode.highlight);

				ConsoleRender.CaptureRowPosition();

				FilteredStrings = queries.FindAll(
					element => element.ToString().ToLower().Contains(UserInput)
					);
				Navigation.UpdateLimit(FilteredStrings.Count);
				
				Console.WriteLine();
				ConsoleRender.RenderOptionsRows(FilteredStrings);

				KeyInfo = PageManager.KeyInput(true);
				if (PageManager.IsPrintableKey(KeyInfo.Key))
					UserInput += KeyInfo.KeyChar.ToString();
				else Navigation.Cycle(KeyInfo.Key);

				switch (KeyInfo.Key)
				{
					case ConsoleKey.Spacebar:
						UserInput += " ";
						break;
					case ConsoleKey.Enter:
						if (FilteredStrings.Any())
							retAction = queryAction(FilteredStrings[Navigation.position]);
						break;
					case ConsoleKey.Backspace:
						if (!UserInput.Any()) retAction = ReturnAction.Break;
						else UserInput = UserInput.Remove(UserInput.Length - 1);
						break;
				}

				switch (retAction)
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

				ConsoleRender.ClearFromLastRow();
			}

			Breakout:
			Navigation.Reset();
			PageManager.Revert();
		}

		public void Start()
			=> this.SearchQuery(this.PageDescription, this.Queries, this.action);

		public void Dispose()
		{
			this.Start();
			this.PageDescription = null;
			this.Page.Dispose();
			this.Queries.Clear();
			this.Queries = null;
			this.action = null;
		}
	}
}
