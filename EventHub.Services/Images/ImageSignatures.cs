using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Services.Images
{
    public static class ImageSignatures
    {
        static byte[] png = new byte[]
       {
            137,
            80,
            78,
            71
       }; 
        static byte[] jpeg = new byte[]
        {
            255,
            216,
            255,
            224
        };     
        static byte[] jpeg2 = new byte[]
        {
            255,
            216,
            255,
            225
        };     

    }
}
