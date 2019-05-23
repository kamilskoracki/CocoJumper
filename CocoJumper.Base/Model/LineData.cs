namespace CocoJumper.Base.Model
{
    public class LineData
    {
        private string _data;
        public int DataLength;
        public int Start;

        public string Data { get => _data; set => _data = value.ToLower(); }
    }
}