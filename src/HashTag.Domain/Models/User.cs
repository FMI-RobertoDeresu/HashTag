namespace HashTag.Domain.Models
{
    public class User : EntityBase<long>
    {
        protected User() { }

        public User(ApplicationUser applicationUser)
        {
            ApplicationUser = applicationUser;
        }

        public ApplicationUser ApplicationUser { get; set; }

        public string UserName => ApplicationUser?.UserName;

        public Photo ProfilePhoto { get; set; }
    }
}