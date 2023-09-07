using System.ComponentModel.DataAnnotations;

namespace JobManagements.Models
{
    public class UserModel
    {
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Compare("password",ErrorMessage="Errors pasword")]
        [DataType(DataType.Password)]
        public string confirmPassword { get; set; }
    }
}
