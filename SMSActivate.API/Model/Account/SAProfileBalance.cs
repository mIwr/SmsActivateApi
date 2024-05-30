namespace SmsActivate.API.Model
{
    /// <summary>
    /// Account balance info
    /// </summary>
    public class SAProfileBalance
    {
        /// <summary>
        /// Account balance value
        /// </summary>
        public readonly double Balance;
        /// <summary>
        /// Account cashback (ruble)
        /// </summary>
        public readonly double Cashback;

        /// <summary>
        /// Available account balance (Balance + Cashback)
        /// </summary>
        public double Total
        {
            get
            {
                return Balance + Cashback;
            }
        }
        
        /// <summary>
        /// Initializes account balance instance
        /// </summary>
        /// <param name="cashback">Account cashback value</param>
        /// <param name="balance">Account balance value</param>
        public SAProfileBalance(double cashback, double balance)
        {
            Cashback = cashback;
            Balance = balance;
        }

        public override string? ToString()
        {
            return "Balance = " + Balance.ToString() + "; Cashback = " + Cashback.ToString();
        }
    }
}
