namespace CocoJumper.Base.Model
{
    public class LineData
    {
        private string data;
        public int DataLength;
        public int Start;

        public string Data { get => data; set => data = value.ToLower(); }
    }
}