using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Entities
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieUserId { get; set; }

        
        
        public string? CommentText { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime CreatedAt { get; set; }


    }
}
