using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GTASaveData.GTA3;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Tabs
{

    public class GangsTabVM : TabPageVM
    {
        // TODO: zone picker

        private Gang m_selectedGang;
        private int m_selectedGangIndex;
        private bool m_isHostilePlayer;
        private bool m_isHostileMafia;
        private bool m_isHostileTriads;
        private bool m_isHostileDiablos;
        private bool m_isHostileYakuza;
        private bool m_isHostileYardies;
        private bool m_isHostileCartel;
        private bool m_isHostileHoods;
        private bool m_isHostileGang8;
        private bool m_isHostileGang9;
        private bool m_isHostileCivilians;
        private bool m_isHostileCops;
        private bool m_isHostileEmergency;
        private bool m_isHostileFirefighters;
        private bool m_isHostileProstitutes;
        private bool m_isHostileCriminals;
        private bool m_isHostileSpecial;
        private ObservableCollection<Zone> m_zones;
        private Zone m_selectedZone;
        private short m_pedDensityDay;
        private short m_pedDensityNight;
        private short m_carDensityDay;
        private short m_carDensityNight;

        public Gang Gang
        {
            get { return m_selectedGang; }
            set { m_selectedGang = value; OnPropertyChanged(); }
        }

        public int GangIndex
        {
            get { return m_selectedGangIndex; }
            set { m_selectedGangIndex = value; OnPropertyChanged(); }
        }

        private PedTypeId GangPedType => GangPedTypeIds[(GangType) GangIndex];

        public bool IsHostilePlayer
        {
            get { return m_isHostilePlayer; }
            set { m_isHostilePlayer = value; OnPropertyChanged(); }
        }

        public bool IsHostileMafia
        {
            get { return m_isHostileMafia; }
            set { m_isHostileMafia = value; OnPropertyChanged(); }
        }

        public bool IsHostileTriads
        {
            get { return m_isHostileTriads; }
            set { m_isHostileTriads = value; OnPropertyChanged(); }
        }

        public bool IsHostileDiablos
        {
            get { return m_isHostileDiablos; }
            set { m_isHostileDiablos = value; OnPropertyChanged(); }
        }

        public bool IsHostileYakuza
        {
            get { return m_isHostileYakuza; }
            set { m_isHostileYakuza = value; OnPropertyChanged(); }
        }

        public bool IsHostileYardies
        {
            get { return m_isHostileYardies; }
            set { m_isHostileYardies = value; OnPropertyChanged(); }
        }

        public bool IsHostileCartel
        {
            get { return m_isHostileCartel; }
            set { m_isHostileCartel = value; OnPropertyChanged(); }
        }

        public bool IsHostileHoods
        {
            get { return m_isHostileHoods; }
            set { m_isHostileHoods = value; OnPropertyChanged(); }
        }

        public bool IsHostileGang8
        {
            get { return m_isHostileGang8; }
            set { m_isHostileGang8 = value; OnPropertyChanged(); }
        }

        public bool IsHostileGang9
        {
            get { return m_isHostileGang9; }
            set { m_isHostileGang9 = value; OnPropertyChanged(); }
        }

        public bool IsHostileCivilians
        {
            get { return m_isHostileCivilians; }
            set { m_isHostileCivilians = value; OnPropertyChanged(); }
        }

        public bool IsHostileCops
        {
            get { return m_isHostileCops; }
            set { m_isHostileCops = value; OnPropertyChanged(); }
        }

        public bool IsHostileEmergency
        {
            get { return m_isHostileEmergency; }
            set { m_isHostileEmergency = value; OnPropertyChanged(); }
        }

        public bool IsHostileFirefighters
        {
            get { return m_isHostileFirefighters; }
            set { m_isHostileFirefighters = value; OnPropertyChanged(); }
        }

        public bool IsHostileProstitutes
        {
            get { return m_isHostileProstitutes; }
            set { m_isHostileProstitutes = value; OnPropertyChanged(); }
        }

        public bool IsHostileCriminals
        {
            get { return m_isHostileCriminals; }
            set { m_isHostileCriminals = value; OnPropertyChanged(); }
        }

        public bool IsHostileSpecial
        {
            get { return m_isHostileSpecial; }
            set { m_isHostileSpecial = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Zone> Zones
        {
            get { return m_zones; }
            set { m_zones = value; OnPropertyChanged(); }
        }

        public Zone SelectedZone
        {
            get { return m_selectedZone; }
            set { m_selectedZone = value; OnPropertyChanged(); }
        }

        public short PedDensityDay
        {
            get { return m_pedDensityDay; }
            set { m_pedDensityDay = value; OnPropertyChanged(); }
        }

        public short PedDensityNight
        {
            get { return m_pedDensityNight; }
            set { m_pedDensityNight = value; OnPropertyChanged(); }
        }

        public short CarDensityDay
        {
            get { return m_carDensityDay; }
            set { m_carDensityDay = value; OnPropertyChanged(); }
        }

        public short CarDensityNight
        {
            get { return m_carDensityNight; }
            set { m_carDensityNight = value; OnPropertyChanged(); }
        }

        public GangsTabVM()
        {
            Zones = new ObservableCollection<Zone>();
        }

        public override void Load()
        {
            base.Load();
            GangIndex = -1;
        }

        public override void Update()
        {
            base.Update();
            ReadHostility();
            FindZones();
            ReadDensity();
        }

        private ZoneInfo GetDayZone(Zone z) => TheSave.Zones.ZoneInfo[z.ZoneInfoDay];
        private ZoneInfo GetNightZone(Zone z) => TheSave.Zones.ZoneInfo[z.ZoneInfoNight];

        public void ReadHostility()
        {
            if (GangIndex == -1) return;
            IsHostilePlayer = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Player1);
            IsHostileMafia = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang1);
            IsHostileTriads = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang2);
            IsHostileDiablos = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang3);
            IsHostileYakuza = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang4);
            IsHostileYardies = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang5);
            IsHostileCartel = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang6);
            IsHostileHoods = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang7);
            IsHostileGang8 = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang8);
            IsHostileGang9 = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Gang9);
            IsHostileCivilians = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.CivMale | PedTypeFlags.CivFemale);
            IsHostileCops = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Cop);
            IsHostileEmergency = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Emergency);
            IsHostileFirefighters = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Fireman);
            IsHostileProstitutes = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Prostitute);
            IsHostileCriminals = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Criminal);
            IsHostileSpecial = TheSave.PedTypeInfo.IsThreat(GangPedType, PedTypeFlags.Special);
        }

        public void FindZones()
        {
            Zones.Clear();
            foreach (Zone z in TheSave.Zones.Zones)
            {
                if (GangIndex == -1 || (z.ChildZoneIndex == -1 && z.ParentZoneIndex == -1 && z.NextZoneIndex == -1))
                {
                    continue;
                }

                ZoneInfo dayZone = GetDayZone(z);
                ZoneInfo nightZone = GetNightZone(z);
                if (dayZone.GangPedDensity[GangIndex] > 0 || nightZone.GangPedDensity[GangIndex] > 0)
                {
                    Zones.Add(z);
                }
            }
        }

        public void ReadDensity()
        {
            if (SelectedZone == null || GangIndex == -1)
            {
                return;
            }

            ZoneInfo dayZone = GetDayZone(SelectedZone);
            ZoneInfo nightZone = GetNightZone(SelectedZone);

            // Car density is the difference from the previous value, for some reason
            short prevCarDensityDay = (GangIndex == 0) ? dayZone.CopCarDensity : dayZone.GangCarDensity[GangIndex - 1];
            short prevCarDensityNight = (GangIndex == 0) ? nightZone.CopCarDensity : nightZone.GangCarDensity[GangIndex - 1];

            CarDensityDay = (short) (dayZone.GangCarDensity[GangIndex] - prevCarDensityDay);
            CarDensityNight = (short) (nightZone.GangCarDensity[GangIndex] - prevCarDensityNight);
            PedDensityDay = dayZone.GangPedDensity[GangIndex];
            PedDensityNight = nightZone.GangPedDensity[GangIndex];
        }

        public void WritePedDensityDay()
        {
            if (SelectedZone == null || GangIndex == -1)
            {
                return;
            }

            GetDayZone(SelectedZone).GangPedDensity[GangIndex] = PedDensityDay;
            TheWindow.SetDirty();
        }

        public void WritePedDensityNight()
        {
            if (SelectedZone == null || GangIndex == -1)
            {
                return;
            }

            GetNightZone(SelectedZone).GangPedDensity[GangIndex] = PedDensityNight;
            TheWindow.SetDirty();
        }

        // TODO: figure this out

        //public void WriteCarDensityDay()
        //{
        //    if (SelectedZone == null || GangIndex == -1)
        //    {
        //        return;
        //    }

        //    ZoneInfo dayZone = GetDayZone(SelectedZone);
        //    short prevCarDensity = (GangIndex == 0) ? dayZone.CopCarDensity : dayZone.GangCarDensity[GangIndex - 1];

        //    for (int i = GangIndex; i < dayZone.GangCarDensity.Count; i++)
        //    {
        //        dayZone.GangCarDensity[i] = (short) (prevCarDensity + CarDensityDay);
        //    }
        //    TheWindow.SetDirty();
        //}

        //public void WriteCarDensityNight()
        //{
        //    if (SelectedZone == null || GangIndex == -1)
        //    {
        //        return;
        //    }

        //    ZoneInfo nightZone = GetNightZone(SelectedZone);
        //    //short prevCarDensityNight = (GangIndex == 0) ? nightZone.CarDensity : nightZone.GangCarDensity[GangIndex - 1];

        //    for (int i = GangIndex; i < nightZone.GangCarDensity.Count; i++)
        //    {
        //        nightZone.GangCarDensity[i] += CarDensityNight;
        //    }
        //    TheWindow.SetDirty();
        //}

        public ICommand SetHostilityPlayer => new RelayCommand(() => SetPedHostility(IsHostilePlayer, PedTypeFlags.Player1));
        public ICommand SetHostilityMafia => new RelayCommand(() => SetPedHostility(IsHostileMafia, PedTypeFlags.Gang1));
        public ICommand SetHostilityTriads => new RelayCommand(() => SetPedHostility(IsHostileTriads, PedTypeFlags.Gang2));
        public ICommand SetHostilityDiablos => new RelayCommand(() => SetPedHostility(IsHostileDiablos, PedTypeFlags.Gang3));
        public ICommand SetHostilityYakuza => new RelayCommand(() => SetPedHostility(IsHostileYakuza, PedTypeFlags.Gang4));
        public ICommand SetHostilityYardies => new RelayCommand(() => SetPedHostility(IsHostileYardies, PedTypeFlags.Gang5));
        public ICommand SetHostilityCartel => new RelayCommand(() => SetPedHostility(IsHostileCartel, PedTypeFlags.Gang6));
        public ICommand SetHostilityHoods => new RelayCommand(() => SetPedHostility(IsHostileHoods, PedTypeFlags.Gang7));
        public ICommand SetHostilityGang8 => new RelayCommand(() => SetPedHostility(IsHostileGang8, PedTypeFlags.Gang8));
        public ICommand SetHostilityGang9 => new RelayCommand(() => SetPedHostility(IsHostileGang9, PedTypeFlags.Gang9));
        public ICommand SetHostilityCivilians => new RelayCommand(() => SetPedHostility(IsHostileCivilians, PedTypeFlags.CivMale | PedTypeFlags.CivFemale));
        public ICommand SetHostilityCops => new RelayCommand(() => SetPedHostility(IsHostileCops, PedTypeFlags.Cop));
        public ICommand SetHostilityEmergency => new RelayCommand(() => SetPedHostility(IsHostileEmergency, PedTypeFlags.Emergency));
        public ICommand SetHostilityFirefighters => new RelayCommand(() => SetPedHostility(IsHostileFirefighters, PedTypeFlags.Fireman));
        public ICommand SetHostilityProstitutes => new RelayCommand(() => SetPedHostility(IsHostileProstitutes, PedTypeFlags.Prostitute));
        public ICommand SetHostilityCriminals => new RelayCommand(() => SetPedHostility(IsHostileCriminals, PedTypeFlags.Criminal));
        public ICommand SetHostilitySpecial => new RelayCommand(() => SetPedHostility(IsHostileSpecial, PedTypeFlags.Special));

        public void SetPedHostility(bool hostile, PedTypeFlags threats)
        {
            if (GangIndex == -1) return;

            if (hostile)
                TheSave.PedTypeInfo.AddThreat(GangPedType, threats);
            else
                TheSave.PedTypeInfo.RemoveThreat(GangPedType, threats);
        }

        private static readonly Dictionary<GangType, PedTypeId> GangPedTypeIds = new Dictionary<GangType, PedTypeId>()
        {
            { GangType.Mafia, PedTypeId.Gang1 },
            { GangType.Triads, PedTypeId.Gang2 },
            { GangType.Diablos, PedTypeId.Gang3 },
            { GangType.Yakuza, PedTypeId.Gang4 },
            { GangType.Yardies, PedTypeId.Gang5 },
            { GangType.Cartel, PedTypeId.Gang6 },
            { GangType.Hoods, PedTypeId.Gang7 },
            { GangType.Gang8, PedTypeId.Gang8 },
            { GangType.Gang9, PedTypeId.Gang9 },
        };
    }
}
