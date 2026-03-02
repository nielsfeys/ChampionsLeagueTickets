namespace ChampionsLeagueTickets.ViewModels;
public class MatchVM {
    public int Id { get; set; }
    public string? HomeTeam {  get; set; }
    public string? AwayTeam { get; set; }
    public DateOnly? Date {  get; set; }
}

