using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Application.Interfaces;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{

    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMapper mapper;

    public CategoriesController(ILogger<CategoriesController> logger,
        ICategoryRepository categoryRepository, IMapper mapper)
    {
        _logger = logger;
        this.categoryRepository = categoryRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CategoryDto>>> Get(int page, int pageSize, CancellationToken ct = default)
    {
        var categories = await categoryRepository.GetCategories(page, pageSize, ct);

        return Ok(new PaginatedList<CategoryDto>(mapper.Map<List<CategoryDto>>(categories.Items),
            page,
            categories.TotalPages));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> Get(int id, CancellationToken ct = default)
    {
        try
        {
            var category = await categoryRepository.GetCategory(id, ct);
            return Ok(mapper.Map<CategoryDto>(category));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
