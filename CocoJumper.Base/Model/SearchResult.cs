namespace CocoJumper.Base.Model
{
    public class SearchResult
    {
        private string key;
        public int Length;
        public int Position;

        public string Key { get => key; set => key = value.ToLower(); }
    }
}