namespace ShowTickets.Ticketmodels{
    public class Venue
    {
        public int VenueId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int TotalStands { get; set; }

        public ICollection<Stand> Stands { get; set; }
    }

}
