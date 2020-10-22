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
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public LeaveTypesController(IMapper mapper, ILeaveTypeRepository leaveTypeRepository)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
        }
        
        // GET: LeaveTypesController
        public async Task<ActionResult> Index()
        {
            var leaveTypes = (await _leaveTypeRepository.FindAll()).ToList();
            var model = _mapper.Map<List<LeaveType>, List<DetailsLeaveTypeViewModel>>(leaveTypes);
            return View(model);
        }

        // GET: LeaveTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (! await _leaveTypeRepository.IsExists(id))
            {
                return NotFound();
            }

            var model = await _leaveTypeRepository.FindById(id);
            var passModel = _mapper.Map<DetailsLeaveTypeViewModel>(model);
            return View(passModel);
        }

        // GET: LeaveTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DetailsLeaveTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var leaveType = _mapper.Map<LeaveType>(model);
                    leaveType.DateCreated = DateTime.Now;
                    var isSuccess= await _leaveTypeRepository.Create(leaveType);
                    if (isSuccess)
                        return RedirectToAction(nameof(Index));
                    else
                    {
                        ModelState.AddModelError("", "Something Went Wrong!.");
                        return View(model);
                    }
                }
                
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong!.");
                return View(model);
            }
        }

        // GET: LeaveTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if ( await _leaveTypeRepository.IsExists(id))
            {
                var model = _mapper.Map<DetailsLeaveTypeViewModel>(await _leaveTypeRepository.FindById(id));
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: LeaveTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DetailsLeaveTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var leaveType = _mapper.Map<LeaveType>(model);
                    if (! await _leaveTypeRepository.Update(leaveType))
                    {
                        ModelState.AddModelError("", "Something Went Wrong!.");
                        return View(model);
                    }
                   
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Something Went Wrong!.");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong!.");
                return View(model);
            }
        }

        // GET: LeaveTypesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (! await _leaveTypeRepository.IsExists(id))
            {
                return NotFound();
            }
            var model = _mapper.Map<DetailsLeaveTypeViewModel>(await _leaveTypeRepository.FindById(id));
            return View(model);
        }

        // POST: LeaveTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DetailsLeaveTypeViewModel model)
        {
            try
            {
                var mod = _mapper.Map<LeaveType>(model);
                await _leaveTypeRepository.Delete(mod);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
