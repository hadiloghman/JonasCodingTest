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
            if (string.IsNullOrEmpty(companyCode))
                throw ErrorHandled.ArgumentNotSpecified(nameof(companyCode));
            return (await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)))?.FirstOrDefault();
        }

        public async Task<bool> SaveCompany(Company company)
        {
            bool updateRequested = !string.IsNullOrEmpty(company.SiteId);
            var itemRepo = (await _companyDbWrapper.FindAsync(t =>
                t.SiteId.Equals(company.SiteId)
                && t.CompanyCode.Equals(company.CompanyCode)))?.FirstOrDefault();
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
            else if (updateRequested)
            {
                //**************Note****************
                // Attempting to update an item:
                // - If no item exists with the specified companyCode, throw an exception.
                // - Note: GetCompany retrieves data by companyCode only, whereas saving requires both siteId and companyCode.
                throw ErrorHandled.NotFound(company.SiteId, nameof(company));

            }
            var entryDuplicate = await GetByCode(company.CompanyCode);
            if (entryDuplicate != null)
                throw ErrorHandled.DuplicateCode(company.CompanyCode, nameof(company));
            //for new company set SiteId
            company.SiteId = Guid.NewGuid().ToString();
            company.LastModified = DateTime.Now;
            return await _companyDbWrapper.InsertAsync(company);
        }

        public async Task DeleteCompany(string id)
        {

            if (string.IsNullOrEmpty(id))
                throw ErrorHandled.ArgumentNotSpecified(nameof(id));
            var entry = (await this._companyDbWrapper.FindAsync(o => o.SiteId.Equals(id, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
            if (entry == null)
                throw ErrorHandled.NotFound(id, nameof(Company));

            //var employees = (await this._employeeDbWrapper.FindAsync(o => o.CompanyCode.Equals(entry.CompanyCode, StringComparison.InvariantCultureIgnoreCase))).AsEnumerable();
            //if (employees.Count() > 0)
            //    throw ErrorHandled.RelationExists(nameof(Company), id, nameof(Employee));
            await _companyDbWrapper.DeleteAsync(o => o.SiteId.Equals(id, StringComparison.InvariantCultureIgnoreCase));

        }

    }
}
