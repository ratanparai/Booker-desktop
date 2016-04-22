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

namespace Booker
{
    public partial class Form1 : Form
    {
        private BookPage[] pageSave = new BookPage[2000];
        private int currentLine;
        private int curPage;
        private int totPage;


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

                    String bookContent = myEbook.GetContentAsPlainText();

                    // each line in a each array elements
                    bookContentArray = bookContent.Split('\n');

                    progressBar1.Visible = true;
                    progressBarLabel.Visible = true;
                    


                    countThePage();

                    progressBar1.Visible = false;
                    progressBarLabel.Visible = false;

                    totalPage.Text = totPage.ToString();
                    label2.Text = "/";

                    curPage = 0;

                    // default show page 1 contents
                    showPageContent(1);

                    // check if the book info is saved or not

                    // if not not saved then save it
                    Booker bkr = new Booker();
                    bkr.searchBook(title, openEbook.FileName, author);

                }

                

                

            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void countThePage()
        {
            int workingPage = 0;
            while(true)
            {
                int startLine = currentLine;
                int nextLine = pageCount();

                if (nextLine >= bookContentArray.Length-1)
                {
                    totPage = workingPage;
                    break;
                }

                BookPage bp = new BookPage(startLine, nextLine);

                pageSave[workingPage] = bp;
                
                workingPage++;
                currentLine++;

            }
        }

        private int pageCount()
        {
            // sample label for checking content size
            tempLabel.Text = "";

            while (currentLine<bookContentArray.Length)
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

        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {
            showPageContent(curPage + 1);
        }

        private void prevPageButton_Click(object sender, EventArgs e)
        {
            showPageContent(curPage - 1);
        }

        private void tempLabel_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings frm1 = new Settings();
            frm1.Show();
        }
    }
}
