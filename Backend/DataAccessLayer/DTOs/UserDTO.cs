using Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class UserDTO
    {
        public const string Email = "email";
        public const string Password = "password";
        public const bool IsLoggedIn = false;
        public Dictionary<string,Board> _boards;
    }
}
