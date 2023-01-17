using System.ComponentModel.DataAnnotations;

namespace MsgFoundation.Models
{
    public class Observation
    {
        [Key]
        public Guid Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ObservationText { get; set; }
        public DateTime Date { get; set; }
        
    }
}
