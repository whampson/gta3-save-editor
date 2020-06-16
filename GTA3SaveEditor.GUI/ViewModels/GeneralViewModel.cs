using GTASaveData.GTA3;
using System;
using System.ComponentModel;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class GeneralViewModel : TabPageViewModelBase
    {
        public SimpleVariables m_simpleVars;

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

        public GeneralViewModel(MainViewModel mainViewModel)
            : base("General", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            SimpleVars = MainViewModel.TheSave.SimpleVars;

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
