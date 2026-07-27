namespace EduApoyos.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string DocumentNumber { get; private set; } = null!;
        public string DocumentType { get; private set; } = null!;
        public string AcademicProgram { get; private set; } = null!;
        public int Semester { get; private set; }
        public User User { get; private set; } = null!;
        public ICollection<SupportRequest> SupportRequests { get; private set; } = new List<SupportRequest>();

        private Student() { }

        public Student(Guid userId, string documentNumber, string documentType, string academicProgram, int semester)
        {
            UserId = userId;
            DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            AcademicProgram = academicProgram ?? throw new ArgumentNullException(nameof(academicProgram));
            if (semester <= 0) throw new ArgumentOutOfRangeException(nameof(semester));
            Semester = semester;
        }
    }
}
