using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HowLongToBeat_Game
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            //set original size
            this.Width = 330;
            this.Height = 146;

            //Hide webbrowser
            webBrowser1.Visible = false;
            openFileDialogueButton.Visible = false;

            string BaseDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

            if (!File.Exists($"{BaseDirectory}\\Information\\login.txt"))
            {
                
            } else
            {
                StreamReader reader = new StreamReader($"{BaseDirectory}\\Information\\login.txt");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    username.Text = values[0];
                    password.Text = values[1];

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Hide controls & prevent them from being used again.
            label1.Hide();
            label2.Hide();
            button1.Hide();
            username.Hide();
            password.Hide();

            //Resizing the form after button is clicked
            Form1.ActiveForm.Width = 800;
            Form1.ActiveForm.Height = 690;

            //Setting location of window according to new size
            Form1.ActiveForm.Left = Form1.ActiveForm.Width - 100;
            Form1.ActiveForm.Top = Form1.ActiveForm.Height - 600;

            //bring back the webbrowser
            webBrowser1.Visible = true;

            //Inputs the values from the textboxes into the form.
            var InputElements = webBrowser1.Document.GetElementsByTagName("input");

            foreach (HtmlElement i in InputElements)
            {
                if (i.GetAttribute("name").Equals("username")) {
                    i.Focus();
                    i.InnerText = username.Text;
                }

                if (i.GetAttribute("name").Equals("password"))
                {
                    i.Focus();
                    i.InnerText = password.Text;
                }
                if (i.GetAttribute("name").Equals("Submit"))
                {
                    i.InvokeMember("click");
                }
            }
            webBrowser1.Navigate("https://howlongtobeat.com/submit.php");

            openFileDialogueButton.Visible = true;
            
        }

        private void openFileDialogueButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Browse Text Files";
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Gets the file location from fileDialog
                var fileName = openFileDialog1.FileName;
                string line;
                var lineCount = File.ReadAllLines($"{fileName}").Length;
                string[] gamelist = new string[lineCount];

                //Writes name to console, for no other reason than debugging.
                Console.WriteLine(fileName);

                // Read the file and display it line by line.  
                StreamReader file = new StreamReader($"{fileName}");
                int counter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    //puts the contents of file into array
                    gamelist[counter] = line;
                    counter++;
                }
                //method to process array names
                processGames(gamelist);
                counter++;
                file.Close();
            }
        }

        //Have issues here in this function.
        private void processGames(string[] gameList)
        {
            for (int count = 0; count < gameList.Length; count++)
            {
                //finds all html elements with the tag "input"
                var InputElements = webBrowser1.Document.GetElementsByTagName("input");
                foreach (HtmlElement i in InputElements)
                {
                    //Same thing again just if the attribute "name" of "input" equals "game_name"
                    if (i.GetAttribute("name").Equals("game_name"))
                    {
                        i.Focus();
                        //Here's the issue, it just places the whole string at once
                        //causing it to not be read in the website, for whatever reason
                        //As a result of this var GamesList = webBrowser1.Document.GetElementsByTagName("h3"); doesn't work properly causing the application
                        //as a whole to fail at it's job. This needs to be fixed.

                        //Issue: Will copy and paste the name into the textbox on the webpage near instaneously which makes it so that the dropdown
                        //       doesn't appear causing GamesList to return false.
                        //Eg;    It'll load https://howlongtobeat.com/submit.php and put the first in your file into it instanenously
                        //       causes JS not to work, which means that GamesList can't find the <h3> tag it's looking for
                        //       Which means it doesn't work.
                        Console.WriteLine(gameList[count]);
                        i.InnerText = gameList[count];

                        //As a result this is all ignored.
                        //This needs to be tested when the above bit works as it's supposed to.
                        var GamesList = webBrowser1.Document.GetElementsByTagName("h3");
                        foreach (HtmlElement game in GamesList)
                        {
                            if (game.InnerText.Equals(gameList[count].ToString()))
                            {
                                game.InvokeMember("click");

                                var CheckBoxandSubmit = webBrowser1.Document.GetElementsByTagName("input");
                                foreach (HtmlElement checkbox in CheckBoxandSubmit)
                                {
                                    if (checkbox.GetAttribute("name").Equals("list_b"))
                                    {
                                        webBrowser1.Document.GetElementById("list_b").InvokeMember("CLICK");
                                    }
                                    if (checkbox.GetAttribute("name").Equals("submitted"))
                                    {
                                        checkbox.InvokeMember("click");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
