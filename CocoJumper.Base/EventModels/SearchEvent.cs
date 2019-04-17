namespace CocoJumper.Base.Events
{
    public class SearchEvent
    {
        public int Length;
        public int StartPosition;
        public string Letters;

        //public int Hash
        //{
        //    get
        //    {
        //        int result = Letters?.GetHashCode() ?? 0;
        //        result = (result * 397) ^ Length;
        //        result = (result * 397) ^ StartPosition;
        //        return result;
        //    }
        //}
    }
}