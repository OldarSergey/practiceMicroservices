﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatementMicroservice.Application.DTOs.MessageKafka
{
    public class UserDeleteDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
