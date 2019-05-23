using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CocoJumper.Models
{
    public class MarkerViewModel : INotifyPropertyChanged
    {
        internal const int FontSizeFactor = -4;
        internal const int HeightFactor = -2;
        private double _markerFontSize;

        private double _markerHeight;

        private string _markerText;

        private int _matchNumber;

        public event PropertyChangedEventHandler PropertyChanged;

        public double MarkerFontSize
        {
            get { return _markerFontSize; }
            set { _markerFontSize = value; OnPropertyChanged("MarkerFontSize"); }
        }

        public double MarkerHeight
        {
            get { return _markerHeight; }
            set { _markerHeight = value; OnPropertyChanged("MarkerHeight"); }
        }

        public string MarkerText
        {
            get { return _markerText; }
            set { _markerText = value; OnPropertyChanged("MarkerText"); }
        }

        public int MatchNumber
        {
            get { return _matchNumber; }
            set { _matchNumber = value; OnPropertyChanged("MatchNumber"); }
        }

        public void Update(string text, double lineHeight, int? matchNumber)
        {
            MarkerText = text;
            MarkerFontSize = lineHeight + FontSizeFactor;
            MarkerHeight = lineHeight + HeightFactor;

            if (matchNumber == null) return;
            MatchNumber = matchNumber.Value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}