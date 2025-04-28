using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Policy
{
    [Key]
    public string? Number { get; set; }
    [Required]
    public string? Reception { get; set; }
    [Required]
    public string? Concept { get; set; }
    [Required]
    public string? CompanyName { get; set; }
    [Required]
    public string? CompanyCuil { get; set; }
    [Required]
    public string? Insurer { get; set; }
    [Required]
    public List<State>? States { get; set; }
}

[Table("policy_states")]
public class State
{
    public string? PolicyNumber { get; set; }
    public string? Name { get; set; }
    public bool Checked { get; set; }
}