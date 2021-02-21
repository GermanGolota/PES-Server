using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.RequestModels
{
    public class DeleteMessageRequest
    {
        public Guid ChatId { get; set; }
    }
}
