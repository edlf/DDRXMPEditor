using DDR4XMPEditor.DDR5SPD;
using Microsoft.Win32;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DDR4XMPEditor.Pages
{
    public class DDR5MiscViewModel : Screen
    {
        public bool IsEnabled { get; set; }

        public ushort sourceProfile { get; set; }
        public ushort targetProfile { get; set; }
        public ushort importExportProfile { get; set; }
        public DDR5_SPD SPD { get; set; }

        public ObservableCollection<Tuple<string, DDR5_SPD.FormFactorEnum>> FormFactorCollection { get; set; }
        public ObservableCollection<Tuple<string, DDR5_SPD.Densities>> DensityCollection { get; set; }

        public ObservableCollection<int> BankGroupsCollection { get; set; }
        public ObservableCollection<int> BanksPerBankGroupCollection { get; set; }
        public ObservableCollection<int> ColumnAddressesCollection { get; set; }
        public ObservableCollection<int> RowAddressesCollection { get; set; }
        public ObservableCollection<int> DeviceWidthsCollection { get; set; }
        public ObservableCollection<ushort> XMPProfileNoCollection { get; set; }

        public DDR5_SPD.FormFactorEnum SelectedFormFactor
        {
            get => SPD != null && SPD.FormFactor.HasValue ? SPD.FormFactor.Value : DDR5_SPD.FormFactorEnum.Reserved;
            set => SPD.FormFactor = value;
        }

        // Density
        public DDR5_SPD.Densities SelectedDensity
        {
            get => SPD != null && SPD.Density.HasValue ? SPD.Density.Value : DDR5_SPD.Densities._0Gb;
            set => SPD.Density = value;
        }
        public int SelectedBankGroups
        {
            get => SPD == null ? BankGroupsCollection.First() : SPD.BankGroups;
            set => SPD.BankGroups = (ushort)value;
        }
        public int SelectedBanksPerBankGroup
        {
            get => SPD != null ? SPD.BanksPerBankGroup : BanksPerBankGroupCollection.First();
            set => SPD.BanksPerBankGroup = (ushort)value;
        }
        public int SelectedColumnAddress
        {
            get => SPD == null ? ColumnAddressesCollection.First() : SPD.ColumnAddresses;
            set => SPD.ColumnAddresses = (ushort)value;
        }
        public int SelectedRowAddress
        {
            get => SPD == null ? RowAddressesCollection.First() : SPD.RowAddresses;
            set => SPD.RowAddresses = (ushort)value;
        }

        // Module organization
        public int SelectedDeviceWidth
        {
            get => SPD == null ? DeviceWidthsCollection.First() : SPD.DeviceWidth;
            set => SPD.DeviceWidth = (ushort)value;
        }

        public void copyXMPProfile()
        {
            if (SPD.copyXmpProfile(sourceProfile, targetProfile))
            {
                MessageBox.Show("Data was sucessfully copied.", "XMP data copied", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Something went wrong while copying the XMP data.", "Failed to copy XMP data", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        public void exportXMPProfile() {
            SPD.UpdateCrc();

            byte[] bytes;
            string profileName;

            switch (importExportProfile)
            {
                case 1:
                    bytes = SPD.XMP1.GetBytes();
                    profileName = SPD.XMPProfile1Name;
                    break;
                case 2:
                    bytes = SPD.XMP2.GetBytes();
                    profileName = SPD.XMPProfile2Name;
                    break;
                case 3:
                    bytes = SPD.XMP3.GetBytes();
                    profileName = SPD.XMPProfile3Name;
                    break;
                case 4:
                    bytes = SPD.XMPUser1.GetBytes();
                    profileName = "User 1";
                    break;
                case 5:
                    bytes = SPD.XMPUser2.GetBytes();
                    profileName = "User 2";
                    break;
                default:
                    return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XMP 3.0 file (*.bin)|*.bin";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, bytes);
                System.Windows.MessageBox.Show(
                    $"Successfully saved XMP 3.0 Profile [{profileName}]  to {saveFileDialog.FileName}",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        public void importXMPProfile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                long length = new FileInfo(openFileDialog.FileName).Length;

                // Check file size before loading contents
                if (length != 64)
                {
                    MessageBox.Show("Invalid XMP 3.0 file, file size must be 64 bytes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var bytes = File.ReadAllBytes(openFileDialog.FileName);

                XMP_3_0 inputProfile = XMP_3_0.Parse(importExportProfile, bytes);

                if (inputProfile.CheckCRCValidity())
                {
                    switch (importExportProfile)
                    {
                        case 1:
                            SPD.XMP1 = inputProfile;
                            SPD.XMPProfile1Name = "Imported";
                            SPD.XMP1Enabled = true;
                            break;
                        case 2:
                            SPD.XMP2 = inputProfile;
                            SPD.XMPProfile2Name = "Imported";
                            SPD.XMP2Enabled = true;
                            break;
                        case 3:
                            SPD.XMP3 = inputProfile;
                            SPD.XMPProfile3Name = "Imported";
                            SPD.XMP3Enabled = true;
                            break;
                        case 4:
                            SPD.XMPUser1 = inputProfile;
                            break;
                        case 5:
                            SPD.XMPUser2 = inputProfile;
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid XMP 3.0 Profile checksum", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private ICommand _copyCommand;
        private ICommand _exportXMPCommand;
        private ICommand _importXMPCommand;

        public class RelayCommand : ICommand
        {
            readonly Action<object> _execute;
            readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                if (execute == null)
                {
                    throw new ArgumentNullException("execute");
                }

                _execute = execute;
                _canExecute = canExecute;
            }
            public bool CanExecute(object parameters)
            {
                return _canExecute == null ? true : _canExecute(parameters);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameters)
            {
                _execute(parameters);
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new RelayCommand(
                        param => this.copyXMPProfile(),
                        param => this.CanCopy()
                    );
                }
                return _copyCommand;
            }
        }

        public ICommand ExportXMPCommand
        {
            get
            {
                if (_exportXMPCommand == null)
                {
                    _exportXMPCommand = new RelayCommand(
                        param => this.exportXMPProfile(),
                        param => this.CanExport()
                    );
                }
                return _exportXMPCommand;
            }
        }

        public ICommand ImportXMPCommand
        {
            get
            {
                if (_importXMPCommand == null)
                {
                    _importXMPCommand = new RelayCommand(
                        param => this.importXMPProfile(),
                        param => this.CanImport()
                    );
                }
                return _importXMPCommand;
            }
        }

        private bool CanCopy()
        {
            return ((sourceProfile >= 1 && sourceProfile <= 5) && (targetProfile >= 1 && targetProfile <= 5) && (sourceProfile != targetProfile));
        }

        private bool CanImport()
        {
            if (!(importExportProfile >= 1 && importExportProfile <= 5)){
                return false;
            }


            return true;
        }
        private bool CanExport()
        {
            if (!(importExportProfile >= 1 && importExportProfile <= 5))
            {
                return false;
            }

            switch (importExportProfile)
            {
                case 1:
                    return SPD.XMP1.CheckCRCValidity() && !SPD.XMP1.IsEmpty();
                case 2:
                    return SPD.XMP2.CheckCRCValidity() && !SPD.XMP2.IsEmpty();
                case 3:
                    return SPD.XMP3.CheckCRCValidity() && !SPD.XMP3.IsEmpty();
                case 4:
                    return SPD.XMPUser1.CheckCRCValidity() && !SPD.XMPUser1.IsEmpty();
                case 5:
                    return SPD.XMPUser2.CheckCRCValidity() && !SPD.XMPUser1.IsEmpty();
                default:
                    return false;
            }
        }
        public DDR5MiscViewModel()
        {
            sourceProfile = 1;
            targetProfile = 2;

            FormFactorCollection = new ObservableCollection<Tuple<string, DDR5_SPD.FormFactorEnum>>
            {
                Tuple.Create("Reserved", DDR5_SPD.FormFactorEnum.Reserved),
                Tuple.Create("RDIMM", DDR5_SPD.FormFactorEnum.RDIMM),
                Tuple.Create("UDIMM", DDR5_SPD.FormFactorEnum.UDIMM),
                Tuple.Create("SODIMM", DDR5_SPD.FormFactorEnum.SODIMM),
                Tuple.Create("LRDIMM", DDR5_SPD.FormFactorEnum.LRDIMM),
                Tuple.Create("CUDIMM", DDR5_SPD.FormFactorEnum.CUDIMM),
                Tuple.Create("CSODIMM", DDR5_SPD.FormFactorEnum.CSODIMM),
                Tuple.Create("MRDIMM", DDR5_SPD.FormFactorEnum.MRDIMM),
                Tuple.Create("CAMM2", DDR5_SPD.FormFactorEnum.CAMM2),
                Tuple.Create("Reserved", DDR5_SPD.FormFactorEnum.Reserved_9),
                Tuple.Create("DDIMM", DDR5_SPD.FormFactorEnum.DDIMM),
                Tuple.Create("Solder_down", DDR5_SPD.FormFactorEnum.Solder_down),
                Tuple.Create("Reserved", DDR5_SPD.FormFactorEnum.Reserved_12),
                Tuple.Create("Reserved", DDR5_SPD.FormFactorEnum.Reserved_13),
                Tuple.Create("Reserved", DDR5_SPD.FormFactorEnum.Reserved_14),
                Tuple.Create("Reserved", DDR5_SPD.FormFactorEnum.Reserved_15)
            };

            DensityCollection = new ObservableCollection<Tuple<string, DDR5_SPD.Densities>>
            {
                Tuple.Create("No memory", DDR5_SPD.Densities._0Gb),
                Tuple.Create("4Gb",   DDR5_SPD.Densities._4Gb),
                Tuple.Create("8Gb",   DDR5_SPD.Densities._8Gb),
                Tuple.Create("12Gb",  DDR5_SPD.Densities._12Gb),
                Tuple.Create("16Gb",  DDR5_SPD.Densities._16Gb),
                Tuple.Create("24Gb",  DDR5_SPD.Densities._24Gb),
                Tuple.Create("32Gb",  DDR5_SPD.Densities._32Gb),
                Tuple.Create("48Gb",  DDR5_SPD.Densities._48Gb),
                Tuple.Create("64Gb",  DDR5_SPD.Densities._64Gb)
            };

            BankGroupsCollection = new ObservableCollection<int> { 1, 2, 4, 8 };
            BanksPerBankGroupCollection = new ObservableCollection<int> { 1, 2, 4 };

            ColumnAddressesCollection = new ObservableCollection<int> { 10, 11 };
            RowAddressesCollection = new ObservableCollection<int> { 16, 17, 18 };

            DeviceWidthsCollection = new ObservableCollection<int> { 4, 8, 16, 32 };
            XMPProfileNoCollection = new ObservableCollection<ushort> { 1, 2, 3, 4, 5 };
        }
    }
}
