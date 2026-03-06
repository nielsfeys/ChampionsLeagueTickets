using Microsoft.Identity.Client;

namespace ChampionsLeagueTickets.ViewModels;
public class StadiumVM {
    public List<SectionVM> SectionVMs { get; set; } = [];
    public int TotalSeats() => SectionVMs.Sum(s => s.Seats);
}

public class SectionVM {
    public string? HomeTeam { get; set; }
    public string? SectionName { get; set; }
    public int Seats { get; set; }
}

