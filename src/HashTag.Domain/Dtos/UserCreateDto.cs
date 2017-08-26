namespace HashTag.Domain.Dtos
{
    public class UserCreateDto
    {
        public string Email { get; protected set; }

        public string UserName { get; protected set; }

        public string Password { get; protected set; }

        public string RePassword { get; protected set; }
    }
}