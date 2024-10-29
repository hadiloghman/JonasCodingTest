using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<EmployeeInfo>> GetAll()
        {
            var res = await _employeeRepository.GetAll();
            return _mapper.Map<IEnumerable<EmployeeInfo>>(res);
        }

        public async Task<EmployeeInfo> GetByCode(string employeeCode)
        {
            var result = await _employeeRepository.GetByCode(employeeCode);
            return _mapper.Map<EmployeeInfo>(result);
        }

        public async Task Delete(string employeeCode)
        {
            await _employeeRepository.Delete(employeeCode);
        }

        public async Task Save(EmployeeInfo employeeInfo)
        {
            await _employeeRepository.Save(_mapper.Map<Employee>(employeeInfo));
        }
    }
}
