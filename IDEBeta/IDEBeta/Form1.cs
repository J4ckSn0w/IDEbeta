using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace IDEBeta
{
    public partial class Form1 : Form
    {
        /*Variables globales para control de texto*/
        string nombreArchivo;
        int lineas = 0;
        bool cambiosGuardados = false;

        int currentLine = 0;
        int currentCol = 0;
        /*Probando cosas para scroll*/
        public enum ScrollBarType : uint
        {
            SbHorz = 0,
            SbVert = 1,
            SbCtl = 2,
            SbBoth = 3
        }

        public enum Message : uint
        {
            WM_VSCROLL = 0x0115
        }

        public enum ScrollBarCommands : uint
        {
            SB_THUMBPOSITION = 4
        }
        public Form1()
        {
            InitializeComponent();
            richTextBox4.AutoScrollOffset = new Point(0, 0);
            this.Resize += new System.EventHandler(this.ReadOnlyRichTextBox_Resize);
            timer1.Start();

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ReadOnlyRichTextBox_Mouse);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ReadOnlyRichTextBox_Mouse);
            base.TabStop = false;
            HideCaret(this.Handle);
        }
        [DllImport("User32.dll")]
        public extern static int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("User32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        protected override void OnGotFocus(EventArgs e)
        {
            HideCaret(this.Handle);
        }

        protected override void OnEnter(EventArgs e)
        {
            HideCaret(this.Handle);
        }

        [DefaultValue(true)]
        public new bool ReadOnly
        {
            get { return true; }
            set { }
        }

        [DefaultValue(false)]
        public new bool TabStop
        {
            get { return false; }
            set { }
        }

        private void ReadOnlyRichTextBox_Mouse(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HideCaret(this.Handle);
        }

        private void ReadOnlyRichTextBox_Resize(object sender, System.EventArgs e)
        {
            HideCaret(this.Handle);

        }

        /*Fin de probando cosas*/
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageList imageList = new ImageList();
            DialogResult respuesta;
            if(cambiosGuardados == false)
            {
                respuesta = MessageBox.Show("Desea guardar los cambios en el archivo?","Guardar",MessageBoxButtons.YesNoCancel);
                if(respuesta == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                    nombreArchivo = "";
                    richTextBox1.Clear();
                }
                if(respuesta == DialogResult.No)
                {
                    nombreArchivo = "";
                    richTextBox1.Clear();
                }
            }
            nombreArchivo = "";
            richTextBox1.Clear();
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
            cambiosGuardados = true;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int nPos = GetScrollPos(richTextBox1.Handle, (int)ScrollBarType.SbVert);
            nPos <<= 16;
            uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)nPos;
            if (richTextBox1.Lines.Length + 1 != lineas)
            {
                string nueva = "1\n";
                for(int i = 2;i<richTextBox1.Lines.Length+1; i++)
                {
                    nueva += (i.ToString() + "\n");
                    
                }
                richTextBox4.Text = nueva.ToString();
                lineas = richTextBox1.Lines.Length;
                /*Probando cosas*/
                
                SendMessage(richTextBox4.Handle, (int)Message.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));
                /*Probando cosas*/
            }
            cambiosGuardados = false;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //onsole.WriteLine("Hola\n");
            int nPos = GetScrollPos(richTextBox1.Handle, (int)ScrollBarType.SbVert);
            nPos <<= 16;
            uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)nPos;
            SendMessage(richTextBox4.Handle, (int)Message.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));

            currentLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
            currentCol = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(currentLine);

            string lineaCol = "";
            lineaCol = currentLine + 1 + ":" + currentCol;
            label3.Text = lineaCol;
        }
        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveAsToolStripMenuItem_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }
    }
}
