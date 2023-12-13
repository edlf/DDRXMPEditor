using DDR4XMPEditor.DDR5SPD;
using Stylet;
using System;
using System.ComponentModel;
using System.Linq;

namespace DDR4XMPEditor.Pages
{
    public class DDR5SPDEditorViewModel : Screen
    {
        public DDR5_SPD JedecProfile { get; set; }

        public double? Frequency
        {
            get
            {
                if (JedecProfile == null)
                {
                    return null;
                }

                return Math.Round(1.0 / ((double)JedecProfile.MinCycleTime / 1000000));
            }
        }

        public double? MegaTransfers
        {
            get
            {
                if (Frequency == null)
                {
                    return null;
                }

                return Frequency * 2;
            }
        }

        public DDR5SPDEditorViewModel()
        {
            CLSupported = new BindingList<bool>(Enumerable.Range(20, 99).Select(n => false).ToList());
            CLSupported.ListChanged += (s, e) =>
            {
                var clSupported = JedecProfile.GetClSupported();
                Utilities.SetCLSupportedDDR5(clSupported, e.NewIndex, CLSupported[e.NewIndex]);
                for (int i = 0; i < clSupported.Length; ++i)
                {
                    JedecProfile.SetClSupported(i, clSupported[i]);
                }
                Refresh();
            };
        }

        public BindingList<bool> CLSupported { get; private set; }
    }
}
