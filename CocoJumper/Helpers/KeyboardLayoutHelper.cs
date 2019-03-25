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

        public static IEnumerable<char> GetKeysNotNull(char key)
        {
            return GetKeys(key).Where(x => x.HasValue).Select(x => x.Value);
        }

        private static char? GetCharByXY(int x, int y)
        {
            if (x >= Layouts.Length || x < 0)
                return null;
            string layout = Layouts[x];
            if (y >= layout.Length || y < 0)
                return null;
            return layout[y];
        }

        private static IEnumerable<char?> GetKeys(char key)
        {
            foreach (string layout in Layouts)
            {
                for (int j = 0; j < layout.Length; j++)
                {
                    if (layout[j] != key) continue;

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
}