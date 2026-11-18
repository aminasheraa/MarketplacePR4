using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace MarketplacePR4.Helpers
{
    public class SavedProductCard
    {
        private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");
        private readonly SavedProductItemDto _item;

        public SavedProductCard(SavedProductItemDto item)
        {
            _item = item;
            Picture = LoadPicture(item.Image);
        }

        public int ProductId { get { return _item.ProductId; } }
        public string Name { get { return _item.Name; } }
        public string PriceText { get { return _item.Price.ToString("N2", Ru) + " ₽"; } }
        public string RatingText { get { return "★ " + _item.Rating.ToString("0.0", Ru); } }

        // Купить можно, только если товар доступен и есть на складе
        public bool CanBuy { get { return _item.IsAvailable && _item.StockQuantity > 0; } }

        public string StockText
        {
            get
            {
                if (!_item.IsAvailable) return "Недоступен для покупки";
                if (_item.StockQuantity <= 0) return "Нет в наличии";
                return "В наличии: " + _item.StockQuantity + " шт.";
            }
        }

        public Brush StockBrush { get { return CanBuy ? Brushes.SeaGreen : Brushes.Firebrick; } }

        public BitmapImage Picture { get; private set; }

        // Фотографии лежат в папке Images рядом с программой. В БД хранится только имя файла.
        private static BitmapImage LoadPicture(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                if (!File.Exists(path)) return null;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;   // файл не блокируется после загрузки
                bmp.UriSource = new Uri(path);
                bmp.DecodePixelWidth = 160;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch (Exception)
            {
                return null;   // битый файл: показываем серый квадрат вместо картинки
            }
        }
    }
}
