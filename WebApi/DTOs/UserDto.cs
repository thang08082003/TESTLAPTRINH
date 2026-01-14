using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Mã là bắt buộc")]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Điện thoại là bắt buộc")]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;
    }

    public class UpdateUserDto : CreateUserDto
    {
        [Required]
        public int Id { get; set; }
    }
}