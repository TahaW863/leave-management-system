using leave_management.Contracts;
using leave_management.Data;
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
            return _dp.LeaveAllocations.Find(id);
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
