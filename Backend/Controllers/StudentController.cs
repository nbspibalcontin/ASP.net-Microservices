using Backend.Exception;
using Backend.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentService.DTOs.Request;
using StudentService.DTOs.Response;
using StudentService.Exception;

namespace Backend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IStudent _studentService;

        public StudentController(IStudent studentService)
        {
            _studentService = studentService;
        }

        //Create Student
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateStudent([FromForm] StudentRequest studentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _studentService.CreateStudent(studentRequest);

                return Ok(response);
            }
            catch (StudentAlreadyExistsException ex)
            {
                return StatusCode(409, new { ErrorMessage = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        //List of Student
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> ListStudent()
        {
            try
            {
                var response = await _studentService.ListOfStudent();

                return Ok(response);
            }
            catch (StudentNotFoundException ex)
            {
                return StatusCode(404, new { ErrorMessage = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        //Student Details by Id
        [HttpGet]
        [Route("{studentId}")]
        public async Task<IActionResult> StudentById(Guid studentId)
        {
            try
            {
                var response = await _studentService.StudentById(studentId);

                return Ok(response);
            }
            catch (StudentNotFoundException ex)
            {
                return StatusCode(404, new { ErrorMessage = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        //Update Student Details
        [HttpPut]
        [Route("update/{studentId}")]
        public async Task<IActionResult> UpdateStudentDetails([FromForm] StudentRequest studentRequest, Guid studentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _studentService.UpdateStudentInfo(studentRequest, studentId);

                return Ok(response);
            }
            catch (StudentNotFoundException ex)
            {
                return StatusCode(404, new { ErrorMessage = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        //Delete Student
        [HttpDelete]
        [Route("delete/{studentId}")]
        public async Task<IActionResult> DeleteStudentById(Guid studentId)
        {
            try
            {
                var response = await _studentService.DeleteStudentById(studentId);

                return Ok(response);
            }
            catch (StudentNotFoundException ex)
            {
                return StatusCode(404, new { ErrorMessage = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}
