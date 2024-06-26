
using Cowberry_ClassLibrary.Hash;

namespace Cowberry_ClassLibrary.RawString;

public class RawString
{
    public string Chars { get; private set; }
    public List<char> CharsList { get; private set; }
    public List<int> UnicodeCharsList { get; private set; }

    public RawString(string chars = "", string strAscii = null, string strUnicode = null, string strHexColor = null, string fullImg = null)
    {
        this.Chars = chars ?? "";
        this.CharsList = chars.ToCharArray().ToList();
        this.UnicodeCharsList = this.CharsList.Select(c => (int)c).ToList();
    }

    public override string ToString()
    {
        return this.Chars;
    }

    private bool IsAscii()
    {
        return this.Chars.All(c => c <= 127);
    }
    
    // Mетод для генерации списка шестнадцатеричных цветов
    private List<string> HexColorList()
    {
        return this.UnicodeCharsList.Select(unicodeChar => Hashing.Sigmoid(unicodeChar)).ToList();
    }
    
    public List<Bitmap> CharsToImageList()
    {
        List<Bitmap> imageList = new List<Bitmap>();
        foreach (string? hexColor in this.HexColorList())
        {
            // Преобразование шестнадцатеричного кода цвета в объект Color
            Color color = ColorTranslator.FromHtml(hexColor);
            Bitmap img = new Bitmap(5, 5);
                
            using (Graphics gfx = Graphics.FromImage(img))
            {
                gfx.Clear(color); // Заливка изображения цветом
            }

            imageList.Add(img);
        }

        return imageList;
    }
    
    // Создание "прямоугольника" из строки
    public static List<string> RectangularStr(string str, int? step = null)
    {
        while (true)
        {
            switch (step)
            {
                case null :
                {
                    int sqrt = (int)Math.Sqrt(str.Length);
                    step = sqrt;
                    break;
                }
                default :
                {
                    List<string> parts = new List<string>();
                    for (int i = 0; i < str.Length; i += step.Value)
                    {
                        parts.Add(str.Substring(i, Math.Min(step.Value, str.Length - i)));
                    }

                    return parts;
                }
            }
        }
    }
    
    // Преобразование строки в двумерный список символов
    public static List<List<char>> Char2D(string str, int? step = null)
    {
        List<string> parts = RectangularStr(str, step ?? (int)Math.Sqrt(str.Length));
        return parts.Select(part => part.ToList()).ToList();
    }
    
    public List<List<Bitmap>> CharsToImageList2D(int? step = null, int key = 1)
    {
        // Генерируем одномерный список изображений
        List<Bitmap> imageList = CharsToImageList();

        // Применяем шифрование Цезаря к списку изображений
        List<Bitmap> encryptedImageList = Hashing.Ceaser(imageList, key);

        // Рассчитываем шаг, если он не был задан
        if (!step.HasValue)
        {
            step = (int)Math.Sqrt(encryptedImageList.Count);
        }

        // Создаем двумерный список изображений из зашифрованного списка
        List<List<Bitmap>> imageList2D = 
            Enumerable.Range(0, (int)Math.Ceiling(encryptedImageList.Count / (double)step.Value))
            .Select(index => encryptedImageList
                .Skip(index * step.Value)
                .Take(step.Value)
                .ToList())
            .ToList();

        return imageList2D;
    }

    public static Bitmap ImgAddList2D(List<List<Bitmap>> img2DList)
    {
        int totalWidth = img2DList.Max(row => row.Sum(img => img.Width));
        int totalHeight = img2DList.Count * img2DList[0][0].Height; // Предполагая, что высота всех строк одинакова

        Bitmap finalImage = new Bitmap(totalWidth, totalHeight);
        using Graphics g = Graphics.FromImage(finalImage);
        // Заполняем фон изображения черным цветом
        g.Clear(Color.Black);

        int yOffset = 0;
        foreach (var row in img2DList)
        {
            int xOffset = 0;
            foreach (Bitmap? img in row)
            {
                g.DrawImage(img, new Point(xOffset, yOffset));
                xOffset += img.Width;
            }
            yOffset += row[0].Height;
        }

        return finalImage;
    }
    
    public Bitmap Encode(int? step = null, int key = 1)
    {
        var imageList2D = this.CharsToImageList2D(step, key);
        return ImgAddList2D(imageList2D);
    }
    
}