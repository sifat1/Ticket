using System.ComponentModel.DataAnnotations;

namespace ShowTickets.Ticketmodels
{
    public class ShowStandPrice
    {
        public long ShowStandPriceId { get; set; }
        public decimal Price { get; set; }

        public long ShowId { get; set; }
        public Show Show { get; set; }

        public long VenueId { get; set; }
        public Venue Venue { get; set; }

        public long StandId { get; set; }
        public Stand Stand { get; set; }
    }
}