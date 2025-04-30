using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("endorsements")]
public class Endorsement
{
    [Key] [MaxLength(50)]
    public string? OriginalPolicy { get; set; }


    [Required] [MaxLength(50)]
    public string? Reception { get; set; }


    [Required] [MaxLength(500)]
    public string? NewConcept { get; set; }
}