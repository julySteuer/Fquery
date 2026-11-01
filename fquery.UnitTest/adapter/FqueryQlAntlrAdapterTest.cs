using fquery.adapter;
using fquery.model;
using Range = fquery.model.Range;

namespace fquery.UnitTest.adapter;

public class FqueryQlAntlrAdapterTest
{
    [Test]
    public void TestSimpleString()
    {
        const string inp = "match(\"test\")";
        var fqueryAntlr = new FqueryQlAntlrAdapter().GetAst(inp);
        Assert.That(fqueryAntlr, Is.EqualTo(new Expression(new Match("test"))));
    }

    [Test]
    public void TestOr()
    {
        const string inp = "match(\"test\") OR includes(\"foo\", > 3)";
        var fqueryAntlr = new FqueryQlAntlrAdapter().GetAst(inp);
        Assert.That(fqueryAntlr, Is.EqualTo(new OrExpression(new Includes("foo", new RangeDescriptor(Range.GreaterThan, 3)), new Expression(new Match("test")))));
    }
}