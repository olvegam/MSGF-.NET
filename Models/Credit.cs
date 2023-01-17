using System.ComponentModel.DataAnnotations;

namespace MsgFoundation.Models
{
    public class Credit
    {
        [Key]
        public Guid Id { get; set; }
        public string IdUser { get; set; }
        public double CreditValue { get; set; }
        public DateTime Date { get; set; }
        public string StatusCredit { get; set; }
        public string Disbursed { get; set; }
    }
}
