using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.RequestModels
{
    public class PostMessageRequest
    {
        public Guid ChatId { get; set; }
        public string Message { get; set; }
    }
}
