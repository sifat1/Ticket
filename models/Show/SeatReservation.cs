using System.ComponentModel.DataAnnotations;

namespace ShowTickets.Ticketmodels
{
    public class SeatReservation
    {

        public long SeatReservationId { get; set; }

        public long ShowSeatId { get; set; }
        public ShowSeat ShowSeat { get; set; }
        public decimal Price { get; set; }

        public long ShowId { get; set; }
        public Show Show { get; set; }

        public long VenueId { get; set; }
        public Venue Venue { get; set; }

        public long StandId { get; set; }
        public Stand Stand { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsPaid { get; set; }
        public long UserId { get; set; }    

        public byte[] RowVersion { get; set; }
    }
}