using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.Interfaces;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase
{

    private readonly ILogger<AuthorsController> _logger;
    private readonly IAuthorRepository authorRepository;
    private readonly IMapper mapper;

    public AuthorsController(ILogger<AuthorsController> logger,
        IAuthorRepository authorRepository, IMapper mapper)
    {
        _logger = logger;
        this.authorRepository = authorRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<AuthorDto>>> Get(int page, int pageSize, CancellationToken ct = default)
    {
        var authors = await authorRepository.GetAuthors(page, pageSize, ct);

        return Ok(new PaginatedList<AuthorDto>(mapper.Map<List<AuthorDto>>(authors.Items),
            page,
            authors.TotalPages));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> Get(int id, CancellationToken ct = default)
    {
        try
        {
            var author = await authorRepository.GetAuthor(id, ct);
            return Ok(mapper.Map<AuthorDto>(author));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
