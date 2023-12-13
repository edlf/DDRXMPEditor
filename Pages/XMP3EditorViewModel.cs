using DDR4XMPEditor.DDR5SPD;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DDR4XMPEditor.Pages
{
    public class XMP3EditorViewModel : Screen
    {
        public XMP_3_0 Profile { get; set; }
        public DDR5_SPD SPD { get; set; }
        public bool IsEnabled { get; set; }
        public int ProfileNumber { get; set; }
        public ObservableCollection<Tuple<string, XMP_3_0.CommandRatesEnum>> CommandRatesCollection { get; set; }
        public string ProfileName
        {
            get {
                if (Profile == null || SPD == null) {
                    return "";
                }

                if (ProfileNumber == 1)
                {
                    return SPD.XMPProfile1Name;
                }
                else if (ProfileNumber == 2)
                {
                    return SPD.XMPProfile2Name;
                }
                else if (ProfileNumber == 3)
                {
                    return SPD.XMPProfile3Name;
                }
                else if (ProfileNumber == 4) {
                    return "User profile 1";
                }
                else if (ProfileNumber == 5)
                {
                    return "User profile 2";
                }

                return "";
            }
            set {
                if ((Profile != null && SPD != null) && !Profile.IsUserProfile())
                {
                    if (ProfileNumber == 1)
                    {
                        SPD.XMPProfile1Name = value;
                    }
                    else if (ProfileNumber == 2)
                    {
                        SPD.XMPProfile2Name = value;
                    }
                    else if (ProfileNumber == 3)
                    {
                        SPD.XMPProfile3Name = value;
                    }
                }
            }
        }
        public bool IsNameEnabled {
            get { 
                if (!IsEnabled || Profile == null) {
                    return false;
                }

                return !Profile.IsUserProfile();
            }
        }

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
        public XMP_3_0.CommandRatesEnum SelectedCommandRate
        {
            get => Profile != null && Profile.CommandRate.HasValue ? Profile.CommandRate.Value : XMP_3_0.CommandRatesEnum._undefined;
            set => Profile.CommandRate = value;
        }

        public XMP3EditorViewModel(int profileNumber)
        {
            ProfileNumber = profileNumber;

            CLSupported = new BindingList<bool>(Enumerable.Range(20, 99).Select(n => false).ToList());
            CLSupported.ListChanged += (s, e) =>
            {
                var clSupported = Profile.GetClSupported();
                Utilities.SetCLSupportedDDR5(clSupported, e.NewIndex, CLSupported[e.NewIndex]);
                for (int i = 0; i < clSupported.Length; ++i)
                {
                    Profile.SetClSupported(i, clSupported[i]);
                }
                Refresh();
            };

            CommandRatesCollection = new ObservableCollection<Tuple<string, XMP_3_0.CommandRatesEnum>>
            {
                Tuple.Create("Undefined", XMP_3_0.CommandRatesEnum._undefined),
                Tuple.Create("1N", XMP_3_0.CommandRatesEnum._1n),
                Tuple.Create("2N", XMP_3_0.CommandRatesEnum._2n),
                Tuple.Create("3N", XMP_3_0.CommandRatesEnum._3n)
            };
        }

        public BindingList<bool> CLSupported { get; private set; }
    }
}
