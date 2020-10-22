using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _dp;

        public LeaveTypeRepository(ApplicationDbContext dp)
        {
            _dp = dp;
        }
        public async Task<bool> Create(LeaveType entity)
        {
            await _dp.LeaveTypes.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveType entity)
        {
            _dp.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveType>> FindAll()
        {
            return await _dp.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveType> FindById(int id)
        {
            return await _dp.LeaveTypes.FindAsync(id);
        }

        

        public async Task<bool> IsExists(int id)
        {
            return await _dp.LeaveTypes.AnyAsync(i=> i.Id==id);
        }

        public async Task<bool> Save()
        {
            return await _dp.SaveChangesAsync()> 0;
        }

        public async Task<bool> Update(LeaveType entity)
        {
            _dp.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
