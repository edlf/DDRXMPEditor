﻿using DDR4XMPEditor.DDR4SPD;
using Stylet;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DDR4XMPEditor.Pages
{
    public class XMP2EditorViewModel : Screen
    {
        public XMP_2_0 Profile { get; set; }
        public bool IsEnabled { get; set; }

        public double? SDRAMCycleTime { get; set; }
        public double? Frequency
        {
            get
            {
                if (SDRAMCycleTime.HasValue && SDRAMCycleTime.Value > 0)
                {
                    return 1000 / SDRAMCycleTime.Value;
                }

                return null;
            }
        }

        public XMP2EditorViewModel()
        {
            CLSupported = new BindingList<bool>(Enumerable.Range(0, 31).Select(n => false).ToList());
            CLSupported.ListChanged += (s, e) =>
            {
                var clSupported = Profile.GetClSupported();
                DDR4_SPD.SetCLSupported(clSupported, e.NewIndex, CLSupported[e.NewIndex]);
                for (int i = 0; i < clSupported.Length; ++i)
                {
                    Profile.SetClSupported(i, clSupported[i]);
                }
                Refresh();
            };
        }

        public BindingList<bool> CLSupported { get; private set; }

        public int? tCL
        {
            get => TimeToTicks(Profile?.CLTicks * DDR4_SPD.MTBps + Profile?.CLFC);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.CLTicks = (byte)ticks.Value;
                }
            }
        }
        public double? tCLTime => (Profile?.CLTicks * DDR4_SPD.MTBps + Profile?.CLFC) / 1000.0;
        public int? tRCD
        {
            get => TimeToTicks(Profile?.RCDTicks * DDR4_SPD.MTBps + Profile?.RCDFC);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RCDTicks = (byte)ticks.Value;
                }
            }
        }
        public double? tRCDTime => (Profile?.RCDTicks * DDR4_SPD.MTBps + Profile?.RCDFC) / 1000.0;
        public int? tRP
        {
            get => TimeToTicks(Profile?.RPTicks * DDR4_SPD.MTBps + Profile?.RPFC);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RPTicks = (byte)ticks.Value;
                }
            }
        }
        public double? tRPTime => (Profile?.RPTicks * DDR4_SPD.MTBps + Profile?.RPFC) / 1000.0;
        public int? tRAS
        {
            get => TimeToTicks(Profile?.RASTicks * DDR4_SPD.MTBps);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RASTicks = (byte)ticks.Value;
                }
            }
        }
        public int? tRC
        {
            get => TimeToTicks(Profile?.RCTicks * DDR4_SPD.MTBps + Profile?.RCFC);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RCTicks = ticks.Value;
                }
            }
        }
        public double? tRCTime => (Profile?.RCTicks * DDR4_SPD.MTBps + Profile?.RCFC) / 1000.0;
        public int? tRFC
        {
            get => TimeToTicks(Profile?.RFC1Ticks * DDR4_SPD.MTBps);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RFC1Ticks = (ushort)ticks.Value;
                }
            }
        }
        public int? tRFC2
        {
            get => TimeToTicks(Profile?.RFC2Ticks * DDR4_SPD.MTBps);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RFC2Ticks = (ushort)ticks.Value;
                }
            }
        }
        public int? tRFC4
        {
            get => TimeToTicks(Profile?.RFC4Ticks * DDR4_SPD.MTBps);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RFC4Ticks = (ushort)ticks.Value;
                }
            }
        }
        public int? tRRDS
        {
            get => TimeToTicks(Profile?.RRDSTicks * DDR4_SPD.MTBps + Profile?.RRDSFC);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RRDSTicks = (byte)ticks.Value;
                }
            }
        }
        public double? tRRDSTime => (Profile?.RRDSTicks * DDR4_SPD.MTBps + Profile?.RRDSFC) / 1000.0;
        public int? tRRDL
        {
            get => TimeToTicks(Profile?.RRDLTicks * DDR4_SPD.MTBps + Profile?.RRDLFC);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.RRDLTicks = (byte)ticks.Value;
                }
            }
        }
        public double? tRRDLTime => (Profile?.RRDLTicks * DDR4_SPD.MTBps + Profile?.RRDLFC) / 1000.0;
        public int? tFAW
        {
            get => TimeToTicks(Profile?.FAWTicks * DDR4_SPD.MTBps);
            set
            {
                if (Profile == null)
                {
                    return;
                }

                int? ticks = DRAMTicksToMTBTicks(value);
                if (ticks.HasValue)
                {
                    Profile.FAWTicks = (byte)ticks.Value;
                }
            }
        }

        /// <summary>
        /// Convert <paramref name="timeps"/> to DRAM ticks.
        /// </summary>
        /// <param name="timeps">Time in picoseconds.</param>
        /// <returns></returns>
        private int? TimeToTicks(int? timeps)
        {
            if (!timeps.HasValue || ! SDRAMCycleTime.HasValue || timeps.Value <= 0)
            {
                return null;
            }
            return (int)Math.Ceiling(timeps.Value/1000.0 / SDRAMCycleTime.Value);
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

            int sdramCycleTime = Profile.SDRAMCycleTicks * DDR4_SPD.MTBps + Profile.SDRAMCycleTimeFC;
            return (int)Math.Floor(1.0 * dramTicks.Value * sdramCycleTime / DDR4_SPD.MTBps);
        }
    }

    /// <summary>
    /// Convert ticks to time (ns).
    /// </summary>
    public class TicksToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) * DDR4_SPD.MTBps / 1000.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)Math.Round(((double)value * 1000.0 / DDR4_SPD.MTBps));
        }
    }
}
