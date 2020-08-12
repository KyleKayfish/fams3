﻿using Serilog.Core;
using Serilog.Events;
using System;

namespace BcGov.Fams3.Utils.Logger
{
    public class DataPartnerEnricher : ILogEventEnricher
    {
        private readonly string innerPropertyName;

        public DataPartnerEnricher(string innerPropertyName)
        {
            this.innerPropertyName = innerPropertyName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            LogEventPropertyValue eventPropertyValue;
            if (logEvent.Properties.TryGetValue(innerPropertyName, out eventPropertyValue))
            {
                var value = (eventPropertyValue as ScalarValue)?.Value as string;
                if (!String.IsNullOrEmpty(value))
                {
                    logEvent.AddOrUpdateProperty(new LogEventProperty(innerPropertyName, new ScalarValue("- " + value)));
                }
            }
        }
    }
}