namespace ChampionsLeagueTickets.Services.DTOs;

public class HotelOfferDTO {
    public string HotelName { get; set; } = string.Empty;
    public double? ReviewScore { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
}