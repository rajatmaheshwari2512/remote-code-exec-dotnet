using System.ComponentModel.DataAnnotations;

namespace RceDotNet.API.Models
{
    public class ExecuteCodeRequest
    {
        [Required]
        public string Code { get; set; }
        [Required]
        [MaxLength(4,ErrorMessage ="Language can be at most 4 characters long")]
        public string Language { get; set; }
        public string? Input { get; set; }
    }
}
