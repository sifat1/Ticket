    public class BookedSeatDTO
    {
        public long ShowSeatId { get; set; }
        public long ShowId { get; set; }
        public string ShowName { get; set; }
        public string ShowDate { get; set; }
        public string ShowTime { get; set; }
        public string Venuename { get; set; }
        public string Venuelocation { get; set; }
        public string StandName { get; set; }
        public long StandSeatId { get; set; }
        public string Token { get; set; }
        public bool IsTokenUsed { get; set; }
        public DateTime? TokenUsedTime { get; set; }
        public DateTime? BookingTime { get; set; }
        public bool IsBooked { get; set; }
    }