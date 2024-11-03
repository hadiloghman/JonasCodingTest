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
            await this._employeeDbWrapper.DeleteAsync(o => o.SiteId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
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
            var employee = (await this._employeeDbWrapper.FindAsync(o => o.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
            if (employee != null && !string.IsNullOrEmpty(employee.CompanyCode))
            {
                employee.CompanyName = (await this._companyRepository.GetByCode(employee.CompanyCode))?.CompanyName;
            }
            return employee;
        }

        public async Task<Employee> GetBySiteId(string siteId)
        {
            return (await _employeeDbWrapper.FindAsync(t => t.SiteId.Equals(siteId, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
        }

        public async Task<Employee> GetBySiteIdAndCode(string siteId, string employeeCode)
        {
            return (await _employeeDbWrapper.FindAsync(t => t.SiteId.Equals(siteId, StringComparison.InvariantCultureIgnoreCase)
            && t.EmployeeCode.Equals(employeeCode, StringComparison.InvariantCultureIgnoreCase)))?.FirstOrDefault();
        }

        public async Task<bool> Save(Employee employee)
        {
            var entry = await this.GetBySiteIdAndCode(employee.SiteId, employee.EmployeeCode);
            if (entry != null)
            {
                entry.EmployeeCode = employee.EmployeeCode;
                entry.EmployeeName = employee.EmployeeName;
                entry.Occupation = employee.Occupation;
                entry.EmployeeStatus = employee.EmployeeStatus;
                entry.EmailAddress = employee.EmailAddress;
                entry.Phone = employee.Phone;
                entry.LastModified = DateTime.Now;
                entry.CompanyCode = employee.CompanyCode;
                return await _employeeDbWrapper.UpdateAsync(entry);
            }


            //for new company set SiteId
            employee.SiteId = Guid.NewGuid().ToString();
            employee.LastModified = DateTime.Now;
            return await _employeeDbWrapper.InsertAsync(employee);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyCode(string companyCode)
        {
            return (await this._employeeDbWrapper.FindAsync(o => o.CompanyCode.Equals(companyCode, StringComparison.InvariantCultureIgnoreCase))).AsEnumerable();
        }
    }
}
