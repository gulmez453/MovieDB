using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Entities
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MovieUserId { get; set; }

        
        
        public string? CommentText { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime CreatedAt { get; set; }


    }
}
