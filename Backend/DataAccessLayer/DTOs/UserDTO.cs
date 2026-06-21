namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// Represents one row in the Users table.
    /// A DTO (Data Transfer Object) is a plain data holder with no logic —
    /// it just carries information between the DB and the BL.
    /// </summary>
    public class UserDTO
    {
        public string Email { get; }
        public string Password { get; }

        public UserDTO(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
