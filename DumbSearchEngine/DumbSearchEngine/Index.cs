using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSearchEngine
{
    public class Index
    {
        private static readonly char[] Separators = new char[] { ' ', ',', '/', '|', ':', ';', '.', '?', '!', '-', '+', '=', '@', '#', '\n', '\r' };

        internal readonly Dictionary<string, List<Node>> _index = new Dictionary<string, List<Node>>();
        internal readonly HashSet<Document> _docs = new HashSet<Document>();

        public void AddDocument(Document doc)
        {
            _docs.Add(doc);

            var stems = doc.Content
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(Stemmer.Stem)
                .GroupBy(s => s.Word, s => s.Score)
                .Select(s => Tuple.Create(s.Key, s.Sum()));
            foreach(var stem in stems)
            {
                List<Node> nodes;
                var node = new Node { Document = doc.Reference, Score = stem.Item2 };
                if (_index.TryGetValue(stem.Item1, out nodes))
                {
                    nodes.Add(node);
                }
                else
                {
                    nodes = new List<Node> { node };
                    _index.Add(stem.Item1, nodes);
                }
            }
        }

        public IEnumerable<Node> Find(string query) => QueryParser.ParseExpression(query).Do(this);

    }
}
