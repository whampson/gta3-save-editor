﻿using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Player : BaseTabPage
    {
        private PlayerPed m_playerPed;
        private PlayerInfo m_playerInfo;
        private GarageData m_garages;

        public PlayerPed PlayerPed
        {
            get { return m_playerPed; }
            set { m_playerPed = value; OnPropertyChanged(); }
        }

        public PlayerInfo PlayerInfo
        {
            get { return m_playerInfo; }
            set { m_playerInfo = value; OnPropertyChanged(); }
        }

        public GarageData Garages
        {
            get { return m_garages; }
            set { m_garages = value; OnPropertyChanged(); }
        }

        public Player(Main mainViewModel)
           : base("Player", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        public override void Load()
        {
            base.Load();
            PlayerPed = MainViewModel.TheSave.PlayerPeds.GetPlayerPed();
            PlayerInfo = MainViewModel.TheSave.PlayerInfo;
            Garages = MainViewModel.TheSave.Garages;
        }
    }
}
