using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace hm8
{
    public class EmailSenderConfig
    {
        [Required]
        public string Host { get; set; }
        [Range(25, 25)]
        public int Port { get; set; }
        [EmailAddress]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool UseSsl { get; set; }
        [StringLength(254, MinimumLength = 1)]
        public string EmailText { get; set; }
    }
}
