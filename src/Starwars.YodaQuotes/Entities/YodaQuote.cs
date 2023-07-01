namespace Starwars.YodaQuotes.Entities
{
    public class YodaQuote
    {
        public string Quote { get; set; }
        public DateTime Timestamp { get; set; }

        public YodaQuote()
        {
            Timestamp = DateTime.Now;
        }

        public YodaQuote(string quote)
        {
            Quote = quote;
            Timestamp = DateTime.Now;
        }
    }
}
