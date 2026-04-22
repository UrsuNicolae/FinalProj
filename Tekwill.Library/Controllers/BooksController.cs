using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{

    private readonly ILogger<BooksController> _logger;
    private readonly IBookRepository bookRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CreateBookDto> createBookValidator;

    public BooksController(ILogger<BooksController> logger,
        IBookRepository bookRepository, IMapper mapper, IValidator<CreateBookDto> createBookValidator)
    {
        _logger = logger;
        this.bookRepository = bookRepository;
        this.mapper = mapper;
        this.createBookValidator = createBookValidator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<BookDto>>> Get(int page, int pageSize, CancellationToken ct = default)
    {
        var books = await bookRepository.GetBooks(page, pageSize, ct);

        return Ok(new PaginatedList<BookDto>(mapper.Map<List<BookDto>>(books.Items),
            page,
            books.TotalPages));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> Get(int id, CancellationToken ct = default)
    {
        try
        {
            var book = await bookRepository.GetBook(id, ct);
            return Ok(mapper.Map<BookDto>(book));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut]
    public async Task<ActionResult<BookDto>> Create(CreateBookDto dto, CancellationToken ct = default)
    {
        var validationResult = await createBookValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var createdBook = await bookRepository.CreateBook(mapper.Map<Book>(dto), ct);
        return CreatedAtRoute("getById", routeValues: new { createdBook.Id }, value: mapper.Map<BookDto>(createdBook));
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Modify(BookDto dto, CancellationToken ct = default)
    {
        var validationResult = await createBookValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            await bookRepository.UpdateBook(mapper.Map<Book>(dto), ct);
            var updatedGen = await bookRepository.GetBook(dto.Id, ct);
            return Ok(mapper.Map<BookDto>(updatedGen));
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BookDto>> Delete(int id, CancellationToken ct = default)
    {
        try
        {
            await bookRepository.DeleteBook(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
