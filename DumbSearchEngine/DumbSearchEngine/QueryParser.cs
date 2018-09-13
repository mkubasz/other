using System.Linq;
using Sprache;

namespace DumbSearchEngine
{
    public class QueryParser
    {
        enum OpCode
        {
            And,
            Or,
            Not
        }

        static Parser<Query> Word => from x in Parse.LetterOrDigit.AtLeastOnce().Text().Token()
                                     select new TermQuery(x);

        static Parser<OpCode> And = Parse.String("AND").Token().Return(OpCode.And);
        static Parser<OpCode> Or = Parse.String("OR").Token().Return(OpCode.Or);
        static Parser<OpCode> Not = Parse.String("NOT").Token().Return(OpCode.Not);

        static Parser<Query> Unary = from not in Not
                                     from expr in Expr
                                     select new NotQuery(expr);

        static Parser<Query> Binary => Parse.ChainOperator(
            And.Or(Or),
            Unary.Or(Word).Or(Parse.Ref(() => Binary)),
            (op, l, r) => op == OpCode.And ? new AndQuery(l, r) as Query : new OrQuery(l, r));

        static Parser<Query> Expr => Unary.Or(Binary);

        public static Query ParseExpression(string text) => Expr.Parse(text);
    }


}
