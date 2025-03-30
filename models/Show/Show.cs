namespace ShowTickets.Ticketmodels{
    public class Show
    {
        public long ShowId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime Date { get; set; }

        public string? ThumnileFilePath { get; set; }
        public long VenueId { get; set; }
        public Venue Venue { get; set; }


        public ICollection<ShowSeat> ShowSeats { get; set; }

        public TicketSellingWindow ticketSellingWindow{ get; set; }
    }


}