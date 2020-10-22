using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _dp;

        public LeaveRequestRepository(ApplicationDbContext dp)
        {
            _dp = dp;
        }
        public async Task<bool> Create(LeaveRequest entity)
        {
            await _dp.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _dp.LeaveRequests.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveRequest>> FindAll()
        {
            return await _dp.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q=>q.LeaveType)
                .Include(q=>q.ApprovedBy)
                .ToListAsync();
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            return await _dp.LeaveRequests
                .Include(q => q.LeaveType)
                .Include(q => q.ApprovedBy)
                .Include(q => q.RequestingEmployee)
                .FirstOrDefaultAsync(q=>q.Id==id);
        }

        public async Task<ICollection<LeaveRequest>> GetLeaveRequests(string employeeid)
        {
            return await _dp.LeaveRequests
                .Include(q => q.LeaveType)
                .Include(q => q.ApprovedBy)
                .Include(q => q.RequestingEmployee)
                .Where(q=>q.RequestingEmployeeId==employeeid).ToListAsync();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _dp.LeaveRequests.Where(q=>q.Id==id).AnyAsync();
        }

        public async Task<bool> Save()
        {
            return await _dp.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _dp.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
