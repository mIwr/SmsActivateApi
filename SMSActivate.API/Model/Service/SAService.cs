
namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate general service info
    /// </summary>
    public class SAService
    {
        /// <summary>
        /// Service short name (ID)
        /// </summary>
        public readonly string ID;
        /// <summary>
        /// Service name
        /// </summary>
        public readonly string Name;

        public SAService(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString()
        {
            return ID.ToString() + " - " + Name.ToString();
        }
    }
}
