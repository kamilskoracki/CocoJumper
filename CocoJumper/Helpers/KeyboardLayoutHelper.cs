using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.Helpers
{
    public static class KeyboardLayoutHelper
    {
        //TODO - add more layouts?
        private static readonly string[] Layouts = {
            "qwertyuiop",
            "asdfghjkl",
            "zxcvbnm"
        };

        public static IEnumerable<char?> GetKeys(char key)
        {
            var inxs = GetIndexsOfKey(key) ?? GetIndexsOfKey('f') ?? throw new System.Exception($"{nameof(GetKeys)} for {key} returned null");

            yield return GetCharByXY(0, inxs.j);
            yield return GetCharByXY(1, inxs.j);
            yield return GetCharByXY(2, inxs.j);
            for (int x = 1; x < 10; x++)
            {
                yield return GetCharByXY(0, inxs.j + x);
                yield return GetCharByXY(1, inxs.j + x);
                yield return GetCharByXY(2, inxs.j + x);
                yield return GetCharByXY(0, inxs.j - x);
                yield return GetCharByXY(1, inxs.j - x);
                yield return GetCharByXY(2, inxs.j - x);
            }
        }

        public static IEnumerable<char> GetKeysNotNull(char key)
        {
            return GetKeys(key).Where(x => x.HasValue).Select(x => x.Value);
        }

        private static char? GetCharByXY(int x, int y)
        {
            if (x >= Layouts.Length || x < 0)
                return null;
            var curr = Layouts[x];
            if (y >= curr.Length || y < 0)
                return null;
            return curr[y];
        }

        private static (int i, int j)? GetIndexsOfKey(char key)
        {
            for (int i = 0; i < Layouts.Length; i++)
            {
                var curr = Layouts[i];
                for (int j = 0; j < curr.Length; j++)
                {
                    if (curr[j] == key)
                    {
                        return (i, j);
                    }
                }
            }
            return null;
        }
    }
}