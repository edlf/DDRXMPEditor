using DDR5XMPEditor.DDR5SPD;
using Stylet;
using System;

namespace DDR5XMPEditor.Pages
{
    public class EXPOEditorViewModel : Screen
    {
        public EXPO EXPOProfile { get; set; }
        public DDR5_SPD SPD { get; set; }
        public bool IsEnabled { get; set; }
        public int ProfileNumber { get; set; }
        public double? Frequency
        {
            get
            {
                if (EXPOProfile == null)
                {
                    return null;
                }

                return Math.Round(1.0 / ((double)EXPOProfile.MinCycleTime / 1000000));
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
        public EXPOEditorViewModel(int profileNumber)
        {
            ProfileNumber = profileNumber;
        }
    }
}
