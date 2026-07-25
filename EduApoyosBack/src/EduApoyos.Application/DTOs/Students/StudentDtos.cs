namespace EduApoyos.Application.DTOs.Students
{
    public record StudentDto(Guid Id, Guid UserId, string FullName, string Email, string DocumentNumber, string DocumentType, string AcademicProgram, int Semester);
    public record CreateStudentRequest(string FullName, string Email, string Password, string DocumentNumber,  string DocumentType, string AcademicProgram, int Semester);
}
