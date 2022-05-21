using System;

namespace TheGameOfAzazel
{
    public class Upgrade
    {
        public string Name { get; set; }
        public float InitialValue { get; set; }
        public int Level { get; set; } = 1;

        public float Value { get; set; } = 0;

        //public float NextValue { get; set; } = 0;
        public string Unit { get; set; } = string.Empty;

        public Func<int?, float> OffsetterFunction { get; set; }
    }
}
