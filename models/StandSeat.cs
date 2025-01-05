namespace ShowTickets.Ticketmodels{
    public class StandSeat
    {
        public long StandSeatId { get; set; }
        public string SeatNumber { get; set; }

        public long StandId { get; set; }
        public Stand Stand { get; set; }
    }
}
