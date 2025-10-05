using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI
{
    internal delegate void RenderStyle();

    internal class Page : IDisposable
    {
        public string PageDescription;
        public ConsoleUI PageConsole;
        public List<Option> options;

        private Page()
        {
            this.PageConsole = new ConsoleUI("Page " + $"{Navigation.depth}");
            this.options = new List<Option>()
            {
                new Option()
            };
        }

        public Page(string Title, string Description)
        {
            this.PageConsole = new ConsoleUI(Title);
            this.PageDescription = Description;
        }

        public Page(string Description) : this()
        {
            this.PageConsole = new ConsoleUI();
            this.PageDescription = Description;
        }

        public void RenderOptions()
            => ConsoleUI.CycleSelection(this.PageDescription, this.options);

        public void RenderOptions(RenderPattern pattern)
            => ConsoleUI.CycleSelection(this.PageDescription, this.options, pattern);
            

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
