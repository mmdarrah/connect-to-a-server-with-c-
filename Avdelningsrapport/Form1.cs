using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Avdelningsrapport
{
    public partial class Form1 : Form
    {
        // Create the variables
        private int port = 12345;
        private string IpAddress = "127.0.0.1";

        // Create the TcpClient object
        private TcpClient client = new TcpClient();

        public Form1()
        {
            InitializeComponent();
            client.NoDelay = true;
        }

        //List box the will contain all the data (List of books)
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //A button to start connection
        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress serverIP = IPAddress.Parse(IpAddress);
            Starconnection(serverIP);
        }

        // A button to send all the list
        private void button2_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    listBox1.SetSelected(i, true);
                byte[] message = Encoding.Unicode.GetBytes(listBox1.Text.ToString());
                    StartTransmissionList(message);
                }

            }
        }

        // A button to load the list of books 
        private void button3_Click(object sender, EventArgs e)
        {
            fileLoader();
        }

        // A button to send the selected book with the use of ToString 
        private void button4_Click(object sender, EventArgs e)
        {
            if (books.Count > 0)
            {
                if (listBox1.SelectedItems.Count == 1)
                {
                    StartTransmission(listBox1.SelectedItem.ToString());
                    books.RemoveAt(listBox1.SelectedIndex);
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                }
            }
        }


        // Async metod for the connection
        private async void Starconnection(IPAddress serverIP)
        {
            try
            {
                await client.ConnectAsync(serverIP, port);
                button1.Enabled = false;
                listBox1.Enabled = true;
            }
            catch (Exception error)// in case of error
            {
                MessageBox.Show(error.Message, this.Text);
                return;
            }
        }

        // A async method to send all the list
        private async void StartTransmissionList(byte[] message)
        {
            try
            {
                await client.GetStream().WriteAsync(message, 0, message.Length);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, this.Text);
                return;
            }
        }

        // A async method to send selected book
        public async void StartTransmission(string message)

        {
            byte[] utData = Encoding.Unicode.GetBytes(listBox1.Text);
            try
            {
                await client.GetStream().WriteAsync(utData, 0, utData.Length);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, Text);
                return;
            }

        }

        // Create a list of Books
        List<Book> books = new List<Book>();
        public void fileLoader()
        {
            // Check if th text file exist
            if (File.Exists("texter.txt"))
            {
                // Creating a list of strings
                List<string> items = new List<string>();
                // StreamReader to load a text, line by line
                StreamReader reader = new StreamReader("texter.txt", Encoding.Default, false);
                string item = "";
                //Continue to read until you reach end of file
                while ((item = reader.ReadLine()) != null)
                {
                    items.Add(item);
                }
                //Lopping 
                foreach (string a in items)
                {
                    string[] vektor = a.Split(new string[] { "###" }, StringSplitOptions.None);
                    //Put each part in the correct variable
                    string titel = vektor[0];
                    string författare = vektor[1];
                    string type = vektor[2];// the type will be uesd in the switch to determine the book class
                    bool available = Convert.ToBoolean(vektor[3]);


                    switch (type)
                    {
                        case "Roman":
                            Novel novelBook = new Novel(titel, författare, available);
                            books.Add(novelBook);
                            break;

                        case "Novellsamling":
                            Journal journalBbook = new Journal(titel, författare, available);
                            books.Add(journalBbook);
                            break;

                        case "Tidskrift":
                            ShorStory shorStoryBook = new ShorStory(titel, författare, available);
                            books.Add(shorStoryBook);
                            break;
                    }

                }

            }
            else
            {
                //A message will be show if the text file is missing
                MessageBox.Show("File is not found");
            }


            // Loop through books to add each one in the listBox
            for (int i = 0; i < books.Count; i++)
                {
                    listBox1.Items.Add(books.ElementAt(i));
                }
        }

       
    }

    // Declaring the base class
    class Book
    {
        // Class properties all are public strings
        public string bookTitle;
        public string bookAuthor;
        public string bookType;
        public bool tillgänglig; // A new bool variable because it is in the text file. Maybe we need it in the future

        // Class constructor
        public Book(string title, string author, bool available)
        {
            bookTitle = title;
            bookAuthor = author;
            tillgänglig = available;
        }
    }

    // Subclass with one more propertie
    class Novel : Book
    {
        //Inherited constructor
        public Novel(string title, string author, bool available) : base(title, author, available)
        {
            bookType = "(Novel)";
        }

        public override string ToString()
        {
            return bookTitle + " of " + bookAuthor + ". " + bookType;
        }
    }
    // Subclass with one more propertie
    class Journal : Book
    {
        //Inherited constructor
        public Journal(string title, string author, bool available) : base(title, author, available)
        {
            bookType = "(Journal)";
        }
        public override string ToString()
        {
            return bookTitle + " of " + bookAuthor + ". " + bookType;
        }
    }
    // Subclass with one more propertie
    class ShorStory : Book
    {
        //Inherited constructor
        public ShorStory(string title, string author, bool available) : base(title, author, available)
        {
            bookType = "(Short story)";
        }
        public override string ToString()
        {
            return bookTitle + " of " + bookAuthor + ". " + bookType;
        }
    }
}
