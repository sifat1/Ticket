using ShowTickets.Ticketmodels;

namespace ShowTickets.Ticketmodels
{
    public class TicketSellingWindow
    {
        public long TicketSellingWindowID { get; set; }
        public long ShowId { get; set; }
        public Show show { get; set; }

        public DateTime startdate { get; set; }

        public DateTime enddate { get; set; }

    }
}