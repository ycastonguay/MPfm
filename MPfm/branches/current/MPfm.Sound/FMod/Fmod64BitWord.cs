using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPfm.Sound
{
    /// <summary>
    /// Represents a 64-bit word (2 x 32-bit unsigned integer) for FMOD.
    /// Used in GetDelay.
    /// </summary>
    public class Fmod64BitWord
    {
        /// <summary>
        /// Top (most significant) 32 bits of a 64bit number.
        /// </summary>
        public uint hi;

        /// <summary>
        /// Bottom (least significant) 32 bits of a 64bit number.
        /// </summary>
        public uint lo;

        /// <summary>
        /// Default constructor. Sets value to 0 for both values.
        /// </summary>
        public Fmod64BitWord()
        {
            // Set default value
            hi = 0;
            lo = 0;
        }

        /// <summary>
        /// Constructor with both values.
        /// </summary>
        /// <param name="hi">High value</param>
        /// <param name="lo">Low value</param>
        public Fmod64BitWord(uint hi, uint lo)
        {
            // Set values
            this.hi = hi;
            this.lo = lo;
        }
    }
}
