using BusinessLayer.Model.Interfaces;
using System.Collections.Generic;
using AutoMapper;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using System.Threading.Tasks;
using DataAccessLayer.Model.Models;
using System;
using System.Linq;

namespace BusinessLayer.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public CompanyService(
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper
            )
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyInfo>> GetAllCompanies()
        {
            var res = await _companyRepository.GetAll();
            return _mapper.Map<IEnumerable<CompanyInfo>>(res);
        }

        public async Task<CompanyInfo> GetCompanyByCode(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                throw ErrorHandled.ArgumentNotSpecified(nameof(companyCode));

            var result = await _companyRepository.GetByCode(companyCode);
            return _mapper.Map<CompanyInfo>(result);
        }

        public async Task DeleteCompany(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw ErrorHandled.ArgumentNotSpecified(nameof(id));
            var entry = (await this._companyRepository.GetBySiteId(id));
            if (entry == null)
                throw ErrorHandled.NotFound(id, nameof(Company));

            var lstEmployee = await this._employeeRepository.GetEmployeesByCompanyCode(entry.CompanyCode);
            if (lstEmployee.Count() > 0)
                throw ErrorHandled.RelationExists(nameof(Company), id, nameof(Employee));

            await _companyRepository.DeleteCompany(id);
        }

        public async Task SaveCompany(CompanyInfo companyInfo)
        {
            bool updateRequested = !string.IsNullOrEmpty(companyInfo.SiteId);
            if (updateRequested)
            {
                //**************Note****************
                // Attempting to update an item:
                // - If no item exists with the specified companyCode, throw an exception.
                // - Note: GetCompany retrieves data by companyCode only, whereas saving requires both siteId and companyCode.
                var entry = await this._companyRepository.GetBySiteIdAndCode(companyInfo.SiteId, companyInfo.CompanyCode);
                if (entry == null)
                    throw ErrorHandled.NotFound(companyInfo.SiteId, nameof(Company));
            }
            else
            {
                var entryDuplicate = await this._companyRepository.GetByCode(companyInfo.CompanyCode);
                if (entryDuplicate != null)
                    throw ErrorHandled.DuplicateCode(companyInfo.CompanyCode, nameof(Company));
            }
            await _companyRepository.SaveCompany(_mapper.Map<Company>(companyInfo));
        }
    }
}
