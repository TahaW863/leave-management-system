using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _dp;

        public LeaveRequestRepository(ApplicationDbContext dp)
        {
            _dp = dp;
        }
        public bool Create(LeaveRequest entity)
        {
            _dp.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _dp.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            return _dp.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q=>q.LeaveType)
                .Include(q=>q.ApprovedBy)
                .ToList();
        }

        public LeaveRequest FindById(int id)
        {
            return _dp.LeaveRequests
                .Include(q => q.LeaveType)
                .Include(q => q.ApprovedBy)
                .Include(q => q.RequestingEmployee)
                .FirstOrDefault(q=>q.Id==id);
        }

        public IEnumerable<LeaveRequest> GetLeaveRequests(string employeeid)
        {
            return _dp.LeaveRequests
                .Include(q => q.LeaveType)
                .Include(q => q.ApprovedBy)
                .Include(q => q.RequestingEmployee)
                .Where(q=>q.RequestingEmployeeId==employeeid);
        }

        public bool IsExists(int id)
        {
            return _dp.LeaveRequests.Where(q=>q.Id==id).Any();
        }

        public bool Save()
        {
            return _dp.SaveChanges() > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _dp.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
