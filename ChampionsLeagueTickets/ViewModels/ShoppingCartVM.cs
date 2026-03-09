namespace ChampionsLeagueTickets.ViewModels;
public class ShoppingCartVM {
    public List<TicketVM> Tickets { get; set; } = [];
}

public abstract class TicketVM {
    public int SectionId { get; set; }
    public string? ClubName { get; set; }
    public string? Ring { get; set; }
    public string? Location { get; set; }
    public double Price { get; set; }
    public DateTime DateCreated { get; set; }
    public abstract bool IsValidOn(DateTime date);
}

public class SeasonTicketVM : TicketVM{
    public override bool IsValidOn(DateTime date) {
        return true;
    }
}

public class DayTicketVM : TicketVM {
    public DateTime ValidDate { get; set; }

    public override bool IsValidOn(DateTime date) {
        return date.Date == ValidDate.Date;
    }
}

