using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Distance;
using Microsoft.Extensions.Configuration;

namespace ASMS.Services.Services
{
    public class DistanceService :  IDistanceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _googleMapsApiKey;
        private const decimal BASE_COST_PER_KM = 10000; // 10,000 VND per km

        public DistanceService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _googleMapsApiKey = _configuration["GoogleMaps:ApiKey"]
                ?? throw new InvalidOperationException("Google Maps API Key is not configured");
        }

        public async Task<DistanceCalculationResponse> CalculateDistanceAsync(DistanceCalculationRequest request)
        {
            try
            {
                // Build Google Maps Distance Matrix API URL
                var url = BuildDistanceMatrixUrl(request.Origin, request.Destination);

                // Call Google Maps API
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var googleResponse = JsonSerializer.Deserialize<GoogleDistanceMatrixResponse>(jsonResponse);

                // Validate response
                if (googleResponse == null || googleResponse.Status != "OK")
                {
                    throw new Exception($"Google Maps API returned status: {googleResponse?.Status ?? "NULL"}");
                }

                if (googleResponse.Rows.Count == 0 || googleResponse.Rows[0].Elements.Count == 0)
                {
                    throw new Exception("No route found between origin and destination");
                }

                var element = googleResponse.Rows[0].Elements[0];

                if (element.Status != "OK" || element.Distance == null || element.Duration == null)
                {
                    throw new Exception($"Cannot calculate distance. Status: {element.Status}");
                }

                // Extract distance and duration
                var distanceInMeters = element.Distance.Value;
                var distanceInKm = distanceInMeters / 1000.0;
                var durationInSeconds = element.Duration.Value;
                var durationInMinutes = (int)Math.Ceiling(durationInSeconds / 60.0);

                // Calculate estimated cost
                var estimatedCost = CalculateEstimatedCost(distanceInKm);

                return new DistanceCalculationResponse
                {
                    Origin = googleResponse.OriginAddresses.FirstOrDefault() ?? request.Origin,
                    Destination = googleResponse.DestinationAddresses.FirstOrDefault() ?? request.Destination,
                    DistanceInMeters = distanceInMeters,
                    DistanceInKm = Math.Round(distanceInKm, 2),
                    DurationInSeconds = durationInSeconds,
                    DurationInMinutes = durationInMinutes,
                    EstimatedCost = estimatedCost,
                    Status = "SUCCESS"
                };
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error calling Google Maps API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating distance: {ex.Message}", ex);
            }
        }

        private string BuildDistanceMatrixUrl(string origin, string destination)
        {
            var baseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json";
            var encodedOrigin = Uri.EscapeDataString(origin);
            var encodedDestination = Uri.EscapeDataString(destination);

            return $"{baseUrl}?origins={encodedOrigin}&destinations={encodedDestination}&key={_googleMapsApiKey}&language=vi";
        }

        private decimal CalculateEstimatedCost(double distanceInKm)
        {
            // Simple pricing formula: Base cost per km
            // You can customize this based on your business rules
            var cost = (decimal)distanceInKm * BASE_COST_PER_KM;

            // Add minimum charge (e.g., 20,000 VND)
            if (cost < 20000)
                cost = 20000;

            // Round to nearest 1000 VND
            return Math.Round(cost / 1000) * 1000;
        }
    }
}
