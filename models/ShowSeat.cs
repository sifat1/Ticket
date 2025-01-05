using System.ComponentModel.DataAnnotations;

namespace ShowTickets.Ticketmodels
{
    public class ShowSeat
    {
        public long ShowSeatId { get; set; }
        public bool IsBooked { get; set; }
        public DateTime? BookingTime { get; set; }
        public string? UserId { get; set; }

        public long ShowId { get; set; }
        public Show Show { get; set; }

        public long StandSeatId { get; set; }
        public StandSeat StandSeat { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }



}
