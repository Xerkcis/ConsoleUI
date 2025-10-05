using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleUI
{
    internal delegate void RenderDelegate<T>(List<T> options);

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

        public Page(string Title, string Description) : this()
        {
            this.PageConsole = new ConsoleUI(Title);
            this.PageDescription = Description;
        }

        public Page(string Description) : this()
        {
            this.PageConsole = new ConsoleUI();
            this.PageDescription = Description;
        }

        private void CycleSelection(string description, List<Option> options, RenderPattern style = RenderPattern.Rows)
        {
            // Initialization
            ConsoleKey inputKey;
            OptionDelegate runAction;
            RenderDelegate<Option> renderManager
                = delegate { throw new Exception("Cycle terminated at startup"); };
            ReturnAction returnAction;

            // Resetting position to zero
            Navigation.StartNew();

            switch(style)
            {
                case RenderPattern.Rows:
                    renderManager = ConsoleRender.RenderOptionsRows;
                    break;
                case RenderPattern.Columns:
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
                inputKey = ConsoleUI.KeyInput();

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

            ConsoleUI.Revert();       // Revert console style after exit
        }

        public void StartPage()
                    => this.CycleSelection(this.PageDescription, this.options);

        public void StartPage(RenderPattern pattern)
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
