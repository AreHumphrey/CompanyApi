using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Models;

namespace CompanyApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CompanyController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCompany(CreateCompanyDto companyDto)
    {
        var company = new Company
        {
            Name = companyDto.Name,
            Country = companyDto.Country,
            BigBoss = companyDto.BigBoss
        };

        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        return Ok(company);
    }

    [HttpGet("get/{companyId}")]
    public IActionResult GetCompany(Guid companyId)
    {
        var company = _context.Companies.Find(companyId);
        if (company == null)
            return NotFound("Company not found.");

        return Ok(company);
    }
}
