namespace EduApoyos.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string FullName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public Student? Student { get; private set; }

        private User() { }

        public User(string fullName, string email, string passwordHash, UserRole role)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Role = role;
        }

        public void SetId(Guid id) => Id = id;
        public void UpdatePasswordHash(string hash) =>
            PasswordHash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    public enum UserRole { Advisor = 1, Student = 2 }
}
