using Cowberry_ClassLibrary.EncodedImage;

namespace Cowberry_ClassLibrary.Hash;

public class Hashing
{
    public static int HashingHexToDec1(string hexString)
    {
        hexString = hexString.ToLower();
        if (hexString.Length != 6)
        {
            throw new ArgumentException("Строка не в верном формате 'ffffff'");
        }

        return hexString.Select(c => c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'a' and <= 'f' => 10 + (c - 'a'),
                _ => throw new ArgumentException("Не верный цвет")
            })
            .Select((hexValue, i) => hexValue * (int)Math.Pow(16, hexString.Length - i - 1))
            .Sum();
    }
    
    public static int HashingHexToDec2(string hexString)
    {
        hexString = hexString.ToLower();
        // Проверка, что строка начинается с '#'
        if (!hexString.StartsWith("#") || hexString.Length != 7)
        {
            throw new ArgumentException("Строка не в верном формате '#ffffff'");
        }

        hexString = hexString[1..]; // Убираем '#'

        return HashingHexToDec1(hexString);
    }
    
    public static string HashingDecToHex(int number)
    {
        string hexString = number.ToString("X6"); // Преобразование в шестнадцатеричный формат с заполнением нулями до 6 символов
        return "#" + hexString.ToLower();
    }

    public static string RGBToHashingHex(int red, int green, int blue)
    {
        // Форматируем RGB значения в шестнадцатеричную строку с сохранением формата "#rrggbb"
        return $"#{red:X2}{green:X2}{blue:X2}".ToLower();
    }
    
    private static string DecToHex(int dec)
    {
        return $"#{dec:X6}";
    }

    // Конвертация шестнадцатеричного представления в десятичное число
    private static int HexToDec(string hex)
    {
        // Убедитесь, что строка начинается с '#', и удалите этот символ
        if (!hex.StartsWith("#")) return Convert.ToInt32(hex, 16);
        hex = hex[1..];
        return Convert.ToInt32(hex, 16);
    }
    
    public static List<Bitmap> Ceaser(List<Bitmap> imageList, int key = 1)
    {
        int count = imageList.Count;
        // Нормализуем ключ, чтобы избежать выхода за пределы списка
        key = ((key % count) + count) % count; // Это учитывает отрицательные значения ключа
        return imageList.Skip(key).Concat(imageList.Take(key)).ToList();
    }
    
    public static List<Bitmap> DeCeaser(List<Bitmap> encImageList, int key = 1)
    {
        // Рассчитываем "дешифровочный" ключ как длину списка минус ключ шифрования
        // Применяем дешифровку путем сдвига элементов списка
        List<Bitmap> decryptedList = encImageList.Skip(encImageList.Count - key).Concat(encImageList.Take(encImageList.Count - key)).ToList();
        return decryptedList;
    }

    public static bool IsPrime(int num)
    {
        return num > 1 && Enumerable.Range(1, num).Where(x => num%x == 0).SequenceEqual(new[] {1, num});
    }
    
    public static string Sigmoid(int x)
    {
        int hashedValue = 1123 / (1123 + x) * 1000000;
        string hashedValueHex = DecToHex(hashedValue);
        return hashedValueHex;
    }

    // Раскодирование значения с использованием сигмоидной функции
    public static int ArcSigmoid(string hashedValue)
    {
        int archashedValue = HexToDec(hashedValue);
        if (archashedValue > 0)
        {
            archashedValue = (int)(1123 / (archashedValue / 1000000.0) - 1123);
        }
        return archashedValue;
    }
    
    public static EncodedImage.EncodedImage TrueEncode(RawString.RawString raw_string)
    {
        return new EncodedImage.EncodedImage(RawString.RawString.ImgAddList2D(raw_string.CharsToImageList2D(50, 1)));
    }
    
    public static RawString.RawString TrueDecode(EncodedImage.EncodedImage img)
    {
        return new RawString.RawString(img.Decode());
    }
}