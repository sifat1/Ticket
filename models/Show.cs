namespace ShowTickets.Ticketmodels{
    public class Show
    {
        public long ShowId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public long VenueId { get; set; }
        public Venue Venue { get; set; }

        public ICollection<ShowSeat> ShowSeats { get; set; }
    }


}