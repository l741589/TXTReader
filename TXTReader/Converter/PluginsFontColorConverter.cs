using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TXTReader.Plugins;

namespace TXTReader.Converter {
    public class PluginsFontColorConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            switch ((PluginState)value) {
                case PluginState.Fail: return Brushes.Red;
                case PluginState.Loaded: return Brushes.Black;
                case PluginState.NoEnoughNecessaryDependencies: return Brushes.Orange;
                case PluginState.NoEnoughUnnecessaryDependencies: return Brushes.Green;
                case PluginState.NotLoad: return Brushes.Blue;
                case PluginState.Ready: return Brushes.Yellow;
                default: return Brushes.Purple;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}
