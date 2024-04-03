
using Cowberry_ClassLibrary.Hash;
using Cowberry_ClassLibrary.RawString;

namespace Cowberry_ClassLibrary.EncodedImage
{
    public class EncodedImage
    {
        public Bitmap Image { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string FullStr { get; private set; }
        
        public EncodedImage(Bitmap image, string fullStr = null)
        {
            Image = image;
            // В .NET свойства Width и Height можно получить напрямую из Bitmap
            Width = image.Width;
            Height = image.Height;
            FullStr = fullStr;
        }

        public override string ToString()
        {
            return this.Decode();
        }
        
        // Возвращает 2D список изображений для раскодирования
        public List<List<Bitmap>> ImgSplitting2D()
        {
            (int, int) repeats = (this.Width / 5, this.Height / 5);
            var imgs2D = new List<List<Bitmap>>();

            for (int y = 0; y < repeats.Item2; y++)
            {
                var singles = new List<Bitmap>();
                for (int x = 0; x < repeats.Item1; x++)
                {
                    Rectangle rect = new Rectangle(x * 5, y * 5, 5, 5);
                    singles.Add(CropImage(Image, rect));
                }
                imgs2D.Add(singles);
            }

            return imgs2D;
        }

        private static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // Clone используется для копирования части изображения, заданной прямоугольником
            return source.Clone(section, source.PixelFormat);
        }
        
        public List<Bitmap> ImgIn1D()
        {
            // Получаем двумерный список изображений
            // Сглаживаем двумерный список в одномерный
            // Применяем дешифровку
            return Hashing.DeCeaser(this.ImgSplitting2D().SelectMany(i => i).ToList());
        }
        
        public List<string> CreateEncryptedColors()
        {
            // Получаем одномерный список изображений
            var img1D = this.ImgSplitting2D().SelectMany(sublist => sublist).ToList();
            var encryptedColors = img1D.Select(img => img.GetPixel(1, 1)).Select(pixelColor => Hashing.RGBToHashingHex(pixelColor.R, pixelColor.G, pixelColor.B)).ToList();

            // Удаление всех вхождений черного цвета
            encryptedColors.RemoveAll(color => color == "#000000");

            return encryptedColors;
        }
        
        public string Decode()
        {
            var encryptedColors = CreateEncryptedColors();
            var listValues = encryptedColors.Select(Hashing.ArcSigmoid).ToList();

            // Коррекция символов из-за неточности
            List<int> correctedValues = listValues.Select(value =>
            {
                return value switch
                {
                    '䑒' => 'a',
                    '䔾' => 'b',
                    '䘯' => 'c',
                    '䜦' => 'd',
                    '䘩' => 'F',
                    '䏋' => ' ',
                    '蔞' => 'ж',
                    '藂' => 'р',
                    '莥' => 'О',
                    '䔪' => '6',
                    '䱹' => '«',
                    _ => value
                };
            }).ToList();

            // Преобразование списка числовых значений в текст
            string PlainText = new string(correctedValues.Select(c => (char)c).ToArray());
            return PlainText;
        }
    }
}