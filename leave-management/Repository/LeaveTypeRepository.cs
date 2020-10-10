using leave_management.Contracts;
using leave_management.Data;
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
        public bool Create(LeaveType entity)
        {
            _dp.LeaveTypes.Add(entity);
            return Save();
        }

        public bool Delete(LeaveType entity)
        {
            _dp.LeaveTypes.Remove(entity);
            return Save();
        }

        public ICollection<LeaveType> FindAll()
        {
            return _dp.LeaveTypes.ToList();
        }

        public LeaveType FindById(int id)
        {
            return _dp.LeaveTypes.Find(id);
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            return _dp.SaveChanges()> 0;
        }

        public bool Update(LeaveType entity)
        {
            _dp.LeaveTypes.Update(entity);
            return Save();
        }
    }
}
