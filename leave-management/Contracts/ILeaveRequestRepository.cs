﻿using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface ILeaveRequestRepository: IRepositoryBase<LeaveRequest>
    {
         Task<ICollection<LeaveRequest>> GetLeaveRequests(string employeeid);
    }
}
