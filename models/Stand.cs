namespace ShowTickets.Ticketmodels{
    public class Stand
    {
        public int StandId { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }

        public int VenueId { get; set; }
        public Venue Venue { get; set; }

        public ICollection<StandSeat> StandSeats { get; set; }
    }


}
