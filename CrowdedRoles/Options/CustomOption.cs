﻿namespace CrowdedRoles.Options
{
    public abstract class CustomOption
    {
        public string Name { get; }
        protected internal string ValueText { get; protected set; } = "None";

        protected CustomOption(string name)
        {
            Name = name;
        }

        internal abstract byte[] ToBytes();
        internal abstract void ByteValueChanged(byte[] newValue);

        internal abstract void ImplementOption(ref OptionBehaviour baseOption);
    }
}