using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbWrapper<Employee> _employeeDbWrapper;
        private readonly ICompanyRepository _companyRepository;
        public EmployeeRepository(IDbWrapper<Employee> employeeDbWrapper, ICompanyRepository companyRepository)
        {
            this._employeeDbWrapper = employeeDbWrapper;
            this._companyRepository = companyRepository;
        }

        public async Task Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw ErrorHandled.ArgumentNotSpecified(nameof(id));
            var entry = (await this._employeeDbWrapper.FindAsync(o => o.SiteId.Equals(id, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
            if (entry == null)
                throw ErrorHandled.NotFound(id, nameof(Employee));
            await this._employeeDbWrapper.DeleteAsync(o => o.SiteId == entry.SiteId.ToString());
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var lstEmployees = await this._employeeDbWrapper.FindAllAsync();
            var lstCompanies = await this._companyRepository.GetAll();
            foreach (var employee in lstEmployees)
            {
                employee.CompanyName = !string.IsNullOrEmpty(employee.CompanyCode) ? lstCompanies.FirstOrDefault(p => p.CompanyCode.Equals(employee.CompanyCode))?.CompanyName : null;
            }
            return lstEmployees;
        }

        public async Task<Employee> GetByCode(string employeeCode)
        {
            if (string.IsNullOrEmpty(employeeCode))
                throw ErrorHandled.ArgumentNotSpecified(nameof(employeeCode));
            var employee = (await this._employeeDbWrapper.FindAsync(o => o.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
            if (employee != null && !string.IsNullOrEmpty(employee.CompanyCode))
            {
                employee.CompanyName = (await this._companyRepository.GetByCode(employee.CompanyCode))?.CompanyName;
            }
            return employee;
        }

        public async Task<bool> Save(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.CompanyCode))
                throw ErrorHandled.ArgumentNotSpecified("CompanyCode");
            var entryCompany = await _companyRepository.GetByCode(employee.CompanyCode);
            if (entryCompany == null)
                throw ErrorHandled.NotFound(employee.CompanyCode, nameof(Company));

            bool updateRequested = !string.IsNullOrEmpty(employee.SiteId);

            var entry = (await _employeeDbWrapper.FindAsync(o =>
               o.SiteId.Equals(employee.SiteId)
               && o.EmployeeCode.Equals(employee.EmployeeCode)))?.FirstOrDefault();
            if (entry != null)
            {
                entry.EmployeeCode = employee.EmployeeCode;
                entry.EmployeeName = employee.EmployeeName;
                entry.Occupation = employee.Occupation;
                entry.EmployeeStatus = employee.EmployeeStatus;
                entry.EmailAddress = employee.EmailAddress;
                entry.Phone = employee.Phone;
                entry.LastModified = DateTime.Now;
                return await _employeeDbWrapper.UpdateAsync(entry);
            }
            else if (updateRequested)
            {
                //**************Note****************
                // Attempting to update an item:
                // - If no item exists with the specified companyCode, throw an exception.
                // - Note: GetCompany retrieves data by companyCode only, whereas saving requires both siteId and companyCode.
                throw ErrorHandled.NotFound(employee.SiteId, nameof(Employee));

            }
            var entryDuplicate = await GetByCode(employee.EmployeeCode);
            if (entryDuplicate != null)
                throw ErrorHandled.DuplicateCode(employee.EmployeeCode, nameof(employee));
            //for new company set SiteId
            employee.SiteId = Guid.NewGuid().ToString();
            return await _employeeDbWrapper.InsertAsync(employee);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyCode(string companyCode)
        {
            return (await this._employeeDbWrapper.FindAsync(o => o.CompanyCode == companyCode)).AsEnumerable();
        }
    }
}
