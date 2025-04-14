public class PolicyModel
{
    public string Number { get; set; }
    public string Reception { get; set; }
    public string Concept { get; set; }
    public string CompanyName { get; set; }
    public string CompanyCuil { get; set; }
    public string Insurer { get; set; }
    public List<StateModel> States { get; set; }
}

public class StateModel
{
    public string Name { get; set; }
    public bool Checked { get; set; }
}