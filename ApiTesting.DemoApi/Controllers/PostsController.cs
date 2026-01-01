using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetPost(int id)
    {
        if (id <= 0)
            return BadRequest();

        if (id != 1)
            return NotFound();

        return Ok(new
        {
            id = 1,
            title = "demo title",
            body = "demo body",
            userId = 1
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] PostRequest request)
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
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public int UserId { get; set; }
}
