﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.GUI.Extensions;
using GTASaveData.GTA3;
using Xceed.Wpf.Toolkit;

namespace GTA3SaveEditor.GUI.Tabs
{
    public class GaragesVM : TabPageVM
    {
        public static ObservableCollection<ColorItem> AvailableCarColors =>
            new ObservableCollection<ColorItem>(
                GTA3.CarColors.Select(x => new ColorItem(x.Color.ToMediaColor(), x.Name)));

        private ObservableCollection<StoredCar> m_safehouseCars;
        private ObservableCollection<Garage> m_garages;
        private Level m_selectedSafehouse;
        private StoredCar m_selectedCar;
        private Garage m_selectedGarage;

        public ObservableCollection<StoredCar> StoredCars
        {
            get { return m_safehouseCars; }
            set { m_safehouseCars = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Garage> Garages
        {
            get { return m_garages; }
            set { m_garages = value; OnPropertyChanged(); }
        }

        public Level SelectedSafehouse
        {
            get { return m_selectedSafehouse; }
            set { m_selectedSafehouse = value; OnPropertyChanged(); }
        }

        public StoredCar SelectedCar
        {
            get { return m_selectedCar; }
            set { m_selectedCar = value; OnPropertyChanged(); }
        }

        public Garage SelectedGarage
        {
            get { return m_selectedGarage; }
            set { m_selectedGarage = value; OnPropertyChanged(); }
        }

        public GaragesVM()
        {
            StoredCars = new ObservableCollection<StoredCar>();
            Garages = new ObservableCollection<Garage>();
        }

        public override void Load()
        {
            base.Load();

            if (SelectedSafehouse == Level.None)
            {
                SelectedSafehouse = Level.Industrial;
            }

            SelectedCar = null;
            SelectedGarage = null;

            UpdateStoredCarList();
            UpdateGarageList();
        }

        public void UpdateStoredCarList()
        {
            switch (SelectedSafehouse)
            {
                case Level.Industrial:
                    StoredCars = new ObservableCollection<StoredCar>(TheSave.Garages.CarsInSafeHouse1);
                    break;
                case Level.Commercial:
                    StoredCars = new ObservableCollection<StoredCar>(TheSave.Garages.CarsInSafeHouse2);
                    break;
                case Level.Suburban:
                    StoredCars = new ObservableCollection<StoredCar>(TheSave.Garages.CarsInSafeHouse3);
                    break;
            }
        }

        public void UpdateGarageList()
        {
            Garages = new ObservableCollection<Garage>(TheSave.Garages.Garages);
        }
    }
}
