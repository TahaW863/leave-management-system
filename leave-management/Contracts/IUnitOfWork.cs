using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<LeaveType> LeaveTypes { get; }
        public IGenericRepository<LeaveAllocation> LeaveAllocations { get;}
        public IGenericRepository<LeaveRequest> LeaveRequests { get;}
        Task Save();
    }
}
