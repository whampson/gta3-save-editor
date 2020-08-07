using GTA3SaveEditor.GUI.Events;
using GTASaveData.GTA3;
using System;
using System.ComponentModel;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class General : BaseTabPage
    {
        private bool m_isLoading;
        private SimpleVariables m_simpleVars;
        private string m_saveTitle;
        private string m_saveTitleGxtKey;
        private bool m_isSaveTitleFromGxt;
        private bool? m_fixedPurpleNinesGlitch;
        private bool? m_fixedHostilePeds;
        private bool? m_fixedPercentageBug;

        public bool IsMobile
        {
            get { return MainViewModel.TheSave.FileFormat.IsMobile; }
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

        public string SaveTitle
        {
            get { return m_saveTitle; }
            set
            {
                if (!m_isLoading && !IsSaveTitleFromGxt)
                {
                    value = SetLastMissionPassedNameString(value);
                }
                m_saveTitle = value;
                OnPropertyChanged();
            }
        } 

        public string SaveTitleGxtKey
        {
            get { return m_saveTitleGxtKey; }
            set
            {
                if (!m_isLoading && IsSaveTitleFromGxt)
                {
                    SaveTitle = SetLastMissionPassedNameGxt(value);
                }
                m_saveTitleGxtKey = value;
                OnPropertyChanged();
            }
        }

        public bool IsSaveTitleFromGxt
        {
            get { return m_isSaveTitleFromGxt; }
            set { m_isSaveTitleFromGxt = value; OnPropertyChanged(); }
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
            get { return (OnFootCameraMode) SimpleVars.CameraModeOnFoot; }
            set { SimpleVars.CameraModeOnFoot = (float) value; OnPropertyChanged(); }
        }

        public InCarCameraMode InCarCameraMode
        {
            get { return (InCarCameraMode) SimpleVars.CameraModeInCar; }
            set { SimpleVars.CameraModeInCar = (float) value; OnPropertyChanged(); }
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

        public General(Main mainViewModel)
            : base("General", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        public override void Load()
        {
            m_isLoading = true;
            base.Load();
            
            SimpleVars = MainViewModel.TheSave.SimpleVars;

            InitSaveTitle();
            DetectPurpleNinesGlitch();
            DetectHostilePeds();
            DetectPercentageBug();

            OnPropertyChanged(nameof(IsMobile));
            OnPropertyChanged(nameof(IsPS2));
            OnPropertyChanged(nameof(GameClock));
            OnPropertyChanged(nameof(CurrentPadMode));
            OnPropertyChanged(nameof(OnFootCameraMode));
            OnPropertyChanged(nameof(InCarCameraMode));

            m_isLoading = false;
        }

        private void InitSaveTitle()
        {
            string name = SimpleVars.LastMissionPassedName;
            if (!string.IsNullOrEmpty(name) && name[0] == '\uFFFF')
            {
                string key = name.Substring(1);
                if (MainViewModel.TheText.TryGetValue(key, out string title))
                {
                    IsSaveTitleFromGxt = true;
                    SaveTitleGxtKey = key;
                    SaveTitle = title;
                    return;
                }
            }

            IsSaveTitleFromGxt = false;
            SaveTitle = name;
        }

        public string SetLastMissionPassedNameString(string value)
        {
            const int MaxLength = SimpleVariables.MaxMissionPassedNameLength - 1;

            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length > MaxLength) value = value.Substring(0, MaxLength);

            SimpleVars.LastMissionPassedName = value;
            return value;
        }

        public string SetLastMissionPassedNameGxt(string key)
        {
            if (key == null) return null;

            if (MainViewModel.TheText.TryGetValue(key, out string name))
            {
                SimpleVars.LastMissionPassedName = '\uFFFF' + key;
            }

            return name;
        }

        private void DetectPurpleNinesGlitch()
        {
            // TODO: ensure mission not yet completed
            Gang hoods = MainViewModel.TheSave.Gangs[GangType.Hoods];

            FixedPurpleNinesGlitch = (hoods.PedModelOverride != -1)
                ? (bool?) false : null;
        }

        private void FixPurpleNinesGlitch()
        {
            Gang hoods = MainViewModel.TheSave.Gangs[GangType.Hoods];
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

        public void GxtSelectionDialog_Callback(bool? result, GxtSelectionEventArgs e)
        {
            if (result == true)
            {
                SaveTitleGxtKey = e.SelectedKey;
            }
        }

        public ICommand SelectGxtKeyCommand => new RelayCommand<Action<bool?, GxtSelectionEventArgs>>
        (
            (_) => MainViewModel.ShowGxtSelectionDialog(GxtSelectionDialog_Callback),
            (_) => IsSaveTitleFromGxt
        );

        public ICommand FixPurpleNinesGlitchCommand => new RelayCommand
        (
            () => FixPurpleNinesGlitch()
        );

        public ICommand FixHostilePedsCommand => new RelayCommand
        (
            () => FixHostilePeds()
        );

        public ICommand FixPercentageBugCommand => new RelayCommand
        (
            () => FixPercentageBug()
        );

        public ICommand ResetTimersCommand => new RelayCommand
        (
            () => ResetTimers()
        );
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
