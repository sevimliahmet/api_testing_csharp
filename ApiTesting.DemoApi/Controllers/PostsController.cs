using Microsoft.AspNetCore.Mvc;

namespace ApiTesting.DemoApi.Controllers;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetPost(int id)
    {
        return Ok(new
        {
            id = id,
            title = "demo title",
            body = "demo body",
            userId = 1
        });
    }

    [HttpPost]
    public IActionResult CreatePost([FromBody] PostRequest request)
    {
        return Created("/posts/1", new
        {
            id = 1,
            title = request.Title,
            body = request.Body,
            userId = request.UserId
        });
    }
}

public class PostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
}
