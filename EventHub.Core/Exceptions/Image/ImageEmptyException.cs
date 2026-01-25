using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Exceptions.Image
{
    public class ImageEmptyException : Exception
    {
        public ImageEmptyException()
            :base("The image file is empty.")
        {
            
        }
    }
}
