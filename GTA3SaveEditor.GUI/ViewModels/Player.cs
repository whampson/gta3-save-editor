using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Player : TabPageViewModelBase
    {
        private PlayerPed m_playerPed;
        private PlayerInfo m_playerInfo;

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

        public Player(Main mainViewModel)
           : base("Player", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            PlayerPed = MainWindow.TheSave.PlayerPeds.GetPlayerPed();
            PlayerInfo = MainWindow.TheSave.PlayerInfo;
        }
    }
}
