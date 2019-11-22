﻿using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class InformationSourceType : Enumeration
    {

        public static InformationSourceType Request = new InformationSourceType(867670000, "Request");
        public static InformationSourceType ICBC = new InformationSourceType(867670001, "ICBC");
        public static InformationSourceType Employer = new InformationSourceType(867670002, "Employer");


        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
}
