using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gRPCServer.Entities
{
    [Table("LoginDetails")]
    public class LoginDetails
    {

        [Key] public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

    }
}
