using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EventHub.Core.Exceptions.Image
{
    public class InvalidImageFormatException : Exception
    {
        public InvalidImageFormatException()
            :base("The image format is invalid.")
        {
        }
    }
}
