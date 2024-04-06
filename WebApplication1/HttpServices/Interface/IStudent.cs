using WebApplication1.DTO;

namespace WebApplication1.HttpServices.Interface
{
    public interface IStudent
    {
        //Create Student Request
        Task<string> CreateStudent(StudentDto student);

        //Get all studunt Request
        Task<List<StudentDto>> ListOfStudent();

        //Get Student by Id
        Task<StudentDto> GetStudent(Guid studentId);

        //Update the Student
        Task<string> UpdateStudent(StudentDto student);

        //Delete Student
        Task<string> DeleteStudent(Guid studentId);
    }
}
