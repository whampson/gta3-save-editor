using GTASaveData.GTA3;
using System;
using System.ComponentModel;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class GeneralViewModel : TabPageViewModelBase
    {
        private SimpleVariables m_simpleVars;
        private bool? m_fixedPurpleNinesGlitch;
        private bool? m_fixedHostilePeds;
        private bool? m_fixedPercentageBug;

        public bool IsAndroid
        {
            get { return MainViewModel.TheSave.FileFormat.IsAndroid; }
        }

        public bool IsPS2
        {
            get { return MainViewModel.TheSave.FileFormat.IsPS2; }
        }

        public SimpleVariables SimpleVars
        {
            get { return m_simpleVars; }
            set { m_simpleVars = value; OnPropertyChanged(); }
        }
        public string LastMissionPassedName
        {
            get { return SimpleVars.LastMissionPassedName; }
            set
            {
                if (value.Length > SimpleVariables.MaxMissionPassedNameLength - 1)
                {
                    value = value.Substring(0, SimpleVariables.MaxMissionPassedNameLength - 1);
                }
                SimpleVars.LastMissionPassedName = value;
                OnPropertyChanged();
            }
        }

        public DateTime GameClock
        {
            get { return new DateTime(1, 1, 1, SimpleVars.GameClockHours, SimpleVars.GameClockMinutes, 0); }
            set
            {
                SimpleVars.GameClockHours = (byte) value.Hour;
                SimpleVars.GameClockMinutes = (byte) value.Minute;
                OnPropertyChanged();
            }
        }

        public PadMode CurrentPadMode
        {
            get { return (PadMode) SimpleVars.CurrPadMode; }
            set { SimpleVars.CurrPadMode = (short) value; OnPropertyChanged(); }
        }

        public OnFootCameraMode OnFootCameraMode
        {
            get { return (OnFootCameraMode) SimpleVars.CameraPedZoomIndicator; }
            set { SimpleVars.CameraPedZoomIndicator = (float) value; OnPropertyChanged(); }
        }

        public InCarCameraMode InCarCameraMode
        {
            get { return (InCarCameraMode) SimpleVars.CameraCarZoomIndicator; }
            set { SimpleVars.CameraCarZoomIndicator = (float) value; OnPropertyChanged(); }
        }

        public static WeatherType[] WeatherList = new WeatherType[]
        {
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Cloudy, WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Rainy,
            WeatherType.Cloudy, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Cloudy, WeatherType.Foggy, WeatherType.Foggy, WeatherType.Cloudy,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Cloudy, WeatherType.Cloudy,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Cloudy, WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Rainy,
            WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Cloudy, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Foggy, WeatherType.Foggy, WeatherType.Sunny,
            WeatherType.Sunny, WeatherType.Sunny, WeatherType.Rainy, WeatherType.Cloudy,
        };

        public bool? FixedPurpleNinesGlitch
        {
            get { return m_fixedPurpleNinesGlitch; }
            set { m_fixedPurpleNinesGlitch = value; OnPropertyChanged(); }
        }

        public bool? FixedHostilePeds
        {
            get { return m_fixedHostilePeds; }
            set { m_fixedHostilePeds = value; OnPropertyChanged(); }
        }

        public bool? FixedPercentageBug
        {
            get { return m_fixedPercentageBug; }
            set { m_fixedPercentageBug = value; OnPropertyChanged(); }
        }

        public GeneralViewModel(MainViewModel mainViewModel)
            : base("General", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            SimpleVars = MainViewModel.TheSave.SimpleVars;

            DetectPurpleNinesGlitch();
            DetectHostilePeds();
            DetectPercentageBug();

            OnPropertyChanged(nameof(IsAndroid));
            OnPropertyChanged(nameof(IsPS2));
            OnPropertyChanged(nameof(LastMissionPassedName));
            OnPropertyChanged(nameof(GameClock));
            OnPropertyChanged(nameof(CurrentPadMode));
            OnPropertyChanged(nameof(OnFootCameraMode));
            OnPropertyChanged(nameof(InCarCameraMode));
        }

        protected override void Shutdown()
        {
            base.Shutdown();
        }

        private void DetectPurpleNinesGlitch()
        {
            // TODO: MainViewModel.TheSave.Gangs[GangType.Hoods]
            Gang hoods = MainViewModel.TheSave.Gangs.Gangs[(int) GangType.Hoods];

            if (hoods.PedModelOverride != -1)
            {
                FixedPurpleNinesGlitch = false;
            }
            {
                FixedPurpleNinesGlitch = null;
            }
        }

        private void FixPurpleNinesGlitch()
        {
            Gang hoods = MainViewModel.TheSave.Gangs.Gangs[(int) GangType.Hoods];
            hoods.PedModelOverride = -1;

            FixedPurpleNinesGlitch = true;
        }

        private void DetectHostilePeds()
        {
            // TODO:
            FixedHostilePeds = null;
        }

        private void FixHostilePeds()
        {
            // TODO:
            FixedHostilePeds = true;
        }

        private void DetectPercentageBug()
        {
            Stats stats = MainViewModel.TheSave.Stats;

            // TODO: && ScmVersion == 1
            if (stats.TotalProgressInGame == 156)
            {
                FixedPercentageBug = false;
            }
            else
            {
                FixedPercentageBug = null;
            }
        }

        private void FixPercentageBug()
        {
            Stats stats = MainViewModel.TheSave.Stats;
            stats.TotalProgressInGame = 154;

            FixedPercentageBug = true;
        }

        private void ResetTimers()
        {
            // TODO:
        }

        public ICommand FixPurpleNinesGlitchCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => FixPurpleNinesGlitch()
                );
            }
        }

        public ICommand FixHostilePedsCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => FixHostilePeds()
                );
            }
        }

        public ICommand FixPercentageBugCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => FixPercentageBug()
                );
            }
        }

        public ICommand ResetTimersCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => ResetTimers()
                );
            }
        }
    }

    public enum PadMode
    {
        [Description("Setup 1")]
        Setup1,

        [Description("Setup 2")]
        Setup2,

        [Description("Setup 3")]
        Setup3,

        [Description("Setup 4")]
        Setup4
    }

    public enum InCarCameraMode
    {
        [Description("Bumper")]
        Bumper,

        [Description("Near")]
        Near,

        [Description("Middle")]
        Middle,

        [Description("Far")]
        Far,

        [Description("Overhead")]
        Overhead,

        [Description("Cinematic")]
        Cinematic,
    }

    public enum OnFootCameraMode
    {
        [Description("Near")]
        Near = 1,

        [Description("Middle")]
        Middle,

        [Description("Far")]
        Far,

        [Description("Overhead")]
        Overhead
    }
}
