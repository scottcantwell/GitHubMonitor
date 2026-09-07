using System.Globalization;
using System.Windows.Data;

namespace GitHubMonitor.Converters;

/// <summary>
/// A value converter that inverts a boolean value.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{

    /// <summary>
    /// Converts a boolean value to its inverse.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="t">The target type (not used).</param>
    /// <param name="p">The converter parameter (not used).</param>
    /// <param name="c">The culture information (not used).</param>
    /// <returns>The inverted boolean value.</returns>
    public object Convert(object value, Type t, object p, CultureInfo c) => value is false;

    /// <summary>
    /// Converts a value back to its original form. In this case, it inverts the boolean value again.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="t">The target type (not used).</param>
    /// <param name="p">The converter parameter (not used).</param>
    /// <param name="c">The culture information (not used).</param>
    /// <returns></returns>
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => value is false;
}