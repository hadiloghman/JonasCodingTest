using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Threading.Tasks;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;

namespace DataAccessLayer.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbWrapper<Company> _companyDbWrapper;
        //private readonly IDbWrapper<Employee> _employeeDbWrapper;

        public CompanyRepository(
            IDbWrapper<Company> companyDbWrapper)
        //IDbWrapper<Employee> employeeRepository)
        {
            _companyDbWrapper = companyDbWrapper;
            //this._employeeDbWrapper = employeeRepository;
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _companyDbWrapper.FindAllAsync();
        }

        public async Task<Company> GetByCode(string companyCode)
        {
            return (await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
        }

        public async Task<Company> GetBySiteId(string siteId)
        {
            return (await _companyDbWrapper.FindAsync(t => t.SiteId.Equals(siteId, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
        }

        public async Task<Company> GetBySiteIdAndCode(string siteId, string companyCode)
        {
            return (await _companyDbWrapper.FindAsync(t => t.SiteId.Equals(siteId, StringComparison.InvariantCultureIgnoreCase)
            && t.CompanyCode.Equals(companyCode, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
        }

        public async Task<bool> SaveCompany(Company company)
        {
            var itemRepo = (await this.GetBySiteIdAndCode(company.SiteId, company.CompanyCode));
            if (itemRepo != null)
            {
                itemRepo.CompanyName = company.CompanyName;
                itemRepo.AddressLine1 = company.AddressLine1;
                itemRepo.AddressLine2 = company.AddressLine2;
                itemRepo.AddressLine3 = company.AddressLine3;
                itemRepo.Country = company.Country;
                itemRepo.EquipmentCompanyCode = company.EquipmentCompanyCode;
                itemRepo.FaxNumber = company.FaxNumber;
                itemRepo.PhoneNumber = company.PhoneNumber;
                itemRepo.PostalZipCode = company.PostalZipCode;
                itemRepo.LastModified = company.LastModified;
                return await _companyDbWrapper.UpdateAsync(itemRepo);
            }
            //for new company set SiteId
            company.SiteId = Guid.NewGuid().ToString();
            company.LastModified = DateTime.Now;
            return await _companyDbWrapper.InsertAsync(company);
        }

        public async Task DeleteCompany(string id)
        {
            await _companyDbWrapper.DeleteAsync(o => o.SiteId.Equals(id, StringComparison.InvariantCultureIgnoreCase));

        }

    }
}
