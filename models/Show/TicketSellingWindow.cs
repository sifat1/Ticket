using System.ComponentModel.DataAnnotations;
using ShowTickets.Ticketmodels;

namespace ShowTickets.Ticketmodels
{
    public class TicketSellingWindow
    {
        public long TicketSellingWindowId { get; set; }
        public long ShowId { get; set; }
        public Show Show { get; set; }

        public DateTime startdate { get; set; }

        public DateTime enddate { get; set; }

    }
}