using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _dp;

        public LeaveAllocationRepository(ApplicationDbContext dp)
        {
            _dp = dp;
        }

        public bool CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            return FindAll().Where(q => (q.EmployeeId == employeeId && q.Period == period && q.LeaveTypeId == leaveTypeId)).Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _dp.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _dp.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            return _dp.LeaveAllocations.ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _dp.LeaveAllocations.Include(q => q.LeaveType)
                .Include(q => q.Employee).FirstOrDefault(q=>q.Id==id);
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return _dp.LeaveAllocations.Where(q => q.EmployeeId == id
            && q.Period == period
            ).Include(q=> q.LeaveType).ToList();
        }

        public LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string EmployeeId, int LeveTypeId)
        {
            var period = DateTime.Now.Year;
            return _dp.LeaveAllocations.FirstOrDefault(q => q.EmployeeId == EmployeeId &&
            q.LeaveTypeId==LeveTypeId
            && q.Period == period
            );
        }

        public bool IsExists(int id)
        {
            return _dp.LeaveAllocations.FirstOrDefault(i => i.Id == id) != null;
        }

        public bool Save()
        {
            return _dp.SaveChanges() > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _dp.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
