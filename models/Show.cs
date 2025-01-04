namespace ShowTickets.Ticketmodels{
    public class Show
    {
        public int ShowId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public int VenueId { get; set; }
        public Venue Venue { get; set; }

        public ICollection<ShowSeat> ShowSeats { get; set; }
    }


}