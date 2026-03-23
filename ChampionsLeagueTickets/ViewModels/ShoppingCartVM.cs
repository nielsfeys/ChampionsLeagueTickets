namespace ChampionsLeagueTickets.ViewModels;
public class ShoppingCartVM {
    public List<SeasonTicketVM> SeasonTickets { get; set; } = [];
    public List<DayTicketVM> DayTickets { get; set; } = [];

    public decimal TotalPrice => 
        SeasonTickets.Sum(st => st.Price ?? 0) +
        DayTickets.Sum(dt => (dt.Price ?? 0) * dt.Quantity);
}

public abstract class TicketVM {
    public int SectionId { get; set; }
    public string? HomeClubName { get; set; }
    public string? Ring { get; set; }
    public string? Location { get; set; }
    public decimal? Price { get; set; }
    public DateOnly DateCreated { get; set; }
}

public class SeasonTicketVM : TicketVM{
    public static readonly DateOnly SeasonStart = DateOnly.FromDateTime(new(2026, 5, 30));
    public static readonly int PriceMultiplier = 10;

}

public class DayTicketVM : TicketVM {
    public int Quantity { get; set; }
    public int MatchId { get; set; }
    public string? AwayClubName { get; set; }
    public DateOnly MatchDate { get; set; }
}

