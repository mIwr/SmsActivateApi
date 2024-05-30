using System.Collections.Generic;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate activation service offer for defined country
    /// </summary>
    public class SAActivationServiceOffer
    {
        /// <summary>
        /// Contry ID
        /// </summary>
        public readonly ushort CountryID;
        /// <summary>
        /// Service short name (ID)
        /// </summary>
        public readonly string ServiceID;
        /// <summary>
        /// Total activations count
        /// </summary>
        public readonly uint Total;
        /// <summary>
        /// Available activations count
        /// </summary>
        public readonly uint Count;
        /// <summary>
        /// Default price
        /// </summary>
        public readonly double DefaultPrice;
        /// <summary>
        /// Default activation cost (retail price)
        /// </summary>
        public readonly double RetailPrice;

        /// <summary>
        /// Offer pairs: key is the price, value means the available activations quantity for this price
        /// </summary>
        public KeyValuePair<double, uint>[] Prices;

        /// <summary>
        /// Min price offer
        /// </summary>
        public KeyValuePair<double, uint> MinPriceQuantity
        {
            get
            {
                var offer = new KeyValuePair<double, uint>(key: double.MaxValue, value: 0);
                foreach (var entry in Prices)
                {
                    if (entry.Value == 0 || entry.Key >= offer.Key)
                    {
                        continue;
                    }
                    offer = entry;
                }
                return offer;
            }
        }

        /// <summary>
        /// Max price offer
        /// </summary>
        public KeyValuePair<double, uint> MaxPriceQuantity
        {
            get
            {
                var offer = new KeyValuePair<double, uint>(key: 0, value: 0);
                foreach (var entry in Prices)
                {
                    if (entry.Value == 0 || entry.Key <= offer.Key)
                    {
                        continue;
                    }
                    offer = entry;
                }
                return offer;
            }
        }       

        public SAActivationServiceOffer(ushort countryID, string serviceID, uint total, uint count, double defaultPrice, double retailPrice, KeyValuePair<double, uint>[] prices)
        {
            CountryID = countryID;
            ServiceID = serviceID;
            Total = total;
            Count = count;
            DefaultPrice = defaultPrice;
            RetailPrice = retailPrice;
            Prices = prices;
        }
    }
}
