using Azure;
using Frontend.DTO;
using Frontend.Exception;
using Frontend.HttpServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Frontend.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudent _studentService;

        public StudentController(IStudent student)
        {
            _studentService = student;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        //Send Request to the Student Service
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(StudentDto student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(student);
                }

                var response = await _studentService.CreateStudent(student);

                TempData["SuccessMessage"] = response;


                return RedirectToAction("list", "student");
            }
            catch (HttpStatusCodeException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(student);
            }
            catch (ApplicationException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(student);
            }          
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(student);
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var studentList = await _studentService.ListOfStudent();

                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

                return View(studentList);
            }
            catch (HttpStatusCodeException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (BadGatewayException ex)
            {
                Response.StatusCode = 502;
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (ApplicationException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpGet]
        [Route("/Student/Edit/{studentId}")]
        public async Task<IActionResult> Edit(Guid studentId)
        {
            try
            {
                var student = await _studentService.GetStudent(studentId);

                return View(student);
            }
            catch (HttpStatusCodeException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (ApplicationException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentDto student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(student);
                }

                var response = await _studentService.UpdateStudent(student);

                TempData["SuccessMessage"] = response;

                return RedirectToAction("list", "student");
            }
            catch (HttpStatusCodeException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (ApplicationException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Student/Delete/{studentId}")]
        public async Task<IActionResult> Delete(Guid studentId)
        {
            try
            {
              
                var response = await _studentService.DeleteStudent(studentId);

                TempData["SuccessMessage"] = response;

                return RedirectToAction("list", "student");

            }
            catch (HttpStatusCodeException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (ApplicationException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }
    }
}
