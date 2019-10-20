namespace Payments.Service.Models
{    
    public class Payment
    {
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string Name { get; set; }
    }
}