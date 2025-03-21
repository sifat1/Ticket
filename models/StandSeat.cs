using System.Text.Json.Serialization;

namespace ShowTickets.Ticketmodels{
    public class StandSeat
    {
        public long StandSeatId { get; set; }
        public string SeatNumber { get; set; }

        public long StandId { get; set; }
        [JsonIgnore]
        public Stand Stand { get; set; }

        public ICollection<ShowSeat> ShowSeats { get; set; }
    }
}
