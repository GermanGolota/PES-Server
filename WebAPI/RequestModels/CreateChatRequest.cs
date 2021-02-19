using System.ComponentModel.DataAnnotations;

namespace WebAPI.RequestModels
{
    public class CreateChatRequest
    {
        [Required]
        public string ChatName { get; set; }
    }
}
