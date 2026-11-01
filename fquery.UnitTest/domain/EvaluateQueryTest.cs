using fquery.adapter;
using fquery.domain;
using fquery.model;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework.Legacy;
using Match = fquery.model.Match;

namespace fquery.UnitTest.domain;

public class EvaluateQueryTest
{
    private ServiceProvider _services;
    private readonly Mock<EvaluateQuery.IPort> _mockedEvaluateQueryIoAdapter = new();

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<EvaluateQuery>();
        serviceCollection.AddSingleton(_mockedEvaluateQueryIoAdapter.Object);

        serviceCollection.BuildServiceProvider();

        _services = serviceCollection.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _services.Dispose();
    }

    [Test]
    public void TestSimpleQuery()
    {
        _mockedEvaluateQueryIoAdapter.Setup(e => e.GetFilesMatchingRegex(It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new FqueryFile("foo")]);

        var ast = new Expression(new Match("foo"));

        var result = GetUnderTest().Execute(ast, ".");
        
        Assert.That(result.Files, Is.EqualTo([new FqueryFile("foo")]));
    }

    private EvaluateQuery GetUnderTest()
    {
        return _services.GetRequiredService<EvaluateQuery>();
    }
}