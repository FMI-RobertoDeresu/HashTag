namespace HashTag.Domain.Dtos
{
    public class UserUpdateDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}