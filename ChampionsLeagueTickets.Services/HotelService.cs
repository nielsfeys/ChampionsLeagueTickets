using ChampionsLeagueTickets.Services.DTOs;
using ChampionsLeagueTickets.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace ChampionsLeagueTickets.Services;
public class HotelService : IHotelService {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiHost;

    public HotelService(HttpClient httpClient, IConfiguration configuration) {
        _httpClient = httpClient;
        _apiKey = configuration["RapidApi:ApiKey"]!;
        _apiHost = configuration["RapidApi:BookingHost"]!;
        _httpClient.BaseAddress = new Uri($"https://{_apiHost}");
        _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", _apiHost);
    }

    public async Task<List<HotelOfferDTO>?> SearchHotelsAsync(int cityId, DateOnly checkIn, DateOnly checkOut, int adults, int roomQuantity) {
        var searchUrl = $"/api/v1/hotels/searchHotels" +
                        $"?dest_id={cityId}" +
                        $"&search_type=CITY" +
                        $"&arrival_date={checkIn:yyyy-MM-dd}" +
                        $"&departure_date={checkOut:yyyy-MM-dd}" +
                        $"&adults={adults}" +
                        $"&room_qty={roomQuantity}" +
                        $"&currency_code=EUR";

        var response = await _httpClient.GetAsync(searchUrl);

        if (response.StatusCode == HttpStatusCode.TooManyRequests) return null;

        if (!response.IsSuccessStatusCode) return [];

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var results = new List<HotelOfferDTO>();

        if (!doc.RootElement.TryGetProperty("data", out var data) || !data.TryGetProperty("hotels", out var hotels)) {
            return results;
        }

        foreach (var hotel in hotels.EnumerateArray()) {
            var offer = new HotelOfferDTO {
                HotelName = hotel.TryGetProperty("property", out var prop)
                    ? prop.GetProperty("name").GetString() ?? "Unknown"
                    : "Unknown"
            };

            if (prop.ValueKind != JsonValueKind.Undefined) {
                if (prop.TryGetProperty("reviewScore", out var score))
                    offer.ReviewScore = score.ValueKind == JsonValueKind.Number ? score.GetDouble() : null;

                if (prop.TryGetProperty("priceBreakdown", out var priceInfo) &&
                    priceInfo.TryGetProperty("grossPrice", out var gross) &&
                    gross.TryGetProperty("value", out var priceVal))
                    offer.Price = priceVal.GetDecimal();

                offer.Currency = "EUR";
            }

            results.Add(offer);
        }

        return results;
    }
}
