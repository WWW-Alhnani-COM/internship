using System.ComponentModel.DataAnnotations;

namespace InternshipManagement.API.DTOs.Admin;

public class UpdateUserRoleDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [RegularExpression("^(Student|SiteSupervisor|AcademicSupervisor|Admin)$")]
    public string NewRole { get; set; } = "Student";
}