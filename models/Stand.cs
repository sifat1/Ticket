using System.Text.Json.Serialization;

namespace ShowTickets.Ticketmodels{
    public class Stand
    {
        public long StandId { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }

        public long VenueId { get; set; }
        [JsonIgnore]
        public Venue Venue { get; set; }

        public ICollection<StandSeat> StandSeats { get; set; }
        public ICollection<ShowSeat> ShowSeats { get; set; }
    }


}
