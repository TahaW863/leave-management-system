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
        private readonly IUnitOfWork _unitOfWork;

        //private readonly ILeaveTypeRepository _leaveTypeRepository;

        public LeaveTypesController(IMapper mapper,
            //ILeaveTypeRepository leaveTypeRepository,
            IUnitOfWork unitOfWork
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            // _leaveTypeRepository = leaveTypeRepository;
        }

        // GET: LeaveTypesController
        public async Task<ActionResult> Index()
        {
            //var leaveTypes = (await _leaveTypeRepository.FindAll()).ToList();
            var leaveTypes = (await _unitOfWork.LeaveTypes.FindAll()).ToList();
            var model = _mapper.Map<List<LeaveType>, List<DetailsLeaveTypeViewModel>>(leaveTypes);
            return View(model);
        }

        // GET: LeaveTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            // if (! await _leaveTypeRepository.IsExists(id))
            if (!await _unitOfWork.LeaveTypes.IsExists(q => q.Id == id))
            {
                return NotFound();
            }

            //var model = await _leaveTypeRepository.FindById(id);
            var model = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
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
                    //var isSuccess= await _leaveTypeRepository.Create(leaveType);
                    await _unitOfWork.LeaveTypes.Create(leaveType);
                    await _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));
                    //if (isSuccess)
                    //    return RedirectToAction(nameof(Index));
                    //else
                    //{
                    //    ModelState.AddModelError("", "Something Went Wrong!.");
                    //    return View(model);
                    //}
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
            // if (! await _leaveTypeRepository.IsExists(id))
            if (!await _unitOfWork.LeaveTypes.IsExists(q => q.Id == id))
            {
                return NotFound();
            }

            //var model = await _leaveTypeRepository.FindById(id);
            var model = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var passModel = _mapper.Map<DetailsLeaveTypeViewModel>(model);
            return View(passModel);
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
                    //if (! await _leaveTypeRepository.Update(leaveType))
                    _unitOfWork.LeaveTypes.Update(leaveType);
                    await _unitOfWork.Save();
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
            //if (! await _leaveTypeRepository.IsExists(id))
            if (!await _unitOfWork.LeaveTypes.IsExists(q => q.Id == id))
            {
                return NotFound();
            }
            //var model = _mapper.Map<DetailsLeaveTypeViewModel>(await _leaveTypeRepository.FindById(id));
            var model = _mapper.Map<DetailsLeaveTypeViewModel>(await _unitOfWork.LeaveTypes.Find(q=>q.Id==id));
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
                //await _leaveTypeRepository.Delete(mod);
                _unitOfWork.LeaveTypes.Delete(mod);
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
