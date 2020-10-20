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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepository,
            ILeaveTypeRepository leaveTypeRepository,
            ILeaveAllocationRepository leaveAllocationRepository,
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveAllocationRepository = leaveAllocationRepository;
            _mapper = mapper;
            _userManager = userManager;
        }
        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public ActionResult Index()
        {
            var leaveRequests = _mapper.Map<List<LeaveRequestVM>>(_leaveRequestRepository.FindAll());
            var model = new AdminLeaveRequestVM
            {
                TotalRequests=leaveRequests.Count,
                ApprovedRequests=leaveRequests.Count(q=> q.Approved==true),
                LeaveRequestVMs=leaveRequests,
                PendingRequests=leaveRequests.Count(q=> q.Approved==null),
                RejectedRequests= leaveRequests.Count(q => q.Approved == false),
            };
            
            return View(model);
        }
        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            var LeaveRequests = _leaveRequestRepository.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(LeaveRequests);
            return View(model);
        }
        [Authorize(Roles = "Administrator")]
        
        public ActionResult ApproveRequest (int id)
        {
            try
            {
                var leaveRequest = _leaveRequestRepository.FindById(id);
                leaveRequest.Approved = true;
                var user= _userManager.GetUserAsync(User).Result;
                leaveRequest.ApprovedBy = user;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;
                var allocation = _leaveAllocationRepository.GetLeaveAllocationsByEmployeeAndType(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypId);
                int daysRequested = Convert.ToInt32((leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).TotalDays);

                allocation.NumberOfDays -= daysRequested;
                _leaveAllocationRepository.Update(allocation);
                _leaveRequestRepository.Update(leaveRequest);
                return RedirectToAction(nameof(Index));
                
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [Authorize(Roles = "Administrator")]
        
        public ActionResult RejectRequest(int id)
        {
            try
            {
                var leaveRequest = _leaveRequestRepository.FindById(id);
                
                var user = _userManager.GetUserAsync(User).Result;
               
                

                leaveRequest.ApprovedBy = user;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.Approved = false;

               
                _leaveRequestRepository.Update(leaveRequest);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong!.");
                return RedirectToAction(nameof(Details), id);
            }
        }
        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var allcations = _mapper.Map<List<LeaveAllocationViewModel>>(_leaveAllocationRepository.GetLeaveAllocationsByEmployee(employee.Id));
            var leaveReqs = _mapper.Map<List<LeaveRequestVM>>(_leaveRequestRepository.GetLeaveRequests(employee.Id));
            var model = new MyLeaveVM
            {
                LeaveAllocations=allcations,
                LeaveRequestVMs=leaveReqs
            };
            return View(model);
        }
            // GET: LeaveRequestController/Create
            public ActionResult Create()
        {
            var leaveTypes =_leaveTypeRepository.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem {
                Text=q.Name,
                Value=q.Id.ToString()
            });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes=leaveTypeItems
            };
            return View(model);
        }
        public ActionResult CancelRequestEmployee(int id)
        {
            try
            {
                var leaveRequest = _leaveRequestRepository.FindById(id);
                _leaveRequestRepository.Delete(leaveRequest);
                return RedirectToAction(nameof(MyLeave));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something Went Wrong!.");
                return RedirectToAction(nameof(MyLeave));
            }
            
        }
        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var leaveTypes = _leaveTypeRepository.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Something Went Wrong!.");
                    return View(model);
                }
                if (DateTime.Compare(startDate, endDate) > 1 && DateTime.Compare(startDate, DateTime.Now)>=0)
                {
                    ModelState.AddModelError("", "Enter Valid Date.");
                    return View(model);
                }
                
                var employee = _userManager.GetUserAsync(User).Result;
                var allocations = _leaveAllocationRepository.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypId);
                int daysRequested = Convert.ToInt32((endDate.Date - startDate.Date).TotalDays);
                if (daysRequested > allocations.NumberOfDays)
                {
                    ModelState.AddModelError("", "Enter Valid number of Days.");
                    return View(model);
                }
                var LeaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId=employee.Id,
                    StartDate =startDate,
                    EndDate=endDate,
                    LeaveTypId=model.LeaveTypId,
                    DateRequested=DateTime.Now,
                    Approved=null
                };
                var leaveRequestEntity = _mapper.Map<LeaveRequest>(LeaveRequestModel);
                if (_leaveRequestRepository.Create(leaveRequestEntity))
                {

                    return RedirectToAction(nameof(Index),"Home");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong with submitting your record.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something Went Wrong!.");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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
