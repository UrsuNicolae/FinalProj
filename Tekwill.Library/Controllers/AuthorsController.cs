using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase
{

    private readonly ILogger<AuthorsController> _logger;
    private readonly IAuthorRepository authorRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CreateAuthorDto> createAuthorValidator;

    public AuthorsController(ILogger<AuthorsController> logger,
        IAuthorRepository authorRepository, IMapper mapper, IValidator<CreateAuthorDto> createAuthorValidator)
    {
        _logger = logger;
        this.authorRepository = authorRepository;
        this.mapper = mapper;
        this.createAuthorValidator = createAuthorValidator;
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

    [HttpPut]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorDto dto, CancellationToken ct = default)
    {
        var validationResult = await createAuthorValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var createdAuthor = await authorRepository.CreateAuthor(mapper.Map<Author>(dto), ct);
        return CreatedAtRoute("getById", routeValues: new { createdAuthor.Id }, value: mapper.Map<AuthorDto>(createdAuthor));
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Modify(UpdateAuthorDto dto, CancellationToken ct = default)
    {
        var validationResult = await createAuthorValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            await authorRepository.UpdateAuthor(mapper.Map<Author>(dto), ct);
            var updatedGen = await authorRepository.GetAuthor(dto.Id, ct);
            return Ok(mapper.Map<AuthorDto>(updatedGen));
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AuthorDto>> Delete(int id, CancellationToken ct = default)
    {
        try
        {
            await authorRepository.DeleteAuthor(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
