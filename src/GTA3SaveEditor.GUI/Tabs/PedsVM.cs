using System;
using System.Collections.Generic;
using System.Windows.Input;
using GTA3SaveEditor.Core.Game;
using GTASaveData.GTA3;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Tabs
{

    public class PedsVM : TabPageVM
    {
        private const PedTypeFlags PedTypeFlagsNone = 0;
        private const PedTypeFlags PedTypeFlagsRealPeds =
            PedTypeFlags.Player1 | PedTypeFlags.Player2 | PedTypeFlags.Player3 | PedTypeFlags.Player4 |
            PedTypeFlags.CivMale | PedTypeFlags.CivFemale | PedTypeFlags.Cop | PedTypeFlags.Gang1 | 
            PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 |
            PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 |
            PedTypeFlags.Emergency | PedTypeFlags.Prostitute | PedTypeFlags.Criminal | PedTypeFlags.Special;
        private const PedTypeFlags PedTypeFlagsAll =
            PedTypeFlagsRealPeds | PedTypeFlags.Gun | PedTypeFlags.CopCar | PedTypeFlags.FastCar |
            PedTypeFlags.Explosion | PedTypeFlags.Fireman | PedTypeFlags.DeadPeds;

        // TODO: gang territory zone picker
        // TODO: game bugfixes

        private int m_selectedPedIndex;
        private PedType m_selectedPed;
        private Gang m_selectedGang;

        // peds will attach ped types they are threatened by
        private bool m_isThreatPlayer1;
        private bool m_isThreatPlayer2;
        private bool m_isThreatPlayer3;
        private bool m_isThreatPlayer4;
        private bool m_isThreatCivMale;
        private bool m_isThreatCivFemale;
        private bool m_isThreatCop;
        private bool m_isThreatGang1;
        private bool m_isThreatGang2;
        private bool m_isThreatGang3;
        private bool m_isThreatGang4;
        private bool m_isThreatGang5;
        private bool m_isThreatGang6;
        private bool m_isThreatGang7;
        private bool m_isThreatGang8;
        private bool m_isThreatGang9;
        private bool m_isThreatEmergency;
        private bool m_isThreatFireman;
        private bool m_isThreatProstitute;
        private bool m_isThreatCriminal;
        private bool m_isThreatSpecial;
        private bool m_isThreatGun;
        private bool m_isThreatCopCar;
        private bool m_isThreatFastCar;
        private bool m_isThreatExplosion;
        private bool m_isThreatDeadPeds;

        // peds will run away from ped types they avoid
        private bool m_isAvoidPlayer1;
        private bool m_isAvoidPlayer2;
        private bool m_isAvoidPlayer3;
        private bool m_isAvoidPlayer4;
        private bool m_isAvoidCivMale;
        private bool m_isAvoidCivFemale;
        private bool m_isAvoidCop;
        private bool m_isAvoidGang1;
        private bool m_isAvoidGang2;
        private bool m_isAvoidGang3;
        private bool m_isAvoidGang4;
        private bool m_isAvoidGang5;
        private bool m_isAvoidGang6;
        private bool m_isAvoidGang7;
        private bool m_isAvoidGang8;
        private bool m_isAvoidGang9;
        private bool m_isAvoidEmergency;
        private bool m_isAvoidFireman;
        private bool m_isAvoidProstitute;
        private bool m_isAvoidCriminal;
        private bool m_isAvoidSpecial;
        private bool m_isAvoidGun;
        private bool m_isAvoidCopCar;
        private bool m_isAvoidFastCar;
        private bool m_isAvoidExplosion;
        private bool m_isAvoidDeadPeds;

        public int SelectedPedIndex
        {
            get { return m_selectedPedIndex; }
            set { m_selectedPedIndex = value; OnPropertyChanged(); }
        }

        public PedTypeId SelectedPedType
        {
            get { return (PedTypeId) SelectedPedIndex; }
            set { SelectedPedIndex = (int) value; OnPropertyChanged(); }
        }

        public PedType SelectedPed
        {
            get { return m_selectedPed; }
            set { m_selectedPed = value; OnPropertyChanged(); }
        }

        public Gang SelectedGang
        {
            get { return m_selectedGang; }
            set { m_selectedGang = value; OnPropertyChanged(); }
        }

        public bool IsSelectedPedTypeGang
        {
            get { return SelectedPedType >= PedTypeId.Gang1 && SelectedPedType <= PedTypeId.Gang9; }
        }

        public bool IsThreatPlayer1
        {
            get { return m_isThreatPlayer1; }
            set { m_isThreatPlayer1 = value; OnPropertyChanged(); }
        }

        public bool IsThreatPlayer2
        {
            get { return m_isThreatPlayer2; }
            set { m_isThreatPlayer2 = value; OnPropertyChanged(); }
        }

        public bool IsThreatPlayer3
        {
            get { return m_isThreatPlayer3; }
            set { m_isThreatPlayer3 = value; OnPropertyChanged(); }
        }

        public bool IsThreatPlayer4
        {
            get { return m_isThreatPlayer4; }
            set { m_isThreatPlayer4 = value; OnPropertyChanged(); }
        }

        public bool IsThreatCivMale
        {
            get { return m_isThreatCivMale; }
            set { m_isThreatCivMale = value; OnPropertyChanged(); }
        }

        public bool IsThreatCivFemale
        {
            get { return m_isThreatCivFemale; }
            set { m_isThreatCivFemale = value; OnPropertyChanged(); }
        }

        public bool IsThreatCop
        {
            get { return m_isThreatCop; }
            set { m_isThreatCop = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang1
        {
            get { return m_isThreatGang1; }
            set { m_isThreatGang1 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang2
        {
            get { return m_isThreatGang2; }
            set { m_isThreatGang2 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang3
        {
            get { return m_isThreatGang3; }
            set { m_isThreatGang3 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang4
        {
            get { return m_isThreatGang4; }
            set { m_isThreatGang4 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang5
        {
            get { return m_isThreatGang5; }
            set { m_isThreatGang5 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang6
        {
            get { return m_isThreatGang6; }
            set { m_isThreatGang6 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang7
        {
            get { return m_isThreatGang7; }
            set { m_isThreatGang7 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang8
        {
            get { return m_isThreatGang8; }
            set { m_isThreatGang8 = value; OnPropertyChanged(); }
        }

        public bool IsThreatGang9
        {
            get { return m_isThreatGang9; }
            set { m_isThreatGang9 = value; OnPropertyChanged(); }
        }

        public bool IsThreatEmergency
        {
            get { return m_isThreatEmergency; }
            set { m_isThreatEmergency = value; OnPropertyChanged(); }
        }

        public bool IsThreatFireman
        {
            get { return m_isThreatFireman; }
            set { m_isThreatFireman = value; OnPropertyChanged(); }
        }

        public bool IsThreatProstitute
        {
            get { return m_isThreatProstitute; }
            set { m_isThreatProstitute = value; OnPropertyChanged(); }
        }

        public bool IsThreatCriminal
        {
            get { return m_isThreatCriminal; }
            set { m_isThreatCriminal = value; OnPropertyChanged(); }
        }

        public bool IsThreatSpecial
        {
            get { return m_isThreatSpecial; }
            set { m_isThreatSpecial = value; OnPropertyChanged(); }
        }

        public bool IsThreatGun
        {
            get { return m_isThreatGun; }
            set { m_isThreatGun = value; OnPropertyChanged(); }
        }

        public bool IsThreatCopCar
        {
            get { return m_isThreatCopCar; }
            set { m_isThreatCopCar = value; OnPropertyChanged(); }
        }

        public bool IsThreatFastCar
        {
            get { return m_isThreatFastCar; }
            set { m_isThreatFastCar = value; OnPropertyChanged(); }
        }

        public bool IsThreatExplosion
        {
            get { return m_isThreatExplosion; }
            set { m_isThreatExplosion = value; OnPropertyChanged(); }
        }

        public bool IsThreatDeadPeds
        {
            get { return m_isThreatDeadPeds; }
            set { m_isThreatDeadPeds = value; OnPropertyChanged(); }
        }

        public bool IsAvoidPlayer1
        {
            get { return m_isAvoidPlayer1; }
            set { m_isAvoidPlayer1 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidPlayer2
        {
            get { return m_isAvoidPlayer2; }
            set { m_isAvoidPlayer2 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidPlayer3
        {
            get { return m_isAvoidPlayer3; }
            set { m_isAvoidPlayer3 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidPlayer4
        {
            get { return m_isAvoidPlayer4; }
            set { m_isAvoidPlayer4 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidCivMale
        {
            get { return m_isAvoidCivMale; }
            set { m_isAvoidCivMale = value; OnPropertyChanged(); }
        }

        public bool IsAvoidCivFemale
        {
            get { return m_isAvoidCivFemale; }
            set { m_isAvoidCivFemale = value; OnPropertyChanged(); }
        }

        public bool IsAvoidCop
        {
            get { return m_isAvoidCop; }
            set { m_isAvoidCop = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang1
        {
            get { return m_isAvoidGang1; }
            set { m_isAvoidGang1 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang2
        {
            get { return m_isAvoidGang2; }
            set { m_isAvoidGang2 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang3
        {
            get { return m_isAvoidGang3; }
            set { m_isAvoidGang3 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang4
        {
            get { return m_isAvoidGang4; }
            set { m_isAvoidGang4 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang5
        {
            get { return m_isAvoidGang5; }
            set { m_isAvoidGang5 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang6
        {
            get { return m_isAvoidGang6; }
            set { m_isAvoidGang6 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang7
        {
            get { return m_isAvoidGang7; }
            set { m_isAvoidGang7 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang8
        {
            get { return m_isAvoidGang8; }
            set { m_isAvoidGang8 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGang9
        {
            get { return m_isAvoidGang9; }
            set { m_isAvoidGang9 = value; OnPropertyChanged(); }
        }

        public bool IsAvoidEmergency
        {
            get { return m_isAvoidEmergency; }
            set { m_isAvoidEmergency = value; OnPropertyChanged(); }
        }

        public bool IsAvoidFireman
        {
            get { return m_isAvoidFireman; }
            set { m_isAvoidFireman = value; OnPropertyChanged(); }
        }

        public bool IsAvoidProstitute
        {
            get { return m_isAvoidProstitute; }
            set { m_isAvoidProstitute = value; OnPropertyChanged(); }
        }

        public bool IsAvoidCriminal
        {
            get { return m_isAvoidCriminal; }
            set { m_isAvoidCriminal = value; OnPropertyChanged(); }
        }

        public bool IsAvoidSpecial
        {
            get { return m_isAvoidSpecial; }
            set { m_isAvoidSpecial = value; OnPropertyChanged(); }
        }

        public bool IsAvoidGun
        {
            get { return m_isAvoidGun; }
            set { m_isAvoidGun = value; OnPropertyChanged(); }
        }

        public bool IsAvoidCopCar
        {
            get { return m_isAvoidCopCar; }
            set { m_isAvoidCopCar = value; OnPropertyChanged(); }
        }

        public bool IsAvoidFastCar
        {
            get { return m_isAvoidFastCar; }
            set { m_isAvoidFastCar = value; OnPropertyChanged(); }
        }

        public bool IsAvoidExplosion
        {
            get { return m_isAvoidExplosion; }
            set { m_isAvoidExplosion = value; OnPropertyChanged(); }
        }

        public bool IsAvoidDeadPeds
        {
            get { return m_isAvoidDeadPeds; }
            set { m_isAvoidDeadPeds = value; OnPropertyChanged(); }
        }

        public PedsVM()
        { }

        public override void Load()
        {
            base.Load();
            SelectedPedIndex = -1;
            SelectedGang = null;
            SelectedPed = null;

            // TODO: ensure every pedtype has correct 'flag' set
            // offer bug fix if mismatch detected
        }

        public override void Update()
        {
            base.Update();
            ReadThreats();
            ReadAvoids();
        }

        public void ReadThreats()
        {
            IsThreatPlayer1 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Player1);
            IsThreatPlayer2 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Player2);
            IsThreatPlayer3 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Player3);
            IsThreatPlayer4 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Player4);
            IsThreatCivMale = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.CivMale);
            IsThreatCivFemale = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.CivFemale);
            IsThreatCop = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Cop);
            IsThreatGang1 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang1);
            IsThreatGang2 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang2);
            IsThreatGang3 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang3);
            IsThreatGang4 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang4);
            IsThreatGang5 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang5);
            IsThreatGang6 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang6);
            IsThreatGang7 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang7);
            IsThreatGang8 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang8);
            IsThreatGang9 = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gang9);
            IsThreatEmergency = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Emergency);
            IsThreatFireman = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Fireman);
            IsThreatProstitute = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Prostitute);
            IsThreatCriminal = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Criminal);
            IsThreatSpecial = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Special);
            IsThreatGun = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Gun);
            IsThreatCopCar = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.CopCar);
            IsThreatFastCar = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.FastCar);
            IsThreatExplosion = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.Explosion);
            IsThreatDeadPeds = TheSave.PedTypeInfo.IsThreat(SelectedPedType, PedTypeFlags.DeadPeds);
        }

        public void ReadAvoids()
        {
            IsAvoidPlayer1 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Player1);
            IsAvoidPlayer2 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Player2);
            IsAvoidPlayer3 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Player3);
            IsAvoidPlayer4 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Player4);
            IsAvoidCivMale = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.CivMale);
            IsAvoidCivFemale = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.CivFemale);
            IsAvoidCop = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Cop);
            IsAvoidGang1 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang1);
            IsAvoidGang2 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang2);
            IsAvoidGang3 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang3);
            IsAvoidGang4 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang4);
            IsAvoidGang5 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang5);
            IsAvoidGang6 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang6);
            IsAvoidGang7 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang7);
            IsAvoidGang8 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang8);
            IsAvoidGang9 = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gang9);
            IsAvoidEmergency = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Emergency);
            IsAvoidFireman = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Fireman);
            IsAvoidProstitute = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Prostitute);
            IsAvoidCriminal = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Criminal);
            IsAvoidSpecial = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Special);
            IsAvoidGun = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Gun);
            IsAvoidCopCar = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.CopCar);
            IsAvoidFastCar = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.FastCar);
            IsAvoidExplosion = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.Explosion);
            IsAvoidDeadPeds = TheSave.PedTypeInfo.IsAvoid(SelectedPedType, PedTypeFlags.DeadPeds);
        }

        public ICommand SetThreatPlayer1 => new RelayCommand(() => SetPedThreat(IsThreatPlayer1, PedTypeFlags.Player1));
        public ICommand SetThreatPlayer2 => new RelayCommand(() => SetPedThreat(IsThreatPlayer2, PedTypeFlags.Player2));
        public ICommand SetThreatPlayer3 => new RelayCommand(() => SetPedThreat(IsThreatPlayer3, PedTypeFlags.Player3));
        public ICommand SetThreatPlayer4 => new RelayCommand(() => SetPedThreat(IsThreatPlayer4, PedTypeFlags.Player4));
        public ICommand SetThreatCivMale => new RelayCommand(() => SetPedThreat(IsThreatCivMale, PedTypeFlags.CivMale));
        public ICommand SetThreatCivFemale => new RelayCommand(() => SetPedThreat(IsThreatCivFemale, PedTypeFlags.CivFemale));
        public ICommand SetThreatCop => new RelayCommand(() => SetPedThreat(IsThreatCop, PedTypeFlags.Cop));
        public ICommand SetThreatGang1 => new RelayCommand(() => SetPedThreat(IsThreatGang1, PedTypeFlags.Gang1));
        public ICommand SetThreatGang2 => new RelayCommand(() => SetPedThreat(IsThreatGang2, PedTypeFlags.Gang2));
        public ICommand SetThreatGang3 => new RelayCommand(() => SetPedThreat(IsThreatGang3, PedTypeFlags.Gang3));
        public ICommand SetThreatGang4 => new RelayCommand(() => SetPedThreat(IsThreatGang4, PedTypeFlags.Gang4));
        public ICommand SetThreatGang5 => new RelayCommand(() => SetPedThreat(IsThreatGang5, PedTypeFlags.Gang5));
        public ICommand SetThreatGang6 => new RelayCommand(() => SetPedThreat(IsThreatGang6, PedTypeFlags.Gang6));
        public ICommand SetThreatGang7 => new RelayCommand(() => SetPedThreat(IsThreatGang7, PedTypeFlags.Gang7));
        public ICommand SetThreatGang8 => new RelayCommand(() => SetPedThreat(IsThreatGang8, PedTypeFlags.Gang8));
        public ICommand SetThreatGang9 => new RelayCommand(() => SetPedThreat(IsThreatGang9, PedTypeFlags.Gang9));
        public ICommand SetThreatEmergency => new RelayCommand(() => SetPedThreat(IsThreatEmergency, PedTypeFlags.Emergency));
        public ICommand SetThreatFireman => new RelayCommand(() => SetPedThreat(IsThreatFireman, PedTypeFlags.Fireman));
        public ICommand SetThreatProstitute => new RelayCommand(() => SetPedThreat(IsThreatProstitute, PedTypeFlags.Prostitute));
        public ICommand SetThreatCriminal => new RelayCommand(() => SetPedThreat(IsThreatCriminal, PedTypeFlags.Criminal));
        public ICommand SetThreatSpecial => new RelayCommand(() => SetPedThreat(IsThreatSpecial, PedTypeFlags.Special));
        public ICommand SetThreatGun => new RelayCommand(() => SetPedThreat(IsThreatGun, PedTypeFlags.Gun));
        public ICommand SetThreatCopCar => new RelayCommand(() => SetPedThreat(IsThreatCopCar, PedTypeFlags.CopCar));
        public ICommand SetThreatFastCar => new RelayCommand(() => SetPedThreat(IsThreatFastCar, PedTypeFlags.FastCar));
        public ICommand SetThreatExplosion => new RelayCommand(() => SetPedThreat(IsThreatExplosion, PedTypeFlags.Explosion));
        public ICommand SetThreatDeadPeds => new RelayCommand(() => SetPedThreat(IsThreatDeadPeds, PedTypeFlags.DeadPeds));

        public ICommand HostileAll => new RelayCommand(() =>
        {
            SetPedThreat(true, PedTypeFlagsAll);
        });

        public ICommand HostileNone => new RelayCommand(() =>
        {
            SetPedThreat(false, PedTypeFlagsAll);
        });

        public ICommand ResetThreats => new RelayCommand(() =>
        {
            SetPedThreat(null, DefaultThreats[SelectedPedType]);
        });

        public void SetPedThreat(bool? addRemSet, PedTypeFlags pedTypes)
        {
            if (SelectedPedIndex == -1) return;

            switch (addRemSet)
            {
                case true: TheSave.PedTypeInfo.AddThreat(SelectedPedType, pedTypes); break;
                case false: TheSave.PedTypeInfo.RemoveThreat(SelectedPedType, pedTypes); break;
                case null: TheSave.PedTypeInfo.SetThreat(SelectedPedType, pedTypes); break;
            }

            ReadThreats();
        }

        public ICommand SetAvoidPlayer1 => new RelayCommand(() => SetPedAvoid(IsAvoidPlayer1, PedTypeFlags.Player1));
        public ICommand SetAvoidPlayer2 => new RelayCommand(() => SetPedAvoid(IsAvoidPlayer2, PedTypeFlags.Player2));
        public ICommand SetAvoidPlayer3 => new RelayCommand(() => SetPedAvoid(IsAvoidPlayer3, PedTypeFlags.Player3));
        public ICommand SetAvoidPlayer4 => new RelayCommand(() => SetPedAvoid(IsAvoidPlayer4, PedTypeFlags.Player4));
        public ICommand SetAvoidCivMale => new RelayCommand(() => SetPedAvoid(IsAvoidCivMale, PedTypeFlags.CivMale));
        public ICommand SetAvoidCivFemale => new RelayCommand(() => SetPedAvoid(IsAvoidCivFemale, PedTypeFlags.CivFemale));
        public ICommand SetAvoidCop => new RelayCommand(() => SetPedAvoid(IsAvoidCop, PedTypeFlags.Cop));
        public ICommand SetAvoidGang1 => new RelayCommand(() => SetPedAvoid(IsAvoidGang1, PedTypeFlags.Gang1));
        public ICommand SetAvoidGang2 => new RelayCommand(() => SetPedAvoid(IsAvoidGang2, PedTypeFlags.Gang2));
        public ICommand SetAvoidGang3 => new RelayCommand(() => SetPedAvoid(IsAvoidGang3, PedTypeFlags.Gang3));
        public ICommand SetAvoidGang4 => new RelayCommand(() => SetPedAvoid(IsAvoidGang4, PedTypeFlags.Gang4));
        public ICommand SetAvoidGang5 => new RelayCommand(() => SetPedAvoid(IsAvoidGang5, PedTypeFlags.Gang5));
        public ICommand SetAvoidGang6 => new RelayCommand(() => SetPedAvoid(IsAvoidGang6, PedTypeFlags.Gang6));
        public ICommand SetAvoidGang7 => new RelayCommand(() => SetPedAvoid(IsAvoidGang7, PedTypeFlags.Gang7));
        public ICommand SetAvoidGang8 => new RelayCommand(() => SetPedAvoid(IsAvoidGang8, PedTypeFlags.Gang8));
        public ICommand SetAvoidGang9 => new RelayCommand(() => SetPedAvoid(IsAvoidGang9, PedTypeFlags.Gang9));
        public ICommand SetAvoidEmergency => new RelayCommand(() => SetPedAvoid(IsAvoidEmergency, PedTypeFlags.Emergency));
        public ICommand SetAvoidFireman => new RelayCommand(() => SetPedAvoid(IsAvoidFireman, PedTypeFlags.Fireman));
        public ICommand SetAvoidProstitute => new RelayCommand(() => SetPedAvoid(IsAvoidProstitute, PedTypeFlags.Prostitute));
        public ICommand SetAvoidCriminal => new RelayCommand(() => SetPedAvoid(IsAvoidCriminal, PedTypeFlags.Criminal));
        public ICommand SetAvoidSpecial => new RelayCommand(() => SetPedAvoid(IsAvoidSpecial, PedTypeFlags.Special));
        public ICommand SetAvoidGun => new RelayCommand(() => SetPedAvoid(IsAvoidGun, PedTypeFlags.Gun));
        public ICommand SetAvoidCopCar => new RelayCommand(() => SetPedAvoid(IsAvoidCopCar, PedTypeFlags.CopCar));
        public ICommand SetAvoidFastCar => new RelayCommand(() => SetPedAvoid(IsAvoidFastCar, PedTypeFlags.FastCar));
        public ICommand SetAvoidExplosion => new RelayCommand(() => SetPedAvoid(IsAvoidExplosion, PedTypeFlags.Explosion));
        public ICommand SetAvoidDeadPeds => new RelayCommand(() => SetPedAvoid(IsAvoidDeadPeds, PedTypeFlags.DeadPeds));

        public ICommand AvoidAll => new RelayCommand(() =>
        {
            SetPedAvoid(true, PedTypeFlagsAll);
        });

        public ICommand AvoidNone => new RelayCommand(() =>
        {
            SetPedAvoid(false, PedTypeFlagsAll);
        });

        public ICommand ResetAvoids => new RelayCommand(() =>
        {
            SetPedAvoid(null, DefaultAvoids[SelectedPedType]);
        });

        public void SetPedAvoid(bool? addRemSet, PedTypeFlags pedTypes)
        {
            if (SelectedPedIndex == -1) return;

            switch (addRemSet)
            {
                case true: TheSave.PedTypeInfo.AddAvoid(SelectedPedType, pedTypes); break;
                case false: TheSave.PedTypeInfo.RemoveAvoid(SelectedPedType, pedTypes); break;
                case null: TheSave.PedTypeInfo.SetAvoid(SelectedPedType, pedTypes); break;
            }

            ReadAvoids();
        }

        public ICommand ResetGang => new RelayCommand(() =>
        {
            if (!IsSelectedPedTypeGang) return;

            GangType g = GetGangType(SelectedPedType);
            SetGangToDefaults(g);
        });

        private void SetGangToDefaults(GangType type)
        {
            Gang defaultGang = DefaultGangs[type];

            TheSave.Gangs[type].VehicleModel = defaultGang.VehicleModel;
            TheSave.Gangs[type].Weapon1 = defaultGang.Weapon1;
            TheSave.Gangs[type].Weapon2 = defaultGang.Weapon2;
            TheSave.Gangs[type].PedModelOverride = defaultGang.PedModelOverride;
        }

        public ICommand PublicEnemy => new RelayCommand(() =>
        {
            foreach (PedTypeId pedType in Enum.GetValues(typeof(PedTypeId)))
            {
                if (pedType >= PedTypeId.CivMale && pedType < PedTypeId.Special)
                {
                    TheSave.PedTypeInfo.AddThreat(pedType, PedTypeFlags.Player1);
                }
            }
            ReadThreats();
        });

        public ICommand LikableGuy => new RelayCommand(() =>
        {
            foreach (PedTypeId pedType in Enum.GetValues(typeof(PedTypeId)))
            {
                if (pedType >= PedTypeId.CivMale && pedType < PedTypeId.Special)
                {
                    TheSave.PedTypeInfo.RemoveThreat(pedType, PedTypeFlags.Player1);
                }
            }
            ReadThreats();
        });

        public ICommand Mayhem => new RelayCommand(() =>
        {
            foreach (PedTypeId pedType in Enum.GetValues(typeof(PedTypeId)))
            {
                if (pedType >= PedTypeId.CivMale && pedType < PedTypeId.Special)
                {
                    TheSave.PedTypeInfo.SetThreat(pedType, PedTypeFlagsRealPeds);
                }
            }
            ReadThreats();
        });

        public ICommand WorldPeace => new RelayCommand(() =>
        {
            foreach (PedTypeId pedType in Enum.GetValues(typeof(PedTypeId)))
            {
                if (pedType >= PedTypeId.CivMale && pedType < PedTypeId.Special)
                {
                    TheSave.PedTypeInfo.RemoveThreat(pedType, PedTypeFlagsRealPeds);
                }
            }
            ReadThreats();
        });

        public ICommand FixNines => new RelayCommand(() =>
        {
            Gang hoods = TheSave.Gangs[GangType.Hoods];
            hoods.PedModelOverride = -1;
        });

        public ICommand FixPeds => new RelayCommand(() =>
        {
            for (int i = 0; i < PedTypeData.NumPedTypes; i++)
            {
                PedType ped = TheSave.PedTypeInfo.PedTypes[i];
                PedTypeId id = (PedTypeId) i;
                ped.Flag = PedType.GetFlag(id);
                ped.Threats = DefaultThreats[id];
                ped.Avoids = DefaultAvoids[id];

                // TODO: set gang hostility according to story vars
            }

            ReadThreats();
            ReadAvoids();
        });

        public ICommand ResetGangs => new RelayCommand(() =>
        {
            // Set default threats/avoids
            for (int i = 0; i < PedTypeData.NumPedTypes; i++)
            {
                PedType ped = TheSave.PedTypeInfo.PedTypes[i];
                PedTypeId id = (PedTypeId) i;

                if (ped.IsGang)
                {
                    ped.Threats = DefaultThreats[id];
                    ped.Avoids = DefaultAvoids[id];

                    GangType g = GetGangType(id);
                    SetGangToDefaults(g);
                }
            }

            ReadThreats();
            ReadAvoids();
        });

        private static GangType GetGangType(PedTypeId pedType)
        {
            if (pedType < PedTypeId.Gang1 || pedType > PedTypeId.Gang9)
            {
                throw new InvalidOperationException($"PedType '{pedType}' is not a gang.");
            }

            return (GangType) (pedType - (int) PedTypeId.Gang1);
        }

        public static Dictionary<PedTypeId, PedTypeFlags> DefaultThreats = new Dictionary<PedTypeId, PedTypeFlags>()
        {
            { PedTypeId.Player1,    PedTypeFlagsNone },
            { PedTypeId.Player2,    PedTypeFlagsNone },
            { PedTypeId.Player3,    PedTypeFlagsNone },
            { PedTypeId.Player4,    PedTypeFlagsNone },
            { PedTypeId.CivMale,    PedTypeFlags.Gun | PedTypeFlags.Explosion | PedTypeFlags.DeadPeds },
            { PedTypeId.CivFemale,  PedTypeFlags.Gun | PedTypeFlags.Explosion | PedTypeFlags.DeadPeds },
            { PedTypeId.Cop,        PedTypeFlags.Gun | PedTypeFlags.Explosion | PedTypeFlags.DeadPeds },
            { PedTypeId.Gang1,      PedTypeFlags.Gun | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang2,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang3,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang4,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang5,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang6,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion | PedTypeFlags.Player1 },
            { PedTypeId.Gang7,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang8,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang9 | PedTypeFlags.Explosion },
            { PedTypeId.Gang9,      PedTypeFlags.Gun | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Explosion },
            { PedTypeId.Emergency,  PedTypeFlags.Explosion },
            { PedTypeId.Criminal,   PedTypeFlags.Gun | PedTypeFlags.Cop | PedTypeFlags.CopCar | PedTypeFlags.Explosion },
            { PedTypeId.Prostitute, PedTypeFlags.Gun | PedTypeFlags.Explosion | PedTypeFlags.DeadPeds },
            { PedTypeId.Fireman,    PedTypeFlagsNone },
            { PedTypeId.Special,    PedTypeFlagsNone },
            { PedTypeId.Unused1,    PedTypeFlagsNone },
            { PedTypeId.Unused2,    PedTypeFlagsNone },
        };

        public static Dictionary<PedTypeId, PedTypeFlags> DefaultAvoids = new Dictionary<PedTypeId, PedTypeFlags>()
        {
            { PedTypeId.Player1,    PedTypeFlagsNone },
            { PedTypeId.Player2,    PedTypeFlagsNone },
            { PedTypeId.Player3,    PedTypeFlagsNone },
            { PedTypeId.Player4,    PedTypeFlagsNone },
            { PedTypeId.CivMale,    PedTypeFlags.Player1 | PedTypeFlags.Player2 | PedTypeFlags.Player3 | PedTypeFlags.Player4 | PedTypeFlags.CivMale | PedTypeFlags.CivFemale | PedTypeFlags.Cop | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Criminal | PedTypeFlags.Special },
            { PedTypeId.CivFemale,  PedTypeFlags.Player1 | PedTypeFlags.Player2 | PedTypeFlags.Player3 | PedTypeFlags.Player4 | PedTypeFlags.CivMale | PedTypeFlags.CivFemale | PedTypeFlags.Cop | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 | PedTypeFlags.Criminal | PedTypeFlags.Special },
            { PedTypeId.Cop,        PedTypeFlags.CivMale | PedTypeFlags.CivFemale | PedTypeFlags.Cop },
            { PedTypeId.Gang1,      PedTypeFlags.Gang1 },
            { PedTypeId.Gang2,      PedTypeFlags.Gang2 },
            { PedTypeId.Gang3,      PedTypeFlags.Gang3 },
            { PedTypeId.Gang4,      PedTypeFlags.Gang4 },
            { PedTypeId.Gang5,      PedTypeFlags.Gang5 },
            { PedTypeId.Gang6,      PedTypeFlags.Gang6 },
            { PedTypeId.Gang7,      PedTypeFlags.Gang7 },
            { PedTypeId.Gang8,      PedTypeFlags.Gang8 },
            { PedTypeId.Gang9,      PedTypeFlags.Gang9 },
            { PedTypeId.Emergency,  PedTypeFlags.CivMale | PedTypeFlags.CivFemale | PedTypeFlags.Cop },
            { PedTypeId.Criminal,   PedTypeFlags.Cop | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 },
            { PedTypeId.Prostitute, PedTypeFlags.Cop | PedTypeFlags.Gang1 | PedTypeFlags.Gang2 | PedTypeFlags.Gang3 | PedTypeFlags.Gang4 | PedTypeFlags.Gang5 | PedTypeFlags.Gang6 | PedTypeFlags.Gang7 | PedTypeFlags.Gang8 | PedTypeFlags.Gang9 },
            { PedTypeId.Fireman,    PedTypeFlagsNone },
            { PedTypeId.Special,    PedTypeFlagsNone },
            { PedTypeId.Unused1,    PedTypeFlagsNone },
            { PedTypeId.Unused2,    PedTypeFlagsNone },
        };

        public static Dictionary<GangType, Gang> DefaultGangs = new Dictionary<GangType, Gang>()
        {
            { GangType.Mafia, new Gang() { VehicleModel = (int) ModelIndex.MAFIA, Weapon1 = WeaponType.Colt45, Weapon2 = WeaponType.Colt45 } },
            { GangType.Triads, new Gang() { VehicleModel = (int) ModelIndex.BELLYUP, Weapon1 = WeaponType.Colt45, Weapon2 = WeaponType.BaseballBat } },
            { GangType.Diablos, new Gang() { VehicleModel = (int) ModelIndex.DIABLOS, Weapon1 = WeaponType.BaseballBat, Weapon2 = WeaponType.None } },
            { GangType.Yakuza, new Gang() { VehicleModel = (int) ModelIndex.YAKUZA, Weapon1 = WeaponType.Colt45, Weapon2 = WeaponType.Uzi } },
            { GangType.Yardies, new Gang() { VehicleModel = (int) ModelIndex.YARDIE, Weapon1 = WeaponType.BaseballBat, Weapon2 = WeaponType.Colt45 } },
            { GangType.Cartel, new Gang() { VehicleModel = (int) ModelIndex.COLUMB, Weapon1 = WeaponType.Colt45, Weapon2 = WeaponType.Uzi } },
            { GangType.Hoods, new Gang() { VehicleModel = (int) ModelIndex.HOODS, Weapon1 = WeaponType.Uzi, Weapon2 = WeaponType.Colt45 } },
            { GangType.Gang8, new Gang() },
            { GangType.Gang9, new Gang() },
        };
    }
}
