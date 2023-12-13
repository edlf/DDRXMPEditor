using DDR4XMPEditor.DDR5SPD;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace DDR4XMPEditor.Pages
{
    public class DDR5MiscViewModel : Screen
    {
        public bool IsEnabled { get; set; }
        public DDR5_SPD SPD { get; set; }

        public ObservableCollection<Tuple<string, DDR5_SPD.FormFactorEnum>> FormFactorCollection { get; set; }
        public ObservableCollection<Tuple<string, DDR5_SPD.Densities>> DensityCollection { get; set; }

        public ObservableCollection<int> BankGroupsCollection { get; set; }
        public ObservableCollection<int> BanksPerBankGroupCollection { get; set; }
        public ObservableCollection<int> ColumnAddressesCollection { get; set; }
        public ObservableCollection<int> RowAddressesCollection { get; set; }
        public ObservableCollection<int> DeviceWidthsCollection { get; set; }

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

        public DDR5MiscViewModel()
        {
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
        }
    }
}
