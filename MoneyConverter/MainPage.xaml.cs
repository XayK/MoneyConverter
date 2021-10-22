using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace MoneyConverter
{
    public class Valute
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }
    class AllValutes
    {
       
        public DateTime Date { get; set; }
        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
       // public Valute Valute { get; set; }
        public Dictionary<string, Valute> Valute { get; set; }
    }
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //JsonSerializer JS = JsonSerializer.Create();
        AllValutes obj;

        public void fillCombos()
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
            Task.Run(async () =>
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync("https://www.cbr-xml-daily.ru/daily_json.js");
                String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(source);

                

               
                obj = JsonConvert.DeserializeObject<AllValutes>(resultat.Text.ToString()+"}");

            }).Wait();


            fillCombos();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextbLeft.Text.Replace('.', ',');
            double course=obj.Valute[Valute2.SelectedItem.ToString()].Value/ obj.Valute[Valute1.SelectedItem.ToString()].Value;
            TextbRight.Text = (Convert.ToDouble(TextbLeft.Text.ToString())/course).ToString();        
        }

        private void Valute1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextbLeft.Text = "0,0";
        }

        private void Valute2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextbRight.Text = "0,0";

        }

        private void TextBlock_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            int sel1 = Valute1.SelectedIndex;
            int sel2 = Valute2.SelectedIndex;
            Valute1.SelectedIndex = sel2;
            Valute2.SelectedIndex = sel1;

        }
    }
}
