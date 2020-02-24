using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDEBeta
{
    public partial class Form1 : Form
    {
        /*Variables globales para control de texto*/
        string nombreArchivo;
        int lineas = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            label1.Text = "1\n2\n3\n";
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageList imageList = new ImageList();
            //imageList.ImageSize.Width = 16;
            //button1.Width = 16;
            //button1.Height = 16;
        }

        private void debuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void debugAndCompileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string r;
            openFileDialog1.ShowDialog();
            using (System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName))
            {
                r = file.ReadLine();
            }
            richTextBox1.Text = r.ToString();
            nombreArchivo = openFileDialog1.FileName;
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "Sin titulo.txt";
            var sf = saveFileDialog1.ShowDialog();
            saveFileDialog1.AddExtension = true;
            if (sf == DialogResult.OK)
            {
                using (var saveFile = new System.IO.StreamWriter(saveFileDialog1.FileName + ".txt"))
                {
                    saveFile.WriteLine(richTextBox1.Text);
                }
            }
            nombreArchivo = saveFileDialog1.FileName;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(nombreArchivo))
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            using (var saveFile = new System.IO.StreamWriter(nombreArchivo + ".txt"))
                saveFile.WriteLine(richTextBox1.Text);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if(richTextBox1.Lines.Length != lineas)
            {
                string nueva = "\n";
                for(int i = 1;i<richTextBox1.Lines.Length; i++)
                {
                    nueva += (i.ToString() + "\n");
                    
                }
                Console.WriteLine(nueva);
                label1.Text = nueva.ToString();
                lineas = richTextBox1.Lines.Length;
            }
            label1.Text = richTextBox1.Lines.Length.ToString();
        }
    }
}
