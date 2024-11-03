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
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository,
            IMapper mapper
            )
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeInfo>> GetAll()
        {
            var res = await _employeeRepository.GetAll();
            return _mapper.Map<IEnumerable<EmployeeInfo>>(res);
        }

        public async Task<EmployeeInfo> GetByCode(string employeeCode)
        {
            if (string.IsNullOrEmpty(employeeCode))
                throw ErrorHandled.ArgumentNotSpecified(nameof(employeeCode));
            var result = await _employeeRepository.GetByCode(employeeCode);
            return _mapper.Map<EmployeeInfo>(result);
        }

        public async Task Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw ErrorHandled.ArgumentNotSpecified(nameof(id));
            var entry = (await this._employeeRepository.GetBySiteId(id));
            if (entry == null)
                throw ErrorHandled.NotFound(id, nameof(Employee));

            await _employeeRepository.Delete(id);
        }

        public async Task Save(EmployeeInfo employeeInfo)
        {
            if (string.IsNullOrEmpty(employeeInfo.CompanyCode))
                throw ErrorHandled.ArgumentNotSpecified("CompanyCode");
            var entryCompany = await _companyRepository.GetByCode(employeeInfo.CompanyCode);
            if (entryCompany == null)
                throw ErrorHandled.NotFound(employeeInfo.CompanyCode, nameof(Company));

            bool updateRequested = !string.IsNullOrEmpty(employeeInfo.SiteId);

            if (updateRequested)
            {
                var entry = await this._employeeRepository.GetBySiteIdAndCode(employeeInfo.SiteId, employeeInfo.EmployeeCode);
                if (entry == null)
                {
                    //**************Note****************
                    // Attempting to update an item:
                    // - If no item exists with the specified companyCode, throw an exception.
                    // - Note: GetCompany retrieves data by companyCode only, whereas saving requires both siteId and companyCode.
                    throw ErrorHandled.NotFound(employeeInfo.SiteId, nameof(Employee));
                }
            }
            else
            {
                var entryDuplicate = await GetByCode(employeeInfo.EmployeeCode);
                if (entryDuplicate != null)
                    throw ErrorHandled.DuplicateCode(employeeInfo.EmployeeCode, nameof(Employee));
            }
            await _employeeRepository.Save(_mapper.Map<Employee>(employeeInfo));
        }
    }
}
