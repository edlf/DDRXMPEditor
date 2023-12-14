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
        public XMP_3_0 XMPProfile {
            get
            {
                if (SPD != null) {
                    if (ProfileNumber == 1) {
                        return SPD.XMP1;
                    }
                    if (ProfileNumber == 2)
                    {
                        return SPD.XMP2;
                    }
                    if (ProfileNumber == 3)
                    {
                        return SPD.XMP3;
                    }
                    if (ProfileNumber == 4)
                    {
                        return SPD.XMPUser1;
                    }
                    if (ProfileNumber == 5)
                    {
                        return SPD.XMPUser2;
                    }
                }

                return null;
            } 
            
            set {
                if (SPD != null)
                {
                    if (ProfileNumber == 1)
                    {
                        SPD.XMP1 = value;
                    }
                    if (ProfileNumber == 2)
                    {
                        SPD.XMP2 = value;
                    }
                    if (ProfileNumber == 3)
                    {
                        SPD.XMP3 = value;
                    }
                    if (ProfileNumber == 4)
                    {
                        SPD.XMPUser1 = value;
                    }
                    if (ProfileNumber == 5)
                    {
                        SPD.XMPUser2 = value;
                    }
                }
            }
        }
        public DDR5_SPD SPD { get; set; }
        public bool IsEnabled { get; set; }
        public int ProfileNumber { get; set; }
        public ObservableCollection<Tuple<string, XMP_3_0.CommandRatesEnum>> CommandRatesCollection { get; set; }
        public string ProfileName
        {
            get {
                if (XMPProfile == null || SPD == null) {
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
                if ((XMPProfile != null && SPD != null) && !XMPProfile.IsUserProfile())
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
                Refresh();
            }
        }
        public bool IsNameEnabled {
            get { 
                if (!IsEnabled || XMPProfile == null) {
                    return false;
                }

                return !XMPProfile.IsUserProfile();
            }
        }

        public double? Frequency
        {
            get
            {
                if (XMPProfile == null)
                {
                    return null;
                }

                return Math.Round(1.0 / ((double)XMPProfile.MinCycleTime / 1000000));
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
            get => XMPProfile != null && XMPProfile.CommandRate.HasValue ? XMPProfile.CommandRate.Value : XMP_3_0.CommandRatesEnum._undefined;
            set => XMPProfile.CommandRate = value;
        }

        public XMP3EditorViewModel(int profileNumber)
        {
            ProfileNumber = profileNumber;

            CommandRatesCollection = new ObservableCollection<Tuple<string, XMP_3_0.CommandRatesEnum>>
            {
                Tuple.Create("Undefined", XMP_3_0.CommandRatesEnum._undefined),
                Tuple.Create("1N", XMP_3_0.CommandRatesEnum._1n),
                Tuple.Create("2N", XMP_3_0.CommandRatesEnum._2n),
                Tuple.Create("3N", XMP_3_0.CommandRatesEnum._3n)
            };
        }
    }
}
