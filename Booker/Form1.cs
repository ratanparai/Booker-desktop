using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eBdb.EpubReader;
using System.Threading;

using Json;

namespace Booker
{
    public partial class Form1 : Form
    {
        //private BookPage[] pageSave = new BookPage[2000];
        private List<BookPage> pageSave = new List<BookPage>();
        private int currentLine;
        private int curPage;
        private int totPage;
        private string current_book_id;


        private string[] bookContentArray;
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // reset every variable
                currentLine = 0;
                curPage = 0;
                totPage = 0;


                OpenFileDialog openEbook = new OpenFileDialog();
                openEbook.Filter = "Ebook Files (.epub)| *.epub";
                openEbook.FilterIndex = 1;
                openEbook.Multiselect = false;

                if(openEbook.ShowDialog() == DialogResult.OK)
                {
                    // MessageBox.Show(openEbook.FileName);
                    Epub myEbook = new Epub(openEbook.FileName);

                    string title = myEbook.Title[0];
                    string author = myEbook.Creator[0];

                    bookInfoLabel.Text = title + " - " + author;

                    String bookContent = myEbook.GetContentAsPlainText();

                    // each line in a each array elements
                    bookContentArray = bookContent.Split('\n');

                    progressBar1.Visible = true;
                    progressBarLabel.Visible = true;

                    string pageSaveLocation = System.IO.Path.ChangeExtension(openEbook.FileName, "page");

                    if (System.IO.File.Exists(pageSaveLocation))
                    {
                        this.pageSave = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BookPage>>(System.IO.File.ReadAllText(pageSaveLocation));
                        this.totPage = pageSave.Count;
                    } else
                    {
                        countThePage();
                        String pageSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(pageSave);
                        System.IO.File.WriteAllText(pageSaveLocation, pageSaveJson);
                    }

                    

                    progressBar1.Visible = false;
                    progressBarLabel.Visible = false;

                    totalPage.Text = totPage.ToString();
                    label2.Text = "/";

                    curPage = 0;

                    

                    // check if the book info is saved or not
                    // get joson file name
                    string bookInfoFile = System.IO.Path.ChangeExtension(openEbook.FileName, "booker");

                    // if file exits
                    Booker bkr = new Booker();
                    if (System.IO.File.Exists(bookInfoFile))
                    {
                        // read file info
                        var book = JsonParser.Deserialize(System.IO.File.ReadAllText(bookInfoFile));

                        this.current_book_id = book.book._id;

                        bookInfoLabel.Text = book.book.title + " : " + book.book.author_name;

                    } else
                    {
                        bkr.searchBook(title, openEbook.FileName, author);

                        var book = JsonParser.Deserialize(System.IO.File.ReadAllText(bookInfoFile));
                        this.current_book_id = book.book._id;
                        bookInfoLabel.Text = book.book.title + " : " + book.book.author_name;

                    }

                    double progressPercentage = bkr.getProgress(current_book_id);



                    // if not not saved then save it
                    // default show page 1 contents
                    if(progressPercentage == 0)
                    {
                        showPageContent(1);
                    } else
                    {
                        Console.WriteLine(progressPercentage);
                        int pageToShow = (int)(((progressPercentage * totPage) / 100) + .5);
                        showPageContent(pageToShow);
                    }

                    // save pagesave as json
                    

                    


                }

                

                

            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void countThePage()
        {
            int workingPage = 0;
            while(true)
            {
                int startLine = currentLine;
                int nextLine = pageCount();

                Console.WriteLine("NextLine : " + nextLine);

                if (nextLine == 1210)
                {
                    Console.WriteLine("Debug..");
                }

                if (nextLine >= bookContentArray.Length-1)
                {
                    totPage = workingPage;
                    break;
                }

                BookPage bp = new BookPage(startLine, nextLine);

                //pageSave[workingPage] = bp;
                pageSave.Add(bp);

                Console.WriteLine("Working Line : " + workingPage + " CurrentLine: " + currentLine);
                Console.WriteLine("Total Line: " + bookContentArray.Length);

                workingPage++;
                currentLine++;

            }
        }

        private int pageCount()
        {
            // sample label for checking content size
            tempLabel.Text = "";

            while (currentLine<=bookContentArray.Length-1)
            {

                tempLabel.Text += bookContentArray[currentLine] + Environment.NewLine;
                //MessageBox.Show(tempLabel.Height.ToString());
                if (tempLabel.Height > 556)
                {
                    currentLine--;
                    break;
                }
                currentLine++;
            }

            //Console.WriteLine(tempLabel.Height);

            return currentLine;
        }

        private void showPageContent(int pageNumber)
        {

            if(pageNumber > totPage)
            {
                pageNumber = totPage;
            }

            if(pageNumber < 1)
            {
                pageNumber = 1;
            }

            label1.Text = "";

            for (int i = pageSave[pageNumber-1].startLineNumber; i <= pageSave[pageNumber-1].endLineNumber; i++)
            {
                label1.Text += bookContentArray[i] + Environment.NewLine;

            }

            Console.WriteLine(label1.Height.ToString());

            if(totPage == pageNumber)
            {
                nextPageButton.Visible = false;
            } else
            {
                nextPageButton.Visible = true;
            }

            if(pageNumber == 1)
            {
                prevPageButton.Visible = false;
            } else
            {
                prevPageButton.Visible = true;
            }

            curPage = pageNumber;
            currentPageLabel.Text = curPage.ToString();

            postProgress();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            totalPage.Text = "";
            currentPageLabel.Text = "";
            label2.Text = "";
            nextPageButton.Visible = false;
            prevPageButton.Visible = false;

            tempLabel.MaximumSize = new Size(600, 0);
            tempLabel.AutoSize = true;
            bookInfoLabel.Text = "";

            gotoPageInput.Visible = false;

            //gotoPageInput.KeyDown += new System.EventHandler(gotoPageInput_KeyDown);


        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {
            showPageContent(curPage + 1);
            //Task t = new Task(postProgress);
            //t.Start();
            
        }

        private void prevPageButton_Click(object sender, EventArgs e)
        {
            showPageContent(curPage - 1);
            //Task t = new Task(postProgress);
            //t.Start();
            //postProgress();
        }

        private void tempLabel_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings frm1 = new Settings();
            frm1.Show();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void postProgress()
        {
            Console.WriteLine("Progress report");
            double progressPercentage = ((double) curPage / (double) totPage) * 100;
            Console.WriteLine(progressPercentage);

            Booker bkr = new Booker();
            bkr.postProgress(progressPercentage, this.current_book_id);
        }

        private void currentPageLabel_Click(object sender, EventArgs e)
        {
            string pageNumber = currentPageLabel.Text;
            currentPageLabel.Visible = false;

            gotoPageInput.Text = pageNumber;
            gotoPageInput.Visible = true;
        }


        private void gotoPageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int gotoPageNumber =  Int32.Parse(gotoPageInput.Text);
                gotoPageInput.Visible = false;
                currentPageLabel.Visible = true;

                showPageContent(gotoPageNumber);
            }
        }
    }
}
