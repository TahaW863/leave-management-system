using leave_management.Contracts;
using leave_management.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private  IGenericRepository<LeaveType> _LeaveTypes;
        private  IGenericRepository<LeaveRequest> _LeaveRequests;
        private  IGenericRepository<LeaveAllocation> _LeaveAllocations;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<LeaveType> LeaveTypes
            => _LeaveTypes ??=  new GenericRepository<LeaveType>(_context);
        public IGenericRepository<LeaveAllocation> LeaveAllocations
            => _LeaveAllocations ??= new GenericRepository<LeaveAllocation>(_context);
        public IGenericRepository<LeaveRequest> LeaveRequests
            => _LeaveRequests ??= new GenericRepository<LeaveRequest>(_context);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                _context.Dispose();
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
