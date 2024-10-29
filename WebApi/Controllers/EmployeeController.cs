using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class EmployeeController : ApiController
    {
        IMapper _mapper;
        IEmployeeService _employeeService;
        Serilog.ILogger _logger;
        public EmployeeController(
            IEmployeeService employeeService, 
            IMapper mapper,
            Serilog.ILogger logger)
        {
            this._mapper = mapper;
            this._employeeService = employeeService;
            this._logger = logger;
        }

        public async Task<EmployeeDto> GetEmployeeByCode(string employeeCode)
        {
            return _mapper.Map<EmployeeDto>(await this._employeeService.GetByCode(employeeCode));
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployees()
        {
            return _mapper.Map<IEnumerable<EmployeeDto>>(await this._employeeService.GetAll());
        }

        public async Task Delete(string id)
        {
            await this._employeeService.Delete(id);
        }

        public async Task Post([FromBody] EmployeeDto employeeDto)
        { 
            // Setting id to null to indicate an insert action in the service layer.
            employeeDto.SiteId = null;
            await this._employeeService.Save(_mapper.Map<EmployeeInfo>(employeeDto));
        }

        public async Task Put(string id, [FromBody] EmployeeDto employeeDto)
        {
            // Setting id to employeeCode to indicate an update action in the service layer.
            employeeDto.SiteId = id;
            await this._employeeService.Save(_mapper.Map<EmployeeInfo>(employeeDto));
        }
    }
}
