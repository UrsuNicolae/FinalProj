using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Gens;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GensController : ControllerBase
{

    private readonly ILogger<GensController> _logger;
    private readonly IGenRepository genRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CreateGenDto> createGenValidator;

    public GensController(ILogger<GensController> logger,
        IGenRepository genRepository, IMapper mapper,
        IValidator<CreateGenDto> createGenValidator)
    {
        _logger = logger;
        this.genRepository = genRepository;
        this.mapper = mapper;
        this.createGenValidator = createGenValidator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<GenDto>>> Get(int page, int pageSize, CancellationToken ct = default)
    {
        var gens = await genRepository.GetGens(page, pageSize, ct);

        return Ok(new PaginatedList<GenDto>(mapper.Map<List<GenDto>>(gens.Items),
            page,
            gens.TotalPages));
    }

    [HttpGet("{id}", Name = "getById")]
    public async Task<ActionResult<GenDto>> Get(int id, CancellationToken ct = default)
    {
        try
        {
            var gen = await genRepository.GetGen(id, ct);
            return Ok(mapper.Map<GenDto>(gen));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut]
    public async Task<ActionResult<GenDto>> Create(CreateGenDto dto, CancellationToken ct = default)
    {
        var validationResult = await createGenValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var createdGen = await genRepository.CreateGen(mapper.Map<Gen>(dto), ct);
        return CreatedAtRoute("getById", routeValues: new { createdGen.Id }, value: mapper.Map<GenDto>(createdGen));
    }

    [HttpPost]
    public async Task<ActionResult<GenDto>> Modify(GenDto dto, CancellationToken ct = default)
    {
        var validationResult = await createGenValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            await genRepository.UpdateGen(mapper.Map<Gen>(dto), ct);
            var updatedGen = await genRepository.GetGen(dto.Id, ct);
            return Ok(mapper.Map<GenDto>(updatedGen));
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<GenDto>> Delete(int id, CancellationToken ct = default)
    {
        try
        {
            await genRepository.DeleteGen(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
