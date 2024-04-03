using System.Drawing;
using System.IO;
using Cowberry_ClassLibrary.RawString;
using Cowberry_ClassLibrary.Hash;
using Cowberry_ClassLibrary.EncodedImage;


namespace Cowberry_CS
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string plain_text = @"abcd";

            RawString text = new RawString(plain_text);
            Bitmap encoded_img = Hashing.TrueEncode(text).Image;
            encoded_img.Save("MyEncoded picture.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}