using AutoMapper;
using Backend.Data;
using Backend.DTOs.Response;
using Backend.Exception;
using Backend.Services.Interface;
using Microsoft.EntityFrameworkCore;
using StudentService.DTOs.Request;
using StudentService.DTOs.Response;
using StudentService.Entity;
using StudentService.Exception;

namespace Backend.Services.Implementation
{
    public class StudentServices : IStudent
    {
        private readonly StudentDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudentServices(StudentDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        //Create Student
        public async Task<MessageResponse> CreateStudent(StudentRequest studentRequest)
        {
            try
            {
                // Check if email already exists
                if (_dbContext.Students.Any(s => s.Email == studentRequest.Email))
                {
                    throw new StudentAlreadyExistsException("Email already exists.");
                }

                // Check if phone number already exists
                if (_dbContext.Students.Any(s => s.PhoneNumber == studentRequest.PhoneNumber))
                {
                    throw new StudentAlreadyExistsException("Phone Number already exists.");
                }

                var student = _mapper.Map<Student>(studentRequest);

                //Upload Student Image
                await ProcessStudentImage(studentRequest, student.StudentId, student);

                _dbContext.Add(student);
                await _dbContext.SaveChangesAsync();

                return new MessageResponse("Student Created Successfully!");
            }
            catch (StudentAlreadyExistsException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Error creating student. Database update failed." + ex);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when creating student." + ex);
            }
        }

        //Delete Student
        public async Task<MessageResponse> DeleteStudentById(Guid studentId)
        {
            try
            {
                var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
                if (existingStudent == null)
                {
                    throw new StudentNotFoundException("Student not found with Id: " + studentId);
                }

                _dbContext.Students.Remove(existingStudent);
                await _dbContext.SaveChangesAsync();

                return new MessageResponse("Student Delete Successfully!");
            }
            catch (StudentNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Error deleting student. Database update failed." + ex);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when deleting student." + ex);
            }
        }

        //List Student
        public async Task<List<StudentDto>> ListOfStudent()
        {
            try
            {
                var studentsWithImages = await _dbContext.Students.Include(s => s.Images).ToListAsync();

                if (studentsWithImages == null || studentsWithImages.Count == 0)
                {
                    throw new StudentNotFoundException("No Students Found.");
                }

                return _mapper.Map<List<StudentDto>>(studentsWithImages);
            }
            catch (StudentNotFoundException)
            {
                throw;
            }        
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when retrieving student." + ex);
            }
        }

        //Student By Id
        public async Task<StudentDto> StudentById(Guid studentId)
        {
            try
            {
                var studentWithImages = await _dbContext.Students.Include(s => s.Images).FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (studentWithImages == null)
                {
                    throw new StudentNotFoundException("Student not found with Id: " + studentId);
                }

                return _mapper.Map<StudentDto>(studentWithImages);
            }
            catch (StudentNotFoundException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when retrieving student." + ex);
            }
        }

        //Update Student Details
        public async Task<MessageResponse> UpdateStudentInfo(StudentRequest studentRequest, Guid studentId)
        {
            try
            {
                var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
                if (existingStudent == null)
                {
                    throw new StudentNotFoundException("Student not found with Id: " + studentId);
                }

                //Update existing student with new data
                _mapper.Map(studentRequest, existingStudent);

                //Update image if provided
                await ProcessStudentImage(studentRequest, studentId,existingStudent);

                await _dbContext.SaveChangesAsync();

                return new MessageResponse("Student Updated Successfully!");
            }
            catch (StudentNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Error updating student. Database update failed." + ex);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when updating student." + ex);
            }
        }

        //Upload Image function
        private async Task ProcessStudentImage(StudentRequest studentRequest, Guid studentId, Student student)
        {
            if (studentRequest.File != null && studentRequest.File.Length > 0)
            {
                // Check if an image already exists for the student
                var existingImage = await _dbContext.Images.FirstOrDefaultAsync(i => i.StudentId == studentId);
                if (existingImage != null)
                {
                    // Update the existing image data
                    using (var ms = new MemoryStream())
                    {
                        studentRequest.File.CopyTo(ms);
                        existingImage.ImageData = ms.ToArray();
                    }
                }
                else
                {
                    // If no existing image, create a new one
                    using (var ms = new MemoryStream())
                    {
                        studentRequest.File.CopyTo(ms);
                        student.Images = new Image
                        {
                            StudentId = studentId,
                            ImageData = ms.ToArray()
                        };
                    }
                }
            }
            else
            {
                // If no new image is provided, handle it accordingly
                if (student.Images != null)
                {
                    _dbContext.Images.Remove(student.Images);
                    student.Images = null;
                }
            }
        }
    }
}
