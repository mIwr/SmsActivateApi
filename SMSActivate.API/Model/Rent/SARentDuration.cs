using System;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate rent duration variant (in hours)
    /// </summary>
    public enum SARentDuration: ushort
    {
        /// <summary>
        /// 4 hours
        /// </summary>
        FourHours = 4, 
        /// <summary>
        /// 12 hours
        /// </summary>
        HalfDay = 12, 
        /// <summary>
        /// 24 hours
        /// </summary>
        Day = 24,
        /// <summary>
        /// 48 hours (2 days)
        /// </summary>
        TwoDays = 48, 
        /// <summary>
        /// 72 hours (3 days)
        /// </summary>
        ThreeDays = 72, 
        /// <summary>
        /// 96 hours (4 days)
        /// </summary>
        FourDays = 96,
        /// <summary>
        /// 120 hours (5 days)
        /// </summary>
        FiveDays = 120,
        /// <summary>
        /// 144 hours (6 days)
        /// </summary>
        SixDays = 144, 
        /// <summary>
        /// 168 hours (7 days)
        /// </summary>
        Week = 168,
        /// <summary>
        /// 336 hours (14 days)
        /// </summary>
        TwoWeeks = 336,
        /// <summary>
        /// 504 hours (21 days)
        /// </summary>
        ThreeWeeks = 504, 
        /// <summary>
        /// 672 hours (28 days)
        /// </summary>
        FourWeeks = 672,
        /// <summary>
        /// 840 hours (35 days)
        /// </summary>
        FiveWeeks = 840,
        /// <summary>
        /// 1008 hours (42 days)
        /// </summary>
        SixWeeks = 1008,
        /// <summary>
        /// 1176 hours (49 days)
        /// </summary>
        SevenWeeks = 1176,
        /// <summary>
        /// 1344 hours (56 days)
        /// </summary>
        EightWeeks = 1344
    }

    /// <summary>
    /// SMS Activate rent duration variant extension
    /// </summary>
    public static class SARentDurationExt
    {
        /// <summary>
        /// All variant values
        /// </summary>
        public static SARentDuration[] Values
        {
            get
            {
                return Enum.GetValues<SARentDuration>();
            }
        }

        /// <summary>
        /// Try parse rent duration from hours value
        /// </summary>
        /// <param name="hours">Rent duration in hours</param>
        /// <returns>Optional rent duration variant</returns>
        public static SARentDuration? FromHours(ushort hours)
        {            
            foreach(var rentDuration in Values)
            {
                var itemHours = rentDuration.TotalHours();
                if (hours == itemHours)
                {
                    return rentDuration;
                }
            }
            return null;
        }
        /// <summary>
        /// Try parse rent duration from hours value. If null, returns the closest rent duration variant
        /// </summary>
        /// <param name="hours">Rent duration in hours</param>
        /// <returns>Rent duration variant</returns>
        public static SARentDuration ClosestFromHours(ushort hours)
        {
            var duration = SARentDuration.FourHours;
            var delta = Math.Abs(hours - duration.TotalHours());
            foreach (var rentDuration in Values)
            {
                var itemHours = rentDuration.TotalHours();
                if (hours == itemHours)
                {
                    return rentDuration;
                }
                var cmpDelta = Math.Abs(hours - rentDuration.TotalHours());
                if (cmpDelta < delta)
                {
                    //Ближе по разнице часов к новому значению длительности аренды
                    delta = cmpDelta;
                    duration = rentDuration;
                }
            }

            return duration;
        }
        /// <summary>
        /// Rent duration variant in hours
        /// </summary>
        /// <param name="rentDuration"></param>
        /// <returns></returns>
        public static ushort TotalHours (this SARentDuration rentDuration)
        {
            return (ushort)rentDuration;
        }
        /// <summary>
        /// Rent duration in seconds
        /// </summary>
        /// <param name="rentDuration"></param>
        /// <returns></returns>
        public static long TotalSeconds(this SARentDuration rentDuration)
        {
            var hours = rentDuration.TotalHours();
            long seconds = hours * 3600;

            return seconds;
        }
    }
}
