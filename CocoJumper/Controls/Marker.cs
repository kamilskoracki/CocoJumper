using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CocoJumper.Controls
{
    public class Marker : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty MarkerTextProperty =
            DependencyProperty.Register("MarkerText", typeof(string),
                typeof(Marker), new PropertyMetadata(MarkerTextChanged));

        public event PropertyChangedEventHandler PropertyChanged;

        public string MarkerText
        {
            get { return (string)GetValue(MarkerTextProperty); }
            set { SetValue(MarkerTextProperty, value); }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void MarkerTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Marker c = sender as Marker;
            c?.OnPropertyChanged(nameof(c.MarkerText));
        }
    }
}