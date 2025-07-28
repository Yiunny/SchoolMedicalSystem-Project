using SchoolMedicalSystem.Core.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SchoolMedicalSystem.WPF.Converters
{
    public class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole userRole && parameter is UserRole requiredRole)
            {
                return userRole == requiredRole ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}