using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        //private readonly ILeaveAllocationRepository _leaveAllocation;
        //private readonly ILeaveTypeRepository _leaveTypeRepository;

        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(IMapper mapper,
            //ILeaveAllocationRepository leaveAllocation,
            //ILeaveTypeRepository leaveTypeRepository,
            IUnitOfWork unitOfWork,
            UserManager<Employee> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            //_leaveAllocation = leaveAllocation;
            //_leaveTypeRepository = leaveTypeRepository;
            _userManager = userManager;
        }
        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            //var leaveTypes =await _leaveTypeRepository.FindAll();
            var leaveTypes =await _unitOfWork.LeaveTypes.FindAll();
            var mappedLeaveAllocation = _mapper.Map<List<LeaveType>, List<DetailsLeaveTypeViewModel>>(leaveTypes.ToList());
            var model = new CreateLeaveAllocationViewModel
            {
                LeaveTypes = mappedLeaveAllocation,
                NumberUpdated = 0
            };
            return View(model);
        }
        public async Task<ActionResult> ListEmployees()
        {
            var employees =await _userManager.GetUsersInRoleAsync("Employee");
            var mapped = _mapper.Map<List<EmployeeViewModel>>(employees);
            return View(mapped);
        }
        public async Task<ActionResult> SetLeave(int id)
        {
            //var leavetype = await _leaveTypeRepository.FindById(id);
            var leavetype = await _unitOfWork.LeaveTypes.Find(q=>q.Id==id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                //if (await _leaveAllocation.CheckAllocation(id, emp.Id))
                var period = DateTime.Now.Year;
                if (await _unitOfWork.LeaveAllocations.IsExists(q => (q.EmployeeId == emp.Id && q.Period == period && q.LeaveTypeId == id)))
                {
                    continue;
                }
                    var allocations = new LeaveAllocationViewModel
                    {
                        EmployeeId = emp.Id,
                        DateCreated = DateTime.Now,
                        LeaveTypeId = id,
                        NumberOfDays = leavetype.DefaultNumberOfDays,
                        Period = DateTime.Now.Year
                    };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocations);
                //await _leaveAllocation.Create(leaveAllocation);
                await _unitOfWork.LeaveAllocations.Create(leaveAllocation);
                await _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var employee = _mapper.Map <EmployeeViewModel> (await _userManager.FindByIdAsync(id));
            //var allcations =_mapper.Map<List<LeaveAllocationViewModel>>(await _leaveAllocation.GetLeaveAllocationsByEmployee(id));
            var period = DateTime.Now.Year;
            List<string> includes = new List<string> { "LeaveType" };
            var allcations =_mapper.Map<List<LeaveAllocationViewModel>>(await _unitOfWork.LeaveAllocations.FindAll(
                q => q.EmployeeId == id
                && q.Period == period,
                null,
                includes
                ));
            var model = new LeaveAllocationVM2
            {
                Employee=employee,
                LeaveAllocations=allcations
            };
            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            //var allocation = _mapper.Map<EditLeaveAllocationViewModel>(await _leaveAllocation.FindById(id));
            List<string> includes = new List<string> { "Employee" , "LeaveType" };
            var allocation = _mapper.Map<EditLeaveAllocationViewModel>(await _unitOfWork.LeaveAllocations.Find(q=>q.Id==id,includes));
            return View(allocation);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EditLeaveAllocationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var record =await _leaveAllocation.FindById(model.Id);
                    List<string> includes = new List<string> { "Employee", "LeaveType" };
                    var record = await _unitOfWork.LeaveAllocations.Find(q => q.Id == model.Id,
                        includes);
                    record.NumberOfDays = model.NumberOfDays;
                    //if (!await _leaveAllocation.Update(record))
                    _unitOfWork.LeaveAllocations.Update(record);
                    await _unitOfWork.Save();
                    return RedirectToAction(nameof(Details), new { id = model.EmployeeId });
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
