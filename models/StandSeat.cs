namespace ShowTickets.Ticketmodels{
    public class StandSeat
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }

        public int StandId { get; set; }
        public Stand Stand { get; set; }
    }
}
