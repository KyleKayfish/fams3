﻿using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchRejected : PersonSearchRejected
    {
        public IEnumerable<ValidationResult> Reasons {get;set;}

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}