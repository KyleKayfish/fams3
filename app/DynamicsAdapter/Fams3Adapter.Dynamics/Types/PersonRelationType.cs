﻿using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class PersonRelationType : Enumeration
    {
        public static PersonRelationType Spouse = new PersonRelationType(867670000, "Spouse");
        public static PersonRelationType AuntUncle = new PersonRelationType(867670002, "Aunt/Uncle");
        public static PersonRelationType Parent = new PersonRelationType(867670001, "Parent");
        public static PersonRelationType Child = new PersonRelationType(867670003, "Child");
        public static PersonRelationType Sibling = new PersonRelationType(867670004, "Sibling");
        public static PersonRelationType Cousin = new PersonRelationType(867670005, "Cousin");
        public static PersonRelationType Friend = new PersonRelationType(867670006, "Friend");
        protected PersonRelationType(int value, string name) : base(value, name)
        {

        }
    }

}
