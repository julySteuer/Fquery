using fquery.model;

namespace fquery.domain;

public class ParseQuery(ParseQuery.IPort port)
{
    public IQueryAst Execute(string query)
    {
        return port.GetAst(query);
    }
    
    public interface IPort
    {
        IQueryAst GetAst(string query);
    }
}