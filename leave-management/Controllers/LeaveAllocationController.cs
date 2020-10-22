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
        private readonly ILeaveAllocationRepository _leaveAllocation;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(IMapper mapper, ILeaveAllocationRepository leaveAllocation,
            ILeaveTypeRepository leaveTypeRepository,
            UserManager<Employee> userManager)
        {
            _mapper = mapper;
            _leaveAllocation = leaveAllocation;
            _leaveTypeRepository = leaveTypeRepository;
            _userManager = userManager;
        }
        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            var leaveTypes =await _leaveTypeRepository.FindAll();
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
            var leavetype = await _leaveTypeRepository.FindById(id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                if (await _leaveAllocation.CheckAllocation(id, emp.Id))
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
                await _leaveAllocation.Create(leaveAllocation);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var employee = _mapper.Map <EmployeeViewModel> (await _userManager.FindByIdAsync(id));
            var allcations =_mapper.Map<List<LeaveAllocationViewModel>>(await _leaveAllocation.GetLeaveAllocationsByEmployee(id));
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
            var allocation = _mapper.Map<EditLeaveAllocationViewModel>(await _leaveAllocation.FindById(id));
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
                    var record =await _leaveAllocation.FindById(model.Id);
                    record.NumberOfDays = model.NumberOfDays;
                    if (!await _leaveAllocation.Update(record))
                    {
                        ModelState.AddModelError("", "Something Went Wrong!.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Something Went Wrong!.");
                    return View(model);
                }

                return RedirectToAction(nameof(Details),new { id = model.EmployeeId });
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
