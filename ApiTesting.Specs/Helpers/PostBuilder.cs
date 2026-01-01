using System.Text.Json;

namespace ApiTesting.Specs.Helpers;

public class PostBuilder
{
    private string _title = "Test Title";
    private string _body = "Test Body";
    private int _userId = 1;

    public PostBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public PostBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public PostBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public string BuildJson()
    {
        var obj = new
        {
            title = _title,
            body = _body,
            userId = _userId
        };
        
        return JsonSerializer.Serialize(obj);
    }

    public static PostBuilder New() => new();
}
