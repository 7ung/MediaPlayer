using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MediaPlayer.Model
{
    //chuyển từ timespan to string để bind
    public class MyConverter : IValueConverter
    {
        // Summary:
        //     Modifies the source data before passing it to the target for display in the
        //     UI.
        //
        // Parameters:
        //   value:
        //     The source data being passed to the target.
        //
        //   targetType:
        //     The type of the target property, as a type reference (System.Type for Microsoft
        //     .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).
        //
        //   parameter:
        //     An optional parameter to be used in the converter logic.
        //
        //   language:
        //     The language of the conversion.
        //
        // Returns:
        //     The value to be passed to the target dependency property.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timespan = (TimeSpan)value;
            if (timespan.Hours > 0)
                return timespan.ToString(@"hh\:mm\:ss");
            else
                return timespan.ToString(@"mm\:ss");

        }
        //
        // Summary:
        //     Modifies the target data before passing it to the source object. This method
        //     is called only in TwoWay bindings.
        //
        // Parameters:
        //   value:
        //     The target data being passed to the source.
        //
        //   targetType:
        //     The type of the target property, as a type reference (System.Type for Microsoft
        //     .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).
        //
        //   parameter:
        //     An optional parameter to be used in the converter logic.
        //
        //   language:
        //     The language of the conversion.
        //
        // Returns:
        //     The value to be passed to the source object.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class ConverterTimeSpanToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timespan = (TimeSpan)value;

            if (parameter.ToString().CompareTo("Milliseconds") == 0)
            {
                return timespan.TotalMilliseconds;
            }
            else
            {
                return timespan.TotalSeconds;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var time = (double)value;

            if (parameter.ToString().CompareTo("Milliseconds") == 0)
            {
                return new TimeSpan(0, 0, 0, (int)time);
            }
            else
            {
                return new TimeSpan(0, 0, (int)time);
            }
        }
    }

    public class ValueTimelineConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = (double)value;
            var bar = parameter as ProgressBar;

            if (bar == null)
                return 0;

            var percent = time / bar.Maximum;

            return bar.Width * percent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var pos = (double)value;
            var bar = parameter as ProgressBar;
            if (bar == null)
                return 0;

            var percent = pos / bar.Width;

            return bar.Maximum * percent;
        }
    }
}
