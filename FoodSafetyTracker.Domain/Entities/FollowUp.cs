using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Entities.Enums;

namespace FoodSafetyTracker.Domain.Entities
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        public int InspectionId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public FollowUpStatus Status { get; set; }

        public DateTime? ClosedDate { get; set; }

        public Inspection? Inspection { get; set; }
    }
}