namespace hm4
{
    public class UTCClock : IClock
    {
        public DateTime ShowClock()
        {
            return DateTime.Now;
        }
    }
}
