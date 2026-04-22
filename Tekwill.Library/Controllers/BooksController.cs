using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.Interfaces;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{

    private readonly ILogger<BooksController> _logger;
    private readonly IBookRepository bookRepository;
    private readonly IMapper mapper;

    public BooksController(ILogger<BooksController> logger,
        IBookRepository bookRepository, IMapper mapper)
    {
        _logger = logger;
        this.bookRepository = bookRepository;
        this.mapper = mapper;
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
}
