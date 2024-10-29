using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Microsoft.Extensions.Logging;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class CompanyController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;
        public CompanyController(ICompanyService companyService, 
            IMapper mapper,
            Serilog.ILogger logger)
        {
            _companyService = companyService;
            _mapper = mapper;
            _logger = logger;
        }
        // GET api/<controller>
        public async Task<IEnumerable<CompanyDto>> GetAll()
        {
            var items = await _companyService.GetAllCompanies();
            return _mapper.Map<IEnumerable<CompanyDto>>(items);
        }

        // GET api/<controller>/5
        public async Task<CompanyDto> Get(string companyCode)
        {
            var item = await _companyService.GetCompanyByCode(companyCode);
            return _mapper.Map<CompanyDto>(item);
        }

        // POST api/<controller>
        public async Task Post([FromBody] CompanyDto value)
        {
            // Setting companyCode to null to indicate an insert action in the service layer.
            value.SiteId = null;
            await _companyService.SaveCompany(_mapper.Map<CompanyInfo>(value));
        }

        // PUT api/<controller>/5
        public async Task Put(string id, [FromBody] CompanyDto value)
        {
            // Setting companyCode to id.Tostring() to indicate an update action in the service layer.
            value.SiteId = id.ToString();
            await _companyService.SaveCompany(_mapper.Map<CompanyInfo>(value));
        }

        // DELETE api/<controller>/5
        public async void Delete(string id)
        {
            await _companyService.DeleteCompany(id);
        }
    }
}