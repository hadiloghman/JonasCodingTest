using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Model.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeInfo>> GetAll();
        Task<EmployeeInfo> GetByCode(string employeeCode);
        Task Save(EmployeeInfo employee);
        Task Delete(string id);
    }
}
