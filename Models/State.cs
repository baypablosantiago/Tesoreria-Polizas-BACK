using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("policy_states")]
public class State
{
    [Required] [MaxLength(50)]
    public string? PolicyNumber { get; set; }


    [Required] [MaxLength(255)]
    public string? Name { get; set; }

    
    [Required]
    public bool Checked { get; set; }
}