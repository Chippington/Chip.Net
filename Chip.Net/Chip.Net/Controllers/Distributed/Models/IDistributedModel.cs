﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Models
{
    public interface IDistributedModel : ISerializable
    {
		int Id { get; set; }
    }
}
