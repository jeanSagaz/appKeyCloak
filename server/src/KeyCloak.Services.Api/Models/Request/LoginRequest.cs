using System.ComponentModel.DataAnnotations;

namespace KeyCloak.Services.Api.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Nome de usuário deve ser informado.")]
        public string UserName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Senha deve ser informado.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
