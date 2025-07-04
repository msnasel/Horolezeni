namespace Horolezeni.Models
{
    /// <summary>
    /// Represents a single climbing event or action.
    /// </summary>
    public class Akce
    {
        public string Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; } = DateTime.Now.AddHours(2);
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Transportation { get; set; } = string.Empty;
        public string Participate { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Manual status, e.g., for cancellation.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// A read-only property that calculates the current status based on dates.
        /// It prioritizes the manual "Zrušeno" status.
        /// </summary>
        public string CalculatedStatus
        {
            get
            {
                // Ručně nastavený stav "Zrušeno" má nejvyšší prioritu.
                if (!string.IsNullOrEmpty(Status) && Status.Equals("Zrušeno", StringComparison.OrdinalIgnoreCase))
                {
                    return "Zrušeno";
                }

                var now = DateTime.Now;
                if (End < now)
                {
                    return "Dokončeno";
                }
                if (Start <= now && End >= now)
                {
                    return "Probíhá";
                }
                if (Start > now)
                {
                    return "Naplánováno";
                }

                // Záložní stav, pokud se nepodaří určit.
                return "Neznámý stav";
            }
        }
    }
}