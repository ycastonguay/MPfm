using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPfm.Sound.BassNetWrapper
{
    public class BassNetWrapperException : Exception
    {
        public BassNetWrapperException(string message) 
            : base("An error has occured in Bass.Net: " + message)
        {            
        }
    }
}
