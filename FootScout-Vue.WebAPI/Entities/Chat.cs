using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootScout_Vue.WebAPI.Entities
{
    // Model (encja) chat roomu
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string User1Id { get; set; }

        [ForeignKey("User1Id")]
        public virtual User User1 { get; set; }

        [Required]
        public string User2Id { get; set; }

        [ForeignKey("User2Id")]
        public virtual User User2 { get; set; }
    }
}