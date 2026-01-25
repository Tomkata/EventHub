namespace EventHub.Services.Images
{
    public static class ImageSignatures
    {
        public static byte[] png = new byte[]
       {
            137,
            80,
            78,
            71
       };
        public static byte[] jpeg = new byte[]
        {
            255,
            216,
            255,
            224
        };
        public static byte[] jpeg2 = new byte[]
        {
            255,
            216,
            255,
            225
        };     

    }
}
