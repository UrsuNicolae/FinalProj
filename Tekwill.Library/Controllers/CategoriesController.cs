using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{

    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CreateCategoryDto> createCategoryValidator;

    public CategoriesController(ILogger<CategoriesController> logger,
        ICategoryRepository categoryRepository, IMapper mapper, IValidator<CreateCategoryDto> createCategoryValidator)
    {
        _logger = logger;
        this.categoryRepository = categoryRepository;
        this.mapper = mapper;
        this.createCategoryValidator = createCategoryValidator;
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

    [HttpPut]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var validationResult = await createCategoryValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var createdCategory = await categoryRepository.CreateCategory(mapper.Map<Category>(dto), ct);
        return CreatedAtRoute("getById", routeValues: new { createdCategory.Id }, value: mapper.Map<CategoryDto>(createdCategory));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Modify(CategoryDto dto, CancellationToken ct = default)
    {
        var validationResult = await createCategoryValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            await categoryRepository.UpdateCategory(mapper.Map<Category>(dto), ct);
            var updatedGen = await categoryRepository.GetCategory(dto.Id, ct);
            return Ok(mapper.Map<CategoryDto>(updatedGen));
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CategoryDto>> Delete(int id, CancellationToken ct = default)
    {
        try
        {
            await categoryRepository.DeleteCategory(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
