using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DaDaPlace.Converters;

public static class BoolConverters
{
    public static readonly EqualityConverter IsEqual = new();
    public static readonly GreaterThanConverter IsGreaterThan = new();
    public static readonly SelectConverter Select = new();
}

public class EqualityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;
        
        return value.ToString() == parameter.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class GreaterThanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;
        
        if (double.TryParse(value.ToString(), out double val) && 
            double.TryParse(parameter.ToString(), out double param))
        {
            return val > param;
        }
        
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter == null)
            return value;
        
        var options = parameter.ToString()?.Split('|');
        if (options == null || options.Length != 2)
            return value;
        
        if (value is bool boolValue)
        {
            return boolValue ? options[0] : options[1];
        }
        
        return options[1]; // Default to false option
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsNotNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}