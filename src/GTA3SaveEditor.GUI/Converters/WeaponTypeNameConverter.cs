using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Converters
{
    [ValueConversion(typeof(WeaponType), typeof(string))]
    public class WeaponTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeaponType || value is int)
            {
                return (WeaponType) value switch
                {
                    WeaponType.None => "(none)",
                    WeaponType.BaseballBat => "Baseball Bat",
                    WeaponType.Colt45 => "Colt 45",
                    WeaponType.Uzi => "Uzi",
                    WeaponType.Shotgun => "Shotgun",
                    WeaponType.AK47 => "AK-47",
                    WeaponType.M16 => "M16",
                    WeaponType.SniperRifle => "Sniper Rifle",
                    WeaponType.RocketLauncher => "Rocket Launcher",
                    WeaponType.Flamethrower => "Flamethrower",
                    WeaponType.Molotov => "Molotov Cocktail",
                    WeaponType.Grenade => "Grenade",
                    WeaponType.Detonator => "Detonator",
                    _ => DependencyProperty.UnsetValue
                };
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter.");
        }
    }
}
