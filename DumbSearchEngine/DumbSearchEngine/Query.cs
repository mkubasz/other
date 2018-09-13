using System;
using System.Collections.Generic;
using System.Linq;

namespace DumbSearchEngine
{
    public abstract class Query
    {
        public abstract IEnumerable<Node> Do(Index index);
    }

    public class TermQuery : Query
    {
        private string _text;

        public TermQuery(string text)
        {
            _text = text;
        }

        public override IEnumerable<Node> Do(Index index)
        {
            List<Node> nodes;
            return index._index.TryGetValue(_text, out nodes)
                ? nodes.OrderByDescending(n => n.Score)
                : Enumerable.Empty<Node>();
        }
    }

    public abstract class BinaryQuery : Query
    {
        protected Query _left, _right;

        public BinaryQuery(Query left, Query right)
        {
            _left = left;
            _right = right;
        }
    }

    public class AndQuery : BinaryQuery
    {
        public AndQuery(Query left, Query right) : base(left, right) { }

        public override IEnumerable<Node> Do(Index index)
            => from l in _left.Do(index)
               join r in _right.Do(index) on l.Document.Path equals r.Document.Path
               let score = l.Score * r.Score
               orderby score descending
               select new Node { Document = l.Document, Score = score };
    }
    public class OrQuery : BinaryQuery
    {
        public OrQuery(Query left, Query right) : base(left, right) { }

        public override IEnumerable<Node> Do(Index index)
            => _left.Do(index)
            .Concat(_right.Do(index))
            .GroupBy(n => n.Document, n => n.Score)
            .OrderByDescending(g => g.Sum())
            .Select(g => new Node { Document = g.Key, Score = g.Sum() });
    }

    public class NotQuery : Query
    {
        private Query _query;

        public NotQuery(Query query)
        {
            _query = query;
        }

        public override IEnumerable<Node> Do(Index index)
            => new HashSet<DocumentRef>(index._docs.Select(doc => doc.Reference))
                .Except(_query.Do(index).Select(n => n.Document))
                .Select(r => new Node { Document = r, Score = 1 });
    }
}