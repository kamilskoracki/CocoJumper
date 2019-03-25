using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.Helpers
{
    public static class KeyboardLayoutHelper
    {
        //TODO - add more layouts?
        private static string[] layout = {
            "qwertyuiop",
            "asdfghjkl",
            "zxcvbnm"
        };

        public static IEnumerable<char> GetKeysNotNull(char key)
        {
            return GetKeys(key).Where(x => x.HasValue).Select(x => x.Value);
        }

        public static IEnumerable<char?> GetKeys(char key)
        {
            for (int i = 0; i < layout.Length; i++)
            {
                var curr = layout[i];
                for (int j = 0; j < curr.Length; j++)
                {
                    if (curr[j] == key)
                    {
                        yield return GetCharByXY(0, j);
                        yield return GetCharByXY(1, j);
                        yield return GetCharByXY(2, j);
                        for (int x = 1; x < 10; x++)
                        {
                            yield return GetCharByXY(0, j + x);
                            yield return GetCharByXY(1, j + x);
                            yield return GetCharByXY(2, j + x);
                            yield return GetCharByXY(0, j - x);
                            yield return GetCharByXY(1, j - x);
                            yield return GetCharByXY(2, j - x);
                        }
                    }
                }
            }
        }

        private static char? GetCharByXY(int x, int y)
        {
            if (x >= layout.Length || x < 0)
                return null;
            var curr = layout[x];
            if (y >= curr.Length || y < 0)
                return null;
            return curr[y];
        }
    }
}