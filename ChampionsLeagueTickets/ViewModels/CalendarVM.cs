namespace ChampionsLeagueTickets.ViewModels;
public class CalendarVM {
    public int TeamId {  get; set; }
    public string? TeamName { get; set; }
    public List<MatchVM>? Matches { get; set;  }

}

