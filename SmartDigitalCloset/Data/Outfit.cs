using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SmartDigitalCloset.Data
{
    public class Outfit
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter an outfit name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select at least one tag")]
        [StringLength(100, ErrorMessage = "Tags cannot exceed 100 characters")]
        public string Tags { get; set; }

        [Required(ErrorMessage = "Please select a planned date")]
        public DateTime PlannedDate { get; set; }

        [Required(ErrorMessage = "Please select at least one item")]
        public string ItemIds { get; set; }

        public DateTime CreatedAt { get; set; }

        // Helper methods for ItemIds conversion
        public List<int> GetItemIdsList()
        {
            if (string.IsNullOrEmpty(ItemIds))
                return new List<int>();
            return JsonSerializer.Deserialize<List<int>>(ItemIds);
        }

        public void SetItemIdsList(List<int> ids)
        {
            ItemIds = JsonSerializer.Serialize(ids);
        }
    }
} 