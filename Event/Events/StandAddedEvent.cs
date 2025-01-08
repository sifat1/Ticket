namespace Ticket.Events
{
    public class StandAddedEvent
    {
        public long StandId { get; set; }
        public int SeatCount { get; set; }
    }
}