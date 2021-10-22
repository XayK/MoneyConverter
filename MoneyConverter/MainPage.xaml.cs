using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using System.Threading;

namespace MoneyConverter
{
    public class Valute// Класс описывающий каждую из валют
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }
    class AllValutes // Класс для десериализации JSOn
    {
       
        public DateTime Date { get; set; }
        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
       // public Valute Valute { get; set; }
        public Dictionary<string, Valute> Valute { get; set; }
    }
    


    public sealed partial class MainPage : Page
    {
        AllValutes obj; // объект для загрузки JSON

        public void fillCombos()// Загрухка всех валют
        {
            foreach (var keyValue in obj.Valute)
            {
                var key = keyValue.Key;
                var value = keyValue.Value;
                Valute1.Items.Add(value.CharCode);
                Valute2.Items.Add(value.CharCode);
            }
            Valute1.SelectedIndex = 0;
            Valute2.SelectedIndex = 1;
        }
        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                Task.Run(async () =>
                {
                    HttpClient http = new HttpClient();
                    var response = await http.GetByteArrayAsync("https://www.cbr-xml-daily.ru/daily_json.js");
                    String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                    source = WebUtility.HtmlDecode(source);
                    HtmlDocument resultat = new HtmlDocument();
                    resultat.LoadHtml(source);//загрузка JSON строки

                    obj = JsonConvert.DeserializeObject<AllValutes>(resultat.Text.ToString() + "}");//десериализация

                }).Wait();


                fillCombos();
            }
            catch
            {
                //закрытие приложения при невозмоности скачать JSON
                CoreApplication.Exit();
               
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //перевод в другу валюту
                TextbLeft.Text.Replace('.', ',');
                double course = obj.Valute[Valute2.SelectedItem.ToString()].Value / obj.Valute[Valute1.SelectedItem.ToString()].Value;
                TextbRight.Text = (Convert.ToDouble(TextbLeft.Text.ToString()) / course).ToString();
            }
            catch
            {
                var msgbox = new ContentDialog
                {
                    Content = "Сумма валюты не число",
                    CloseButtonText = "Закрыть"
                };
                Task<ContentDialogResult> X = null;
                X = msgbox.ShowAsync().AsTask<ContentDialogResult>();
            }
        }

        private void Valute1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextbLeft.Text = "0,0";//обнуления суммы валюты слева
        }

        private void Valute2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextbRight.Text = "0,0";//обнуления суммы валюты справа

        }

        private void TextBlock_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //обмен валют (левый на право и наоборот)
            int sel1 = Valute1.SelectedIndex;
            int sel2 = Valute2.SelectedIndex;
            Valute1.SelectedIndex = sel2;
            Valute2.SelectedIndex = sel1;

        }

        private void TextbLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = 0; i < TextbLeft.Text.Length; i++)//удаление лишних символов при вводе
            {
                if (TextbLeft.Text[i] == '.')
                    TextbLeft.Text = TextbLeft.Text.Substring(0,i)+","+ TextbLeft.Text.Substring(i+1);
                else if(!((TextbLeft.Text[i]>='0' && TextbLeft.Text[i]<='9') ||TextbLeft.Text[i]==','))
                    TextbLeft.Text = TextbLeft.Text.Substring(0, i) + TextbLeft.Text.Substring(i + 1);
            }
        }
    }
}
