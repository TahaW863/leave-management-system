using leave_management.Contracts;
using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveHistoryRepository : ILeaveHistoryRepository
    {
        private readonly ApplicationDbContext _dp;

        public LeaveHistoryRepository(ApplicationDbContext dp)
        {
            _dp = dp;
        }
        public bool Create(LeaveHistory entity)
        {
            _dp.LeaveHistories.Add(entity);
            return Save();
        }

        public bool Delete(LeaveHistory entity)
        {
            _dp.LeaveHistories.Remove(entity);
            return Save();
        }

        public ICollection<LeaveHistory> FindAll()
        {
            return _dp.LeaveHistories.ToList();
        }

        public LeaveHistory FindById(int id)
        {
            return _dp.LeaveHistories.Find(id);
        }

        public bool Save()
        {
            return _dp.SaveChanges() > 0;
        }

        public bool Update(LeaveHistory entity)
        {
            _dp.LeaveHistories.Remove(entity);
            return Save();
        }
    }
}
