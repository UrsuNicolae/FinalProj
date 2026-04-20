using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Gens;
using Tekwill.Library.Application.Interfaces;

namespace Tekwill.Library.Controllers;

[ApiController]
[Route("[controller]")]
public class GensController : ControllerBase
{

    private readonly ILogger<GensController> _logger;
    private readonly IGenRepository genRepository;
    private readonly IMapper mapper;

    public GensController(ILogger<GensController> logger,
        IGenRepository genRepository, IMapper mapper)
    {
        _logger = logger;
        this.genRepository = genRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<GenDto>>> Get(int page, int pageSize, CancellationToken ct = default)
    {
        var gens = await genRepository.GetGens(page, pageSize, ct);

        return new PaginatedList<GenDto>(mapper.Map<List<GenDto>>(gens.Items),
            page,
            gens.TotalPages);
    }
}
