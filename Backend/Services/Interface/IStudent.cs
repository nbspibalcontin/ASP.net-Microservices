using Backend.DTOs.Response;
using StudentService.DTOs.Request;
using StudentService.DTOs.Response;

namespace Backend.Services.Interface
{
    public interface IStudent
    {
        //Create Student
        Task<MessageResponse> CreateStudent(StudentRequest studentRequest);

        //List of Student
        Task<List<StudentDto>> ListOfStudent();

        //Student by Id
        Task<StudentDto> StudentById(Guid studentId);

        //Update Student Info
        Task<MessageResponse> UpdateStudentInfo(StudentRequest studentRequest, Guid studentId);

        //Delete Student
        Task<MessageResponse> DeleteStudentById(Guid studentId);
    }
}
