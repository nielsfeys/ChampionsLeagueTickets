using System.ComponentModel.DataAnnotations;

namespace ChampionsLeagueTickets.ViewModels {
    public class HotelSearchVM {
        [Required]
        public int Destination { get; set; }
        
        [Required]
        public DateOnly CheckIn { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        [Required]
        public DateOnly CheckOut { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

        [Required]
        public int Adults { get; set; } = 1;

        [Required]
        public int RoomQuantity { get; set; } = 1;
        public List<HotelOfferVM> Results { get; set; } = [];

    }

    public class HotelOfferVM {
        public string HotelName { get; set; } = string.Empty;
        public double? ReviewScore { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EUR";
    }
}
