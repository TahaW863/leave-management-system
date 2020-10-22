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

        public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            return await _dp.LeaveAllocations.Where(q => (q.EmployeeId == employeeId && q.Period == period && q.LeaveTypeId == leaveTypeId)).AnyAsync();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _dp.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _dp.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            return await _dp.LeaveAllocations.ToListAsync();
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await _dp.LeaveAllocations.Include(q => q.LeaveType)
                .Include(q => q.Employee).FirstOrDefaultAsync(q=>q.Id==id);
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return await _dp.LeaveAllocations.Where(q => q.EmployeeId == id
            && q.Period == period
            ).Include(q=> q.LeaveType).ToListAsync();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string EmployeeId, int LeveTypeId)
        {
            var period = DateTime.Now.Year;
            return await _dp.LeaveAllocations.FirstOrDefaultAsync(q => q.EmployeeId == EmployeeId &&
            q.LeaveTypeId==LeveTypeId
            && q.Period == period
            );
        }

        public async Task<bool> IsExists(int id)
        {
            return await _dp.LeaveAllocations.AnyAsync(i => i.Id == id);
        }

        public async Task<bool> Save()
        {
            return await _dp.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _dp.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
