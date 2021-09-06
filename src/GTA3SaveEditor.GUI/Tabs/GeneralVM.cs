using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using GTA3SaveEditor.Core.Extensions;
using GTA3SaveEditor.Core.Game;
using GTASaveData;
using GTASaveData.GTA3;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Tabs
{
    public class GeneralVM : TabPageVM
    {
        private SimpleVariables m_simpleVars;
        private FileFormat m_platform;
        private bool m_isPS2;
        private bool m_canNameBeEdited;
        private bool m_canNameBeGxtKey;
        private bool m_isNameGxtKey;

        public bool UsingForcedWeather => SimpleVars.ForcedWeatherType != WeatherType.None;
        public bool SuppressNameUpdateOnce { get; private set; }

        public SimpleVariables SimpleVars
        {
            get { return m_simpleVars; }
            set { m_simpleVars = value; OnPropertyChanged(); }
        }

        public FileFormat Platform
        {
            get { return m_platform; }
            set { m_platform = value; OnPropertyChanged(); }
        }

        public bool IsPS2
        {
            get { return m_isPS2; }
            set { m_isPS2 = value; OnPropertyChanged(); }
        }

        public bool CanEditNameDirectly
        {
            get { return m_canNameBeEdited; }
            set { m_canNameBeEdited = value; OnPropertyChanged(); }
        }

        public bool CanNameBeGxtKey
        {
            get { return m_canNameBeGxtKey; }
            set { m_canNameBeGxtKey = value; OnPropertyChanged(); }
        }

        public bool IsNameGxtKey
        {
            get { return m_isNameGxtKey; }
            set { m_isNameGxtKey = value; OnPropertyChanged(); }
        }

        public WeatherType ForcedWeather
        {
            get { return SimpleVars.ForcedWeatherType; }
            set { SimpleVars.ForcedWeatherType = value; OnPropertyChanged(); OnPropertyChanged(nameof(UsingForcedWeather)); }
        }

        public CamZoom PedCam
        {
            get { return (CamZoom) (int) Math.Round(SimpleVars.CameraModeOnFoot); }
            set { SimpleVars.CameraModeOnFoot = (float) value; OnPropertyChanged(); }
        }

        public CamZoom CarCam
        {
            get { return (CamZoom) (int) Math.Round(SimpleVars.CameraModeInCar); }
            set { SimpleVars.CameraModeInCar = (float) value; OnPropertyChanged(); }
        }

        public AudioOutputType AudioOutput
        {
            get { return (SimpleVars.StereoMono) ? AudioOutputType.Mono : AudioOutputType.Stereo; }
            set { SimpleVars.StereoMono = (value == AudioOutputType.Mono); OnPropertyChanged(); }
        }

        public DateTime GameClock
        {
            get
            {
                int hh = SimpleVars.GameClockHours;
                int mm = SimpleVars.GameClockMinutes;
                return new DateTime(1, 1, 1, hh, mm, 0);
            }

            set
            {
                SimpleVars.GameClockHours = (byte) value.Hour;
                SimpleVars.GameClockMinutes = (byte) value.Minute;
                OnPropertyChanged();
            }
        }

        public override void Load()
        {
            base.Load();

            SimpleVars = TheSave.SimpleVars;
            SimpleVars.PropertyChanged += SimpleVars_PropertyChanged;
        }

        public override void Unload()
        {
            base.Unload();

            SimpleVars.PropertyChanged -= SimpleVars_PropertyChanged;
            SimpleVars = null;
        }

        public override void Update()
        {
            base.Update();
            SuppressNameUpdateOnce = true;

            Platform = TheSave.FileFormat;
            IsPS2 = TheSave.IsPS2;
            CanNameBeGxtKey = !IsPS2 && TheSave.IsMobile;
            IsNameGxtKey = TheSave.IsNameGxtKey();
            UpdateNameTextBoxVisibility();

            OnPropertyChanged(nameof(GameClock));
            OnPropertyChanged(nameof(UsingForcedWeather));
        }

        public void UpdateNameTextBoxVisibility()
        {
            CanEditNameDirectly = !IsPS2 && !TheSave.IsNameGxtKey();
        }

        public void UpdateName(bool isGxtKey)
        {
            if (SuppressNameUpdateOnce)
            {
                SuppressNameUpdateOnce = false;
                return;
            }

            string oldName = TheSave.GetNameRaw();
            TheSave.SetName(oldName, isGxtKey);
        }

        private void SimpleVars_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SimpleVars.LastMissionPassedName):
                {
                    string name = SimpleVars.LastMissionPassedName;
                    IsNameGxtKey = name != null && name.StartsWith('\uFFFF');
                    break;
                }
                default:
                    break;
            }
        }

        public ICommand FixPurpleNines => new RelayCommand
        (
            () =>
            {
                bool wasFixed = TheSave.FixPurpleNinesGlitch();
                TheWindow.SetTimedStatusText((wasFixed)
                    ? "Fixed Purple Nines glitch."
                    : "Purple Nines Glitch not present.");
            }

        );
    }
}
