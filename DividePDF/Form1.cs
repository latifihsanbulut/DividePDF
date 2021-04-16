
using System;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace DividePDF
{
    public partial class Form1 : Form
    {
        private OpenFileDialog chooseFile = new OpenFileDialog();
        private string filePath = "";

        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            chooseFile.Multiselect = false;
            chooseFile.Filter = "Pdf files (*.pdf)|*.pdf";
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            filePath = files[files.Length-1];
            WriteFileName();
        }       

        private void button1_Click(object sender, EventArgs e)
        {
            chooseFile.ShowDialog();
            filePath = chooseFile.FileName;
            WriteFileName();
        }

        private void WriteFileName()
        {
            string[] words = filePath.Split('\\');
            textBox1.Text = words[words.Length - 1];
            label2.Visible = false;
            label2.Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (filePath.Trim().Equals(""))
            {
                MessageBox.Show("Lütfen bölmek istediğiniz pdf dosyasını seçin");
                return;
            }
            PdfReader reader = new PdfReader(filePath);
            int n = reader.NumberOfPages;
            // step 1
            iTextSharp.text.Rectangle mediabox = new iTextSharp.text.Rectangle(getHalfPageSize(reader.GetPageSizeWithRotation(1)));
            Document document = new Document(mediabox);
            // step 2
            string saveFileName = DateTime.Now.ToFileTime() + ".pdf";
            string savePath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.Desktop), saveFileName);
            PdfWriter writer
                = PdfWriter.GetInstance(document, new FileStream(savePath, FileMode.Create));
            // step 3
            document.Open();
            // step 4
            PdfContentByte content = writer.DirectContent;
            PdfImportedPage page;
            int i = 1;
            while (true)
            {
                page = writer.GetImportedPage(reader, i);
                content.AddTemplate(page, 0, -mediabox.Height);
                //document.NewPage(); /*sayfanın altındaki boş kısımı almamak için commentlendi*/
                //content.AddTemplate(page, 0, 0);
                if (++i > n)
                    break;
                mediabox = new iTextSharp.text.Rectangle(getHalfPageSize(reader.GetPageSizeWithRotation(i)));
                document.SetPageSize(mediabox);
                document.NewPage();
            }
            // step 5
            document.Close();
            reader.Close();

            label2.Visible = true;
            label2.Update();
        }

        public iTextSharp.text.Rectangle getHalfPageSize(iTextSharp.text.Rectangle pagesize)
        {
            float width = pagesize.Width;
            float height = pagesize.Height;
            return new iTextSharp.text.Rectangle(width, height / 2);
        }
    }
    
}
