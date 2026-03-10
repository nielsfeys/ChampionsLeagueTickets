using Microsoft.Identity.Client;

namespace ChampionsLeagueTickets.ViewModels;
public class StadiumVM {
    public List<SectionVM> SectionVMs { get; set; } = [];
    public int TotalSeats() => SectionVMs.Sum(s => s.Seats);
}

public class SectionVM {
    public int Id { get; set; }
    public string? HomeTeam { get; set; }
    public string? Ring { get; set; }
    public string? Location { get; set; }
    public double Price { get; set; }
    public int Seats { get; set; }
}

