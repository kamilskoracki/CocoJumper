using System.Windows;
using System.Windows.Controls;

namespace CocoJumper.Controls
{
    public class Marker : ContentControl
    {
        public static readonly DependencyProperty MarkerTextProperty =
            DependencyProperty.Register("MarkerText", typeof(string),
                typeof(Marker), new PropertyMetadata(MarkerTextChanged));

        public string MarkerText
        {
            get { return (string)GetValue(MarkerTextProperty); }
            set { SetValue(MarkerTextProperty, value); }
        }

        private static void MarkerTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Marker)d).MarkerText = e.NewValue as string;
        }

        //public static readonly DependencyProperty MarkerTextProperty = DependencyProperty.RegisterAttached(
        //    "MarkerText", 
        //    typeof(string), 
        //    typeof(Marker), 
        //    new PropertyMetadata(default(string)));

        //public static string GetMarkerText(UIElement element)
        //{
        //    return (string) element.GetValue(MarkerTextProperty);
        //}

        //public static void SetMarkerText(UIElement element, string value)
        //{
        //    element.SetValue(MarkerTextProperty, value);
        //}
    }
}