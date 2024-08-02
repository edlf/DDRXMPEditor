using System;
using Stylet;

namespace DDR5XMPEditor.Pages
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        public MenuBarViewModel MenuBar { get; private set; }
        public DDR5MainViewModel DDR5Editor { get; private set; }
        public ShellViewModel(IEventAggregator aggregator)
        {
            MenuBar = new MenuBarViewModel(aggregator);
            DDR5Editor = new DDR5MainViewModel(aggregator);
        }
    }
}
