using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using System.Xml.Linq;
using System.IO;

namespace lr4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XDocument xDocument;
        XElement books;
        string filepath;

        public MainWindow()
        {
            InitializeComponent();

            OpenFileDialog fileDialog = new OpenFileDialog() 
            {
                Filter = "Text files (*.xml)|*.xml"
            };

            if (fileDialog.ShowDialog() == true)
            {
                xDocument = XDocument.Load(fileDialog.FileName);
                books = xDocument.Root;
                refreshList();
                BookDataListBox.SelectedIndex = 0;
                filepath= fileDialog.FileName;
            }
        }

        private void refreshList()
        {
            BookDataListBox.Items.Clear();

            foreach (XElement book in books.Elements("book"))
            {
                XAttribute title = book.Attribute("title");       
                XElement author = book.Element("author");
                XElement release_date = book.Element("release_date");
                XElement price = book.Element("price");
                XElement genre = book.Element("genre");
                XElement rating = book.Element("rating");
                ListBoxItem item = new ListBoxItem();   

                item.Content = ($"Название: \"{title.Value}\" \n" +
                                   $"Автор: {author.Value}\n" +
                                   $"Год издания: {release_date.Value}\n" +
                                   $"Цена: {price.Value} рублей\n" +    
                                   $"Жанр: {genre.Value} \n" +
                                   $"Рейтинг: {rating.Value}\n");

                BookDataListBox.Items.Add(item); 
            }
        }

        private void BookDataListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookDataListBox.SelectedIndex != -1)
            {
                XElement book = books.Elements("book").ElementAt(BookDataListBox.SelectedIndex);
                TitleTextBox.Text = book.Attribute("title").Value;
                AuthorTextBox.Text = book.Element("author").Value;
                ReleaseDateTextBox.Text = book.Element("release_date").Value;
                PriceTextBox.Text = book.Element("price").Value;
                GenreTextBox.Text = book.Element("genre").Value;
                RatingTextBox.Text = book.Element("rating").Value;     
            }
        }

        private void saveInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (BookDataListBox.SelectedIndex != -1)                    
            {
                var elementIndex = BookDataListBox.SelectedIndex;
                XElement book = books.Elements("book").ElementAt(elementIndex);
                book.Attribute("title").Value = TitleTextBox.Text;
                book.Element("author").Value = AuthorTextBox.Text;
                book.Element("release_date").Value = ReleaseDateTextBox.Text;
                book.Element("price").Value = PriceTextBox.Text;
                book.Element("genre").Value = GenreTextBox.Text;
                book.Element("rating").Value = RatingTextBox.Text;

                xDocument.Save(filepath);        
                refreshList();                
                BookDataListBox.SelectedIndex = elementIndex;
            }
        }

        private void deleteInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (BookDataListBox.SelectedIndex != -1)  
            {
                int elementIndex = BookDataListBox.SelectedIndex;
                XElement book = books.Elements("book").ElementAt(elementIndex);
                book.Remove();
                xDocument.Save(filepath);
                refreshList();
                if (BookDataListBox.Items.Count > 0) BookDataListBox.SelectedIndex = 0;
            }
        }

        private void createInfoButton_Click(object sender, RoutedEventArgs e)
        {
            // добавляем пустой элемент
            books.Add(new XElement("book",
                                    new XAttribute("title", ""),
                                    new XElement("author", ""),
                                    new XElement("release_date", ""),
                                    new XElement("price", ""),
                                    new XElement("genre", ""),
                                    new XElement("rating", "")
                                    ));
            xDocument.Save(filepath);
            refreshList();
            BookDataListBox.SelectedIndex = BookDataListBox.Items.Count - 1;
        }
    }
}