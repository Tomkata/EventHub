using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Exceptions.Image
{
    public class ImageTooLargeException : Exception
    {
        public ImageTooLargeException()
            :base("The image size is too large")
        {
            
        }
    }
}
