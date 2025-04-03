using System.Reflection.Metadata;

namespace AuthorizationService
{
    public class CreateUserDTO : UserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } 
    }
}
