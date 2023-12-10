using System;
using Stylet;

namespace DDR4XMPEditor.Pages
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        public MenuBarViewModel MenuBar { get; private set; }
        public DDR4EditorViewModel DDR4Editor { get; private set; }
        public DDR5EditorViewModel DDR5Editor { get; private set; }

        public ShellViewModel(IEventAggregator aggregator)
        {
            MenuBar = new MenuBarViewModel(aggregator);
            // TODO Make this toggleable
            //DDR4Editor = new DDR4EditorViewModel(aggregator);
            DDR5Editor = new DDR5EditorViewModel(aggregator);
        }
    }
}
