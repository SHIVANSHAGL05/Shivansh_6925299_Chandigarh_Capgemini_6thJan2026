using System;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        // GET: /Students
        public IActionResult Index()
        {
            var students = _service.GetAll();
            return View(students);
        }

        // GET: /Students/Details/5
        public IActionResult Details(int id)
        {
            var student = _service.GetById(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // -----------------------------------------------------------------------
        // CREATE
        // -----------------------------------------------------------------------

        // POST: /Students/Create  (called from modal via AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                // Return JSON with validation errors so the modal can display them
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(er => er.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, errors });
            }

            _service.Add(student);
            TempData["SuccessMessage"] = $"Student \"{student.Name}\" added successfully.";
            return Json(new { success = true });
        }

        // -----------------------------------------------------------------------
        // EDIT
        // -----------------------------------------------------------------------

        // GET: /Students/Edit/5  (returns partial for modal)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _service.GetById(id);
            if (student == null) return NotFound();
            return Json(student);          // return data so JS can populate the modal
        }

        // POST: /Students/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(er => er.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, errors });
            }

            _service.Update(student);
            TempData["SuccessMessage"] = $"Student \"{student.Name}\" updated successfully.";
            return Json(new { success = true });
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        // POST: /Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var student = _service.GetById(id);
            if (student == null) return NotFound();
            _service.Delete(id);
            TempData["SuccessMessage"] = $"Student \"{student.Name}\" deleted successfully.";
            return Json(new { success = true });
        }
    }
}
