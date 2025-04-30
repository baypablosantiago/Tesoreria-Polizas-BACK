using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("policies")]
public class Policy
{
    [Key] [MaxLength(50)]
    public string? Number { get; set; }


    [Required] [MaxLength(50)]
    public string? Reception { get; set; }


    [Required] [MaxLength(500)]
    public string? Concept { get; set; }


    [Required] [MaxLength(50)]
    public string? CompanyName { get; set; }


    [Required] [MaxLength(50)]
    public string? CompanyCuil { get; set; }


    [Required] [MaxLength(50)]
    public string? Insurer { get; set; }
    

    [Required]
    public List<State>? States { get; set; }


    [Required]
    public bool? Endorsements { get; set; }
}