using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

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
            if (timespan.Hours > 0 )
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
}
