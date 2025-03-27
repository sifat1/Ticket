using System.ComponentModel.DataAnnotations;
using ShowTickets.Ticketmodels.User;

namespace ShowTickets.Ticketmodels
{
    public class ShowSeat
    {
        public long ShowSeatId { get; set; }
        public bool IsBooked { get; set; }
        public DateTime? BookingTime { get; set; }
        public long? UserId { get; set; }
        public Users User { get; set; }

        public long ShowId { get; set; }
        public Show Show { get; set; }

        public long VenueId { get; set; }
        public Venue Venue { get; set; }

        public long StandId{ get; set; }
        public Stand Stand { get; set; }

        public long StandSeatId { get; set; }
        public StandSeat StandSeat { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }



}
