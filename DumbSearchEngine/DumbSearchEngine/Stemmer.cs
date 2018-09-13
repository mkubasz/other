using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSearchEngine
{
    public class Stemmer
    {
        public static IEnumerable<Stem> Stem(string word)
        {
            int len = word.Length;
            for (var i = 1; i <= len; i++) yield return new Stem { Word = word.Substring(0, i), Score = (double)i / len };
            for (var i = 1; i < len; i++) yield return new Stem { Word = word.Substring(i, len - i), Score = (double)(len - i) / len };
        }
    }
}
