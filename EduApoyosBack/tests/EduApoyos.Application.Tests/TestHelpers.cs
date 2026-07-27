using EduApoyos.Domain.Entities;
using System.Reflection;

namespace EduApoyos.Application.Tests
{
    internal static class TestHelpers
    {
        public static void SetPrivateProperty<T>(T target, string propertyName, object? value)
        {
            var prop = typeof(T).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Property {propertyName} not found");
            prop.SetValue(target, value);
        }

        public static Student CreateStudentWithUser(
            Guid? studentId = null,
            Guid? userId = null,
            string fullName = "Ana Student",
            string email = "student@test.com")
        {
            var uid = userId ?? Guid.NewGuid();
            var user = new User(fullName, email, "hash", UserRole.Student);
            user.SetId(uid);

            var student = new Student(uid, "1234567890", "CC", "Systems Engineering", 6);
            if (studentId.HasValue)
                SetPrivateProperty(student, nameof(Student.Id), studentId.Value);
            SetPrivateProperty(student, nameof(Student.User), user);
            return student;
        }

        public static SupportRequest CreateSupportRequest(Guid studentId)
            => new(studentId, SupportType.Scholarship, 1_500_000m, "Need support for tuition");
    }
}
