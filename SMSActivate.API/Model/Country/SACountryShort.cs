namespace SmsActivate.API.Model
{
    /// <summary>
    /// Country short info
    /// </summary>
    public class SACountryShort
    {
        /// <summary>
        /// Country ID
        /// </summary>
        public readonly ushort ID;
        /// <summary>
        /// Country name
        /// </summary>
        public readonly string Name;

        public SACountryShort(ushort id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString()
        {
            return ID.ToString() + " - " + Name;
        }
    }
}
