using DDR4XMPEditor.DDR4SPD;
using DDR4XMPEditor.DDR5SPD;
using DDR4XMPEditor.Events;
using Stylet;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace DDR4XMPEditor.Pages
{
    public class DDR4EditorViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<SelectedSPDFileEvent>, 
        IHandle<SaveSPDFileEvent>
    {
        public DDR4_SPD DDR4_SPD { get; set; }
        public DDR5_SPD DDR5_SPD { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool IsDDR4SPDValid => DDR4_SPD != null;
        public bool IsDDR5SPDValid => DDR5_SPD != null;

        private readonly DDR4SPDEditorViewModel ddr4spdVM;
        private readonly XMP2EditorViewModel xmpVm1, xmpVm2;
        private readonly DDR4MiscViewModel miscVm = new DDR4MiscViewModel { DisplayName = "Misc" };

        public DDR4EditorViewModel(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            Items.Add(ddr4spdVM = new DDR4SPDEditorViewModel { DisplayName = "SPD" });
            Items.Add(xmpVm1 = new XMP2EditorViewModel { DisplayName = "XMP 1" });
            Items.Add(xmpVm2 = new XMP2EditorViewModel { DisplayName = "XMP 2" });
            Items.Add(miscVm);
            ActiveItem = Items[0];
        }

        public void Handle(SelectedSPDFileEvent e)
        {
            long length = new System.IO.FileInfo(e.FilePath).Length;

            // Check file size before loading contents
            if (length != 512) {
                System.Windows.MessageBox.Show("Invalid SPD file, file size must be 512", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] spd_bytes = File.ReadAllBytes(e.FilePath);

            // DDR4 SPD
            if (length == DDR4_SPD.TotalSize)
            {
                var spd = DDR4_SPD.Parse(spd_bytes);
                if (spd == null)
                {
                    System.Windows.MessageBox.Show("Error parsing DDR4 SPD file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    DDR4_SPD = spd;

                    ddr4spdVM.Profile = DDR4_SPD;
                    for (int i = 0; i < ddr4spdVM.CLSupported.Count; ++i)
                    {
                        ddr4spdVM.CLSupported[i] = DDR4_SPD.IsCLSupported(ddr4spdVM.Profile.GetClSupported(), i);
                    }
                    BindNotifyPropertyChanged(ddr4spdVM);

                    xmpVm1.IsEnabled = DDR4_SPD.XMP1Enabled;
                    DDR4_SPD.Bind(x => x.XMP1Enabled, (s, args) => xmpVm1.IsEnabled = args.NewValue);
                    xmpVm1.Profile = DDR4_SPD.XMP1;
                    for (int i = 0; i < xmpVm1.CLSupported.Count; ++i)
                    {
                        xmpVm1.CLSupported[i] = DDR4_SPD.IsCLSupported(xmpVm1.Profile.GetClSupported(), i);
                    }

                    xmpVm2.IsEnabled = DDR4_SPD.XMP2Enabled;
                    DDR4_SPD.Bind(x => x.XMP2Enabled, (s, args) => xmpVm2.IsEnabled = args.NewValue);
                    xmpVm2.Profile = DDR4_SPD.XMP2;
                    for (int i = 0; i < xmpVm2.CLSupported.Count; ++i)
                    {
                        xmpVm2.CLSupported[i] = DDR4_SPD.IsCLSupported(xmpVm2.Profile.GetClSupported(), i);
                    }

                    BindNotifyPropertyChanged(xmpVm1);
                    BindNotifyPropertyChanged(xmpVm2);

                    FilePath = e.FilePath;
                    FileName = System.IO.Path.GetFileName(FilePath);

                    miscVm.IsEnabled = true;
                    miscVm.SPD = spd;
                    if (spd.Density.HasValue)
                    {
                        miscVm.SelectedDensity = spd.Density.Value;
                    }
                    miscVm.SelectedBank = spd.Banks;
                    miscVm.SelectedBankGroups = spd.BankGroups;
                    miscVm.SelectedColumnAddress = spd.ColumnAddresses;
                    miscVm.SelectedRowAddress = spd.RowAddresses;
                }
            }
            else if (length == DDR5_SPD.TotalSize) // DDR5 SPD
            {
                System.Windows.MessageBox.Show("DDR4 Mode used with DDR5 SPD", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }          
        }

        public void Handle(SaveSPDFileEvent e)
        {
            DDR4_SPD.UpdateCrc();
            var bytes = DDR4_SPD.GetBytes();
            File.WriteAllBytes(e.FilePath, bytes);
            System.Windows.MessageBox.Show(
                $"Successfully SPD saved to {e.FilePath}",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BindNotifyPropertyChanged(XMP2EditorViewModel vm)
        {
            void UpdateFrequency()
            {
                int? timeps = vm.Profile?.SDRAMCycleTicks * DDR4_SPD.MTBps + vm.Profile?.SDRAMCycleTimeFC;
                vm.SDRAMCycleTime = timeps / 1000.0;
            }
            UpdateFrequency();
            vm.Profile.Bind(x => x.SDRAMCycleTicks, (s, e) => UpdateFrequency());
            vm.Profile.Bind(x => x.SDRAMCycleTimeFC, (s, e) => UpdateFrequency());

            // Bind timings.
            vm.Profile.Bind(x => x.CLTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.CLFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RCDTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RCDFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RPTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RPFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RASTicks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RCTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RCFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RFC1Ticks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RFC2Ticks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RFC4Ticks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RRDSTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RRDSFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RRDLTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RRDLFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.FAWTicks, (s, e) => vm.Refresh());
        }

        private void BindNotifyPropertyChanged(DDR4SPDEditorViewModel vm)
        {
            void UpdateFrequency()
            {
                int? timeps = vm.Profile?.MinCycleTime * DDR4_SPD.MTBps + vm.Profile?.MinCycleTimeFC;
                vm.SDRAMCycleTime = timeps / 1000.0;
            }
            UpdateFrequency();
            vm.Profile.Bind(x => x.MinCycleTime, (s, e) => UpdateFrequency());
            vm.Profile.Bind(x => x.MinCycleTimeFC, (s, e) => UpdateFrequency());

            // Bind timings.
            vm.Profile.Bind(x => x.CLTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.CLFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RCDTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RCDFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RPTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RPFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RASTicks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RCTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RCFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RFC1Ticks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RFC2Ticks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RFC4Ticks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RRDSTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RRDSFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.RRDLTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.RRDLFC, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.FAWTicks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.WRTicks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.WTRSTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.WTRLTicks, (s, e) => vm.Refresh());

            vm.Profile.Bind(x => x.CCDLTicks, (s, e) => vm.Refresh());
            vm.Profile.Bind(x => x.CCDLFC, (s, e) => vm.Refresh());
        }
    }
}
