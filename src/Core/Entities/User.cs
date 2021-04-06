using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [MaxLength(100)]
        public string Username { get; set; }
        [MaxLength(40)]
        public string PasswordHash { get; set; }
        [MaxLength(40)] // would be used to calculate users PES Score
        public string PESKey { get; set; }
    }
}
