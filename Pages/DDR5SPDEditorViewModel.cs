using DDR4XMPEditor.DDR5SPD;
using Stylet;
using System;
using System.ComponentModel;
using System.Linq;

namespace DDR4XMPEditor.Pages
{
    public class DDR5SPDEditorViewModel : Screen
    {
        public DDR5_SPD Profile { get; set; }

        public double? Frequency
        {
            get
            {
                if (Profile == null)
                {
                    return null;
                }

                return Math.Round(1.0 / ((double)Profile.MinCycleTime / 1000000));
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
                var clSupported = Profile.GetClSupported();
                DDR5_SPD.SetCLSupported(clSupported, e.NewIndex, CLSupported[e.NewIndex]);
                for (int i = 0; i < clSupported.Length; ++i)
                {
                    Profile.SetClSupported(i, clSupported[i]);
                }
                Refresh();
            };
        }

        public BindingList<bool> CLSupported { get; private set; }

        /// <summary>
        /// Convert <paramref name="timeps"/> to DRAM ticks.
        /// </summary>
        /// <param name="timeps">Time in picoseconds.</param>
        /// <returns></returns>
        private int? TimeToTicks(int? timeps)
        {
            if (!timeps.HasValue || timeps.Value <= 0)
            {
                return null;
            }
            return timeps.Value;
        }

        /// <summary>
        /// Convert <paramref name="dramTicks"/> to MTB ticks.
        /// </summary>
        /// <param name="dramTicks">Ticks using DRAM cycle time units.</param>
        /// <returns>Ticks using MTB units.</returns>
        private int? DRAMTicksToMTBTicks(int? dramTicks)
        {
            if (!dramTicks.HasValue)
            {
                return null;
            }

            int sdramCycleTime = Profile.MinCycleTime;
            return dramTicks.Value * sdramCycleTime;
        }
    }
}
