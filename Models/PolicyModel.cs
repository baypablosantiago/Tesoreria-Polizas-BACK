using System.ComponentModel.DataAnnotations;

public class Policy
{
    [Key]
    public string Number { get; set; }
    [Required]
    public string Reception { get; set; }
    [Required]
    public string Concept { get; set; }
    [Required]
    public string CompanyName { get; set; }
    [Required]
    public string CompanyCuil { get; set; }
    [Required]
    public string Insurer { get; set; }
    [Required]
    public List<StateModel> States { get; set; }
}

public class StateModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public bool Checked { get; set; }
}