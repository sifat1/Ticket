namespace ShowTickets.Ticketmodels{
    public class VenueSeat
    {
        public long SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public long VenueId { get; set; }
        public Venue Venue { get; set; }
    }
}
