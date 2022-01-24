using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpeechLib;
using System.Speech.Synthesis;
using System.Threading;
using System.IO;

namespace english_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Word GlobalWord = null;
        public static List<Tuple<string, bool>> GlobalTupleList;
        public static string currentWord;
        public static int Volume = 30;

        public static string[] randomWord;
        private Caroussel carous = new Caroussel();
        [Localizable(true)]
        public MainWindow()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
             
            InitializeComponent();

            randomWord = File.ReadAllLines("RandomWords.txt");
            DbNames dbnames = new DbNames(combobox.Text);
            SetWords();

            carous.Topmost = true;
            
        }

        
        private void SetWords()
        {
            Random RND = new Random();

            GlobalTupleList = GetWords();
            GlobalTupleList = GlobalTupleList.OrderBy(a => Guid.NewGuid()).ToList();
            this.Dispatcher.Invoke(() =>
            {
                this.button2.Content = GlobalTupleList[0].Item1;
                this.button3.Content = GlobalTupleList[1].Item1;
                this.button4.Content = GlobalTupleList[2].Item1;
                this.button5.Content = GlobalTupleList[3].Item1;
            });
        }


        private delegate void VocalizeHandler(Languages language, String word);
        private event VocalizeHandler Vocalize;
        private void AddVocalizeHandler()
        {
            Vocalize += VocalizeMethod;
        }
        private void RemoveVocalizeHandler()
        {
            Vocalize -= VocalizeMethod;
        }
        private void VocalizeMethod(Languages language, String word)
        {

            if (language == Languages.EN)
            {
                //var spvoiceRus = new SpVoice();
                //spvoiceRus.Volume = (Volume);
                //spvoiceRus.Rate = -3;
                //spvoiceRus.Speak(word, SpeechVoiceSpeakFlags.SVSFDefault);

                var synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = Volume;
                synthesizer.Rate = 0;
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.SelectVoice("Microsoft Hazel Desktop");
                synthesizer.Speak(word);
                //var synthesizer = new SpeechSynthesizer();
                //synthesizer.Volume = 50;
                //synthesizer.SetOutputToDefaultAudioDevice();
                //synthesizer.SelectVoice("Microsoft                                     Zira Desktop");
                //synthesizer.Speak(word);
            }
            if (language == Languages.RU)
            {
                var synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = Volume;
                synthesizer.Rate = 2;
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.SelectVoice("Microsoft Irina Desktop");
                synthesizer.Speak(word);
            }
        }

        enum Languages
        {
            RU, EN
        }
        private List<Tuple<string, bool>> GetWords()
        {

            int orderNumber = new Random().Next(0, 3);

            var db = new RandomWordsContext();
            int counts = db.Words.Count();
            int wordNumber = new Random().Next(0, counts - 1);
            var word = db.Words.Skip(wordNumber).FirstOrDefault();

            GlobalWord = word;
            currentWord = word.EnglishWord;//set true english word

            this.Dispatcher.Invoke(() =>
            {
                carous.courlabel1.Content = word.RussianWord;
                carous.courlabel2.Content = word.EnglishWord;
            });

            Vocalize?.Invoke(Languages.EN, word.EnglishWord);
            Vocalize?.Invoke(Languages.RU, word.RussianWord);
            Thread.Sleep(50);

            this.Dispatcher.Invoke(() =>
            {
                this.label1.Content = word.RussianWord;// set label russian word
                attempts_label.Content = word.Try + "/5"; // counter attempts
                wordsInBase.Content = db.Words.Count().ToString();
            });
        

            List<Tuple<string, bool>> List = new List<Tuple<string, bool>>();
            for (int i = 0; i <= 3; i++)
            {
                if (i == orderNumber)
                {
                    List.Add(new Tuple<string, bool>(word.EnglishWord, true));
                }
                else
                {
                    List.Add(new Tuple<string, bool>(GetFakeWords(), false));
                }
            }
            return List;
        }
        private string GetFakeWords()
        {
            return randomWord[new Random(Guid.NewGuid().GetHashCode()).Next(0, 9999)];
        }
        private void CheckWord(Button checkButton)
        {


            SpVoice spvoice = new SpVoice();
            spvoice.Speak(currentWord, SpeechVoiceSpeakFlags.SVSFDefault);



            if (checkButton.Content == currentWord)
            {
                using (var db = new RandomWordsContext())
                {
                    var tableToChange = db.Words.SingleOrDefault(b => b.EnglishWord == currentWord);    // add try +1
                    int numberOfTry = tableToChange.Try;

                    if (numberOfTry == 5)
                    {
                        db.Remove(tableToChange);
                    }
                    else
                    {
                        tableToChange.Try++;
                    }
                    db.SaveChanges();
                    wordsInBase.Content = db.Words.Count().ToString();
                }
            }
            else
            {
                using (var db = new RandomWordsContext())
                {
                    var tableToChange = db.Words.SingleOrDefault(b => b.EnglishWord == currentWord); // refresh try 0
                    tableToChange.Try = 0;
                    db.SaveChanges();
                }

                Thread.Sleep(200);
                try
                {

                    SpVoice spv = new SpVoice();
                    spv.Rate = -4;

                    spv.Speak(currentWord, SpeechVoiceSpeakFlags.SVSFDefault);
                }
                catch { }


            }


            if (button2.Content == currentWord) { button2.Background = new SolidColorBrush(Colors.Green); } //else { button2.BackColor = Color.Red; }
            if (button3.Content == currentWord) { button3.Background = new SolidColorBrush(Colors.Green); } //else { button3.BackColor = Color.Red; }
            if (button4.Content == currentWord) { button4.Background = new SolidColorBrush(Colors.Green); } //else { button4.BackColor = Color.Red; }
            if (button5.Content == currentWord) { button5.Background = new SolidColorBrush(Colors.Green); } //else { button5.BackColor = Color.Red; }

            Thread.Sleep(300);
            try { new SpVoice().Speak(currentWord, SpeechVoiceSpeakFlags.SVSFDefault); } catch { }
            Thread.Sleep(200);

            button2.Background = new SolidColorBrush(Colors.White);
            button3.Background = new SolidColorBrush(Colors.White);
            button4.Background = new SolidColorBrush(Colors.White);
            button5.Background = new SolidColorBrush(Colors.White);

            this.button2.IsEnabled = true;
            this.button3.IsEnabled = true;
            this.button4.IsEnabled = true;
            this.button5.IsEnabled = true;

            SetWords();

        }
        private void Cycle()
        {
            while (true)
            {
                SetWords();
            }
        }
        private void buttonsOff()
        {
            this.button2.IsEnabled = false;
            this.button3.IsEnabled = false;
            this.button4.IsEnabled = false;
            this.button5.IsEnabled = false;
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {

            buttonsOff();
            var task = Task.Factory.StartNew(() =>
            {
                CheckWord(this.button2);
            });
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            buttonsOff();
            var task = Task.Factory.StartNew(() =>
            {
                CheckWord(this.button3);
            });
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            buttonsOff();
            var task = Task.Factory.StartNew(() =>
            {
                CheckWord(this.button4);
            });
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            buttonsOff();
            var task = Task.Factory.StartNew(() =>
            {
                CheckWord(this.button5);
            });
        }
 

        private async void button7_Click(object sender, RoutedEventArgs e)
        {
           await Task.Run(() => {
                this.Dispatcher.Invoke(() =>
                {
                    carous.Show();
                });
                AddVocalizeHandler();
                Cycle();
            }); 
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            string rword = rusWordTextbox.Text;
            string eword = engWordTextbox.Text;

            if (!String.IsNullOrEmpty(rword) && !String.IsNullOrEmpty(eword))
            {
                using (RandomWordsContext dbcontext = new RandomWordsContext())
                {
                    var row = dbcontext.Words.Where(x => x.EnglishWord == eword).FirstOrDefault();
                    if (row == null)
                    {
                        Word newword = new Word { EnglishWord = eword, RussianWord = rword, Try = 0 };
                        dbcontext.Words.Add(newword);
                        dbcontext.SaveChanges();
                        Task task = Task.Factory.StartNew(() =>
                        {
                            this.button8.Background = new SolidColorBrush(Colors.Red);
                            Thread.Sleep(1000);
                            this.button8.Background = new SolidColorBrush(Colors.White);
                        });
                        SetWords();
                    }
                    else
                    {
                        Task task = Task.Factory.StartNew(() =>
                        {
                            this.button8.Background = new SolidColorBrush(Colors.Red);
                            Thread.Sleep(1000);
                            this.button8.Background = new SolidColorBrush(Colors.White);
                        });
                        this.button8.Background = new SolidColorBrush(Colors.White);
                        return;
                    }
                }
            }
            else
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    this.button8.Background = new SolidColorBrush(Colors.Red);
                    Thread.Sleep(1000);
                    this.button8.Background = new SolidColorBrush(Colors.White);
                });
                this.button8.Background = new SolidColorBrush(Colors.White);
                return;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
