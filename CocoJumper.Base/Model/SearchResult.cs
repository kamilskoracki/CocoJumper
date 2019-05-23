namespace CocoJumper.Base.Model
{
    public class SearchResult
    {
        private string _key;
        public int Length;
        public int Position;

        public string Key { get => _key; set => _key = value.ToLower(); }
    }
}