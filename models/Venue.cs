namespace ShowTickets.Ticketmodels{
    public class Venue
    {
        public long VenueId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int TotalStands { get; set; }

        public ICollection<Stand> Stands { get; set; }
        public ICollection<ShowSeat> ShowSeats { get; set; }
    }

}
