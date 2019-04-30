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

        public static IEnumerable<string> GetKeys(char key)
        {
            string keyToAdd = "";
            while (true)
            {
                foreach (char? p in GetAllKeys(key).Where(p => p.HasValue && p != 'z'))
                    yield return keyToAdd + p;

                keyToAdd += "z";
            }
        }

        private static IEnumerable<char?> GetAllKeys(char key)
        {
            (int i, int j) inxs = GetIndexsOfKey(key) ??
                                  GetIndexsOfKey('f') ??
                                  throw new System.Exception($"{nameof(GetKeys)} for {key} returned null");

            yield return GetCharByXy(0, inxs.j);
            yield return GetCharByXy(1, inxs.j);
            yield return GetCharByXy(2, inxs.j);
            for (int x = 1; x < 10; x++)
            {
                yield return GetCharByXy(0, inxs.j + x);
                yield return GetCharByXy(1, inxs.j + x);
                yield return GetCharByXy(2, inxs.j + x);
                yield return GetCharByXy(0, inxs.j - x);
                yield return GetCharByXy(1, inxs.j - x);
                yield return GetCharByXy(2, inxs.j - x);
            }
        }

        public static IEnumerable<string> GetKeysNotNull(char key)
        {
            return GetKeys(key).Where(p => !string.IsNullOrEmpty(p));
        }

        private static char? GetCharByXy(int x, int y)
        {
            if (x >= Layouts.Length || x < 0)
                return null;
            string curr = Layouts[x];
            if (y >= curr.Length || y < 0)
                return null;
            return curr[y];
        }

        private static (int i, int j)? GetIndexsOfKey(char key)
        {
            for (int i = 0; i < Layouts.Length; i++)
            {
                string curr = Layouts[i];
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