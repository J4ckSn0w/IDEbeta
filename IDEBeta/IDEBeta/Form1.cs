using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace IDEBeta
{
    public partial class Form1 : Form
    {
        class Operadores
        {
            string valor;
            string descripcion = "Palabra Reservada";
            public Operadores(string palabra)
            {
                this.valor = palabra;
                this.descripcion = descripcion;
            }
        }
        class palabraReservada
        {
            string valor;
            string descripcion = "Palabra Reservada";
            public palabraReservada(string palabra)
            {
                this.valor = palabra;
                this.descripcion = descripcion;
            }
        }

        class caracteresEspeciales
        {
            string valor;
            string descripcion = "Palabra Reservada";
            public caracteresEspeciales(string palabra)
            {
                this.valor = palabra;
                this.descripcion = descripcion;
            }
        }
        /*Variables globales para control de texto*/
        string nombreArchivo;
        bool banderaLexico = false;
        int lineas = 1;
        bool cambiosGuardados = false;

        class token
        {
            public string tipo { get; set; }
            public string lexema { get; set; }
            public int fila { get; set; }
            public int columna { get; set; }

            /*public token(string tipo, string lexema)
            {
                tipo = tipo;
                lexema = lexema;
            }*/

            public string Tipo
            {
                get
                {
                    return tipo;
                }
                set
                {
                    tipo = value;
                }
            }

            public string Lexema
            {
                get
                {
                    return lexema;
                }
                set
                {
                    lexema = value;
                }
            }

            public int Fila
            {
                get
                {
                    return fila;
                }
                set
                {
                    fila = value;
                }
            }

            public int Columna
            {
                get
                {
                    return columna;
                }
                set
                {
                    columna = value;
                }
            }
        }

        enum Estado
        {
            Inicio,
            Final,
            Entero,
            Punto,
            Flotante,
            ID,
            Division,
            ComentarioLinea,
            ComentarioMultiple,
            FinComentarioMultiple,
            Menos,
            Mas,
            Asignacion,
            Comparacion,
            Menor,
            Mayor,
            Diferente,
            FinLectura,
            CaracterSimple
        }
        enum Token
        {
            Entero,
            Flotante,
            ID,
            PalabraReservada,
            CaracterSimple,
            Operador,
            Decremento,
            Incremento,
            Asignacion,
            Comparacion,
            Mayor,
            Menor,
            MayorIgual,
            MenorIgual,
            Diferente,
            Error
        }

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
            /*string[] argumentos = Environment.GetCommandLineArgs();
            if (argumentos.Count() > 1)
            {
                MessageBox.Show("Entro a modo consola.");
                MessageBox.Show(argumentos.Count().ToString());
                return;
            }
            else
            {*/
            InitializeComponent();
            /*}*/
            richTextBox4.AutoScrollOffset = new Point(0, 0);
            this.Resize += new System.EventHandler(this.ReadOnlyRichTextBox_Resize);
            timer1.Start();

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ReadOnlyRichTextBox_Mouse);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ReadOnlyRichTextBox_Mouse);
            base.TabStop = false;
            HideCaret(this.Handle);
            richTextBox4.Text = null;
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
        public bool ReadOnly
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
            /*string[] argumentos = Environment.GetCommandLineArgs();
            if(argumentos.Count() > 1)
            {
                MessageBox.Show("Entro a modo consola.");
                MessageBox.Show(argumentos.Count().ToString());
                foreach(var argumento in argumentos)
                {
                    MessageBox.Show(argumento);
                }
                //Console.WriteLine("Entro a modo consola.\n");
            }
            //MessageBox.Show("Cargar el form.");*/
            //this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;
            //this.MinimizeBox = false;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageList imageList = new ImageList();
            DialogResult respuesta;
            if (cambiosGuardados == false)
            {
                respuesta = MessageBox.Show("Desea guardar los cambios en el archivo?", "Guardar", MessageBoxButtons.YesNoCancel);
                if (respuesta == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                    nombreArchivo = "";
                    richTextBox1.Clear();
                }
                if (respuesta == DialogResult.No)
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)//Lexico
        {
            if (cambiosGuardados == true)
            {
                leerArchivo(nombreArchivo);
            }
            else
            {
                MessageBox.Show("Primero debe guardar los cambios");
            }
        }
        /*Estados*/
        /*0 = inicial
         1 = Entero
         2 = Punto
         3 = Flotante
         10 = finalizo*/
        Estado estado = Estado.Inicio;
        Token tokenActual = Token.Entero;
        Token tokenAnterior = Token.Entero;
        Token fuera = Token.Flotante;
        bool guardar = true;
        char caracterActual;
        string contenido;
        int posicion = 0;
        string lexemaActual = "";
        string errorActual = "";

        /*Control de archivo*/
        int linea = 1;
        int columna = 0;
        string resultado = "";

        List<token> tokensLexicos = new List<token>();
        List<token> tokensErrorLexicos = new List<token>();

        void leerArchivo(string nombreArchivo)
        {
            tokensLexicos.Clear();
            /*array de tokens*/
            //List<token> tokensLexicos = new List<token>();
            //List<token> tokensErrorLexicos = new List<token>();
            /*Diccionario Palabras Reservadas*/
            //main, if, then, else, end, do, while, cin, cout, real, int, boolean
            Dictionary<string, palabraReservada> reservadas = new Dictionary<string, palabraReservada>();
            reservadas.Add("main", new palabraReservada("main"));
            reservadas.Add("if", new palabraReservada("if"));
            reservadas.Add("then", new palabraReservada("then"));
            reservadas.Add("else", new palabraReservada("else"));
            reservadas.Add("end", new palabraReservada("end"));
            reservadas.Add("do", new palabraReservada("do"));
            reservadas.Add("while", new palabraReservada("while"));
            reservadas.Add("cin", new palabraReservada("cin"));
            reservadas.Add("cout", new palabraReservada("cout"));
            reservadas.Add("real", new palabraReservada("real"));
            reservadas.Add("int", new palabraReservada("int"));
            reservadas.Add("boolean", new palabraReservada("boolean"));
            reservadas.Add("until", new palabraReservada("until"));
            reservadas.Add("float", new palabraReservada("float"));
            /*Diccionario caracteres especiales*/
            //+ - * /  % < <= > >= == != := ( ) { } // /**/ ++ -- 
            Dictionary<string, caracteresEspeciales> especiales = new Dictionary<string, caracteresEspeciales>();
            especiales.Add("(", new caracteresEspeciales("("));
            especiales.Add(")", new caracteresEspeciales(")"));
            especiales.Add("{", new caracteresEspeciales("{"));
            especiales.Add("}", new caracteresEspeciales("}"));
            especiales.Add("[", new caracteresEspeciales("["));
            especiales.Add("]", new caracteresEspeciales("]"));
            especiales.Add(";", new caracteresEspeciales(";"));
            especiales.Add(",", new caracteresEspeciales(","));
            /*Diccionario de operadores*/
            Dictionary<string, Operadores> operadores = new Dictionary<string, Operadores>();
            operadores.Add("*", new Operadores("*"));
            operadores.Add("%", new Operadores("%"));
            operadores.Add("^", new Operadores("^"));
            //MessageBox.Show("Entre a leer Archivo");                
            System.IO.StreamReader file = new System.IO.StreamReader(nombreArchivo);
            contenido = file.ReadToEnd();
            file.Close();
            /*Analisis Lexico*/
            char ch = 'a';
            var contenidoChar = contenido.ToCharArray();
            while (/*posicion < contenido.Length - 2*/(int)ch != 3)
            {
                estado = Estado.Inicio;
                //MessageBox.Show(ch.ToString());
                guardar = true;
                while (estado != Estado.Final && (int)ch != 3)
                {
                    //MessageBox.Show("Token antes de leer el siguiente caracter "+tokenActual.ToString());
                    ch = siguienteCaracter(posicion++);
                    columna++;
                    guardar = true;
                    //MessageBox.Show(((int)ch).ToString());
                    switch (estado)
                    {
                        case Estado.Inicio:
                            //MessageBox.Show("Entre a Inicio");                                
                            tokenActual = Token.Error;
                            if (char.IsDigit(ch))
                            {
                                estado = Estado.Entero;
                                tokenActual = Token.Entero;
                                //MessageBox.Show("Digito "+ch.ToString());
                            }
                            else if (char.IsLetter(ch))
                            {
                                //main
                                estado = Estado.ID;
                                tokenActual = Token.ID;
                                //MessageBox.Show("Entre en es letra");
                            }
                            else if (ch.ToString() == "\n")
                            {
                                linea++;
                                columna = 0;
                                guardar = false;
                                estado = Estado.Inicio;
                            }
                            //cout; //86

                            //cout;
                            //
                            else if (ch.ToString() == " " || (int)ch == 13 || ch.ToString() == "\t")
                            {
                                guardar = false;
                                estado = Estado.Inicio;
                            }
                            else if (especiales.ContainsKey(ch.ToString()))
                            {
                                estado = Estado.Final;
                                tokenActual = Token.CaracterSimple;
                            }
                            else if (ch.ToString() == "/")
                            {
                                //MessageBox.Show("Entre en division");
                                //MessageBox.Show("Posicion " + posicion);
                                //MessageBox.Show(ch.ToString());
                                estado = Estado.Division;
                                tokenActual = Token.Operador;
                            }
                            else if (ch.ToString() == "-")
                            {
                                estado = Estado.Menos;
                                tokenActual = Token.Operador;
                            }
                            else if (ch.ToString() == "+")
                            {
                                estado = Estado.Mas;
                                tokenActual = Token.Operador;
                            }
                            else if (ch.ToString() == ":")
                            {
                                estado = Estado.Asignacion;
                                tokenActual = Token.Asignacion;
                            }
                            else if (ch.ToString() == "=")
                            {
                                estado = Estado.Comparacion;
                                tokenActual = Token.Error;
                                //MessageBox.Show("Entre al igual");
                            }
                            else if (ch.ToString() == "<")
                            {
                                estado = Estado.Menor;
                                tokenActual = Token.Menor;
                            }
                            else if (ch.ToString() == ">")
                            {
                                estado = Estado.Mayor;
                                tokenActual = Token.Mayor;
                            }
                            else if (ch.ToString() == "!")
                            {
                                estado = Estado.Diferente;
                                tokenActual = Token.Diferente;
                            }
                            /*else if(ch.ToString() == "#")//control fin del texto
                            {
                                guardar = false;
                                estado = Estado.Final;
                            }*/
                            else if (operadores.ContainsKey(ch.ToString()))
                            {
                                estado = Estado.Final;
                                tokenActual = Token.Operador;
                            }
                            else
                            {
                                //MessageBox.Show("Entre al ELSE");
                                //guardar = false;
                                estado = Estado.Final;
                                tokenActual = Token.Error;
                                //lexemaActual = "";
                            }
                            break;
                        case Estado.Entero:
                            //MessageBox.Show("Entre en entero");
                            tokenActual = Token.Entero;
                            if (Char.IsDigit(ch))
                            {
                                break;
                            }
                            if (ch == '.')
                            {
                                tokenActual = Token.Flotante;
                                estado = Estado.Punto;
                            }
                            else
                            {
                                //MessageBox.Show("Entre en else");
                                posicion--;
                                columna--;
                                guardar = false;
                                estado = Estado.Final;
                                tokenActual = Token.Entero;
                            }
                            break;
                        case Estado.Punto:
                            if (!Char.IsDigit(ch))
                            {
                                guardar = false;
                                posicion--;
                                columna--;
                                valorEsperado();
                                tokenActual = Token.Error;
                                estado = Estado.Final;
                                break;
                            }
                            estado = Estado.Flotante;
                            tokenActual = Token.Flotante;
                            break;
                        case Estado.Flotante:
                            //MessageBox.Show("Char Actual " + ch.ToString());
                            if (!Char.IsDigit(ch))
                            {
                                posicion--;
                                columna--;
                                guardar = false;
                                estado = Estado.Final;
                                tokenActual = Token.Flotante;
                                break;
                            }
                            tokenActual = Token.Flotante;
                            break;
                        case Estado.ID:
                            if (!Char.IsLetterOrDigit(ch) && (int)ch != 95)
                            {
                                //main(
                                posicion--;
                                columna--;
                                guardar = false;
                                estado = Estado.Final;
                                tokenActual = Token.ID;
                                break;
                            }
                            tokenActual = Token.ID;
                            break;
                        case Estado.Division:
                            if (ch.ToString() == "/")
                            {
                                estado = Estado.ComentarioLinea;
                                //lexemaActual.Remove(lexemaActual.Length - 1, 1);
                                lexemaActual = "";
                                guardar = false;
                                break;
                            }
                            if (ch.ToString() == "*")
                            {
                                estado = Estado.ComentarioMultiple;
                                //lexemaActual.Remove(lexemaActual.Length - 1, 1);
                                lexemaActual = "";
                                posicion--;
                                columna--;
                                guardar = false;
                                break;
                            }
                            estado = Estado.Final;
                            tokenActual = Token.Operador;
                            guardar = false;
                            posicion--;
                            columna--;
                            break;
                        case Estado.ComentarioLinea:
                            //MessageBox.Show("Entre en comentario de linea");
                            if (ch.ToString() == "\n")
                            {
                                linea++;
                                columna = 0;
                                estado = Estado.Inicio;
                                guardar = false;
                                break;
                            }
                            //MessageBox.Show("LexemaActual " + lexemaActual.ToString());
                            guardar = false;
                            break;
                        case Estado.ComentarioMultiple:
                            if (ch.ToString() == "*")
                            {
                                estado = Estado.FinComentarioMultiple;
                                guardar = false;
                                break;
                            }
                            if (ch.ToString() == "\n")
                            {
                                linea++;
                                columna = 0;
                            }
                            guardar = false;
                            break;
                        case Estado.FinComentarioMultiple:
                            if (ch.ToString() == "/")
                            {
                                estado = Estado.Inicio;
                                guardar = false;
                                break;
                            }
                            if (ch.ToString() != "*")
                            {
                                estado = Estado.ComentarioMultiple;
                                guardar = false;
                                break;
                            }
                            guardar = false;
                            break;
                        case Estado.Menos:
                            estado = Estado.Final;
                            if (ch.ToString() == "-")
                            {
                                tokenActual = Token.Decremento;
                                break;
                            }
                            guardar = false;
                            posicion--;
                            columna--;
                            tokenActual = Token.Operador;
                            break;
                        case Estado.Mas:
                            estado = Estado.Final;
                            if (ch.ToString() == "+")
                            {
                                tokenActual = Token.Incremento;
                                break;
                            }
                            guardar = false;
                            posicion--;
                            columna--;
                            tokenActual = Token.Operador;
                            break;
                        case Estado.Asignacion:
                            if (ch.ToString() == "=")
                            {
                                estado = Estado.Final;
                                guardar = true;
                                tokenActual = Token.Asignacion;
                                break;
                            }
                            posicion--;
                            columna--;
                            guardar = false;
                            valorEsperado();
                            estado = Estado.Final;
                            tokenActual = Token.Error;
                            break;
                        case Estado.Comparacion:
                            if (ch.ToString() == "=")
                            {
                                //MessageBox.Show("Entre a la igualacion");
                                tokenActual = Token.Comparacion;
                                estado = Estado.Final;
                                break;
                            }
                            posicion--;
                            columna--;
                            guardar = false;
                            valorEsperado();
                            estado = Estado.Final;
                            tokenActual = Token.Error;
                            break;
                        case Estado.Menor:
                            if (ch.ToString() == "=")
                            {
                                tokenActual = Token.MenorIgual;
                                estado = Estado.Final;
                                break;
                            }
                            posicion--;
                            columna--;
                            guardar = false;
                            tokenActual = Token.Menor;
                            estado = Estado.Final;
                            break;
                        case Estado.Mayor:
                            if (ch.ToString() == "=")
                            {
                                tokenActual = Token.MayorIgual;
                                estado = Estado.Final;
                                break;
                            }
                            posicion--;
                            columna--;
                            guardar = false;
                            tokenActual = Token.Mayor;
                            estado = Estado.Final;
                            break;
                        case Estado.Diferente:
                            if (ch.ToString() == "=")
                            {
                                tokenActual = Token.Diferente;
                                estado = Estado.Final;
                                break;
                            }
                            posicion--;
                            columna--;
                            guardar = false;
                            valorEsperado();
                            tokenActual = Token.Error;
                            estado = Estado.Final;
                            break;
                        default:
                            //MessageBox.Show("Entre en default");
                            estado = Estado.Final;
                            break;
                    }
                    if (guardar && (int)ch != 3)
                    {
                        //MessageBox.Show("Guardamos " + ch.ToString());
                        lexemaActual += ch;
                    }
                }
                //MessageBox.Show("Operador: " + tokenActual.ToString());
                /*if((int)ch == 3)
                {
                    MessageBox.Show("Dentro de if final de caracter " + fuera.ToString());
                    tokenActual = fuera;
                }*/
                if (!string.IsNullOrEmpty(lexemaActual))
                {
                    if (reservadas.ContainsKey(lexemaActual))
                    {
                        tokenActual = Token.PalabraReservada;
                    }
                    //Forma de guardarlos antes de solo guardar tokens validos en el resultado
                    /*
                        resultado += tokenActual.ToString();
                        resultado += "->";
                        resultado += lexemaActual;
                    */
                    if (tokenActual == Token.Error)
                    {
                        tokensErrorLexicos.Add(new token()
                        {
                            Tipo = tokenActual.ToString(),
                            Lexema = lexemaActual.ToString() + errorActual + " en linea " + linea + " columna " + columna,
                            Fila = linea,
                            Columna = columna
                        });
                        //resultado += " en linea " + linea + " columna " + columna;
                        //AGREGAR A TOKENS DE ERRORES, CON MENSAJE
                        //if (!string.IsNullOrEmpty(errorActual))
                        //{
                        //    resultado += errorActual;
                        errorActual = "";
                        lexemaActual = "";
                        //}
                    }
                    else
                    {
                        resultado += tokenActual.ToString();
                        resultado += "->";
                        resultado += lexemaActual;
                        resultado += '\n';
                        //MessageBox.Show(resultado);
                        /*Guardamos el token en el array de tokens*/
                        //tokensLexicos.Add(new token(tokenActual.ToString(), lexemaActual.ToString()));
                        tokensLexicos.Add(new token()
                        {
                            Tipo = tokenActual.ToString(),
                            Lexema = lexemaActual.ToString(),
                            Fila = linea,
                            Columna = columna
                        });
                        lexemaActual = "";
                    }
                }
            }
            //MessageBox.Show("Justo despues de salir");
            //MessageBox.Show(estado.ToString());
            if (estado != Estado.Final && !string.IsNullOrEmpty(lexemaActual))
            {
                //main
                if (reservadas.ContainsKey(lexemaActual))
                {
                    tokenActual = Token.PalabraReservada;
                }
                //Forma de guardarlos antes de solo guardar tokens validos en el resultado
                /*
                resultado += tokenActual.ToString();
                resultado += "->";
                resultado += lexemaActual;
                */
                if (tokenActual == Token.Error)
                {
                    tokensErrorLexicos.Add(new token()
                    {
                        Tipo = tokenActual.ToString(),
                        Lexema = lexemaActual.ToString() + errorActual + " en linea " + linea + " columna " + columna,
                        Fila = linea,
                        Columna = columna
                    });
                    //resultado += " en linea " + linea + " columna " + columna;
                    //AGREGAR A TOKENS DE ERRORES, CON MENSAJE
                    //if (!string.IsNullOrEmpty(errorActual))
                    //{
                    //    resultado += errorActual;
                    errorActual = "";
                    lexemaActual = "";
                    //}
                }
                else
                {
                    resultado += tokenActual.ToString();
                    resultado += "->";
                    resultado += lexemaActual;
                    resultado += '\n';
                    tokensLexicos.Add(new token()
                    {
                        Tipo = tokenActual.ToString(),
                        Lexema = lexemaActual.ToString(),
                        Fila = linea,
                        Columna = columna
                    });
                    lexemaActual = "";
                }
            }
            if (!string.IsNullOrEmpty(lexemaActual))
            {
                //Forma de guardarlos antes de solo guardar tokens validos en el resultado
                /*
                resultado += tokenActual.ToString();
                resultado += "->";
                resultado += lexemaActual;
                */
                if (tokenActual == Token.Error)
                {
                    tokensErrorLexicos.Add(new token()
                    {
                        Tipo = tokenActual.ToString(),
                        Lexema = lexemaActual.ToString() + errorActual + " en linea " + linea + " columna " + columna,
                        Fila = linea,
                        Columna = columna
                    });
                    //resultado += " en linea " + linea + " columna " + columna;
                    //if (!string.IsNullOrEmpty(errorActual))
                    //{
                    //    resultado += errorActual;
                    errorActual = "";
                    lexemaActual = "";
                    //}

                    //AGREGAR A TOKENS DE ERRORES, CON MENSAJE

                    //resultado += '\n';
                }
                //resultado += '\n';
                else
                {
                    resultado += tokenActual.ToString();
                    resultado += "->";
                    resultado += lexemaActual;
                    resultado += '\n';
                    tokensLexicos.Add(new token()
                    {
                        Tipo = tokenActual.ToString(),
                        Lexema = lexemaActual.ToString(),
                        Fila = linea,
                        Columna = columna
                    });
                    lexemaActual = "";
                }
            }
            guardarResultado(resultado);
            posicion = 0;
            columna = 0;
            linea = 1;
            ch = 'a';
            lexico.Clear();
            lexico.Text = resultado;
            resultado = "";//Limpiamos el resultado
            System.Diagnostics.Debug.WriteLine(resultado);
            if (tokensErrorLexicos.Count > 0)
            {
                string lexicos = "";
                foreach (var error in tokensErrorLexicos)
                {
                    lexicos += error.Lexema + "\n";
                }
                erroresLexicos.Text = lexicos;
            }
            //Imprimir tokens
            //foreach(var tok in tokensLexicos)
            //{
            //    Console.WriteLine("Fila: " + tok.fila + "Columna:" + tok.columna + "\n");
            //}

        }
        void valorEsperado()
        {
            switch (estado)
            {
                case Estado.Punto:
                    errorActual = " Se esperaba un digito";
                    break;
                case Estado.Asignacion:
                    errorActual = " Se esperaba un =";
                    break;
                case Estado.Comparacion:
                    errorActual = " Se esperaba un =";
                    break;
                case Estado.Diferente:
                    errorActual = " Se esperaba un =";
                    break;
            }
        }
        char siguienteCaracter(int posicionActual)
        {
            if (posicionActual == contenido.Length)
            {
                //MessageBox.Show("Entre a el size");
                //MessageBox.Show("tokenActual " + tokenActual.ToString());
                //fuera = tokenActual;
                return (char)3;
            }
            var ch = contenido[posicionActual];
            if (ch == null)
            {
                //MessageBox.Show("Entre a el else de ch");
            }
            return ch;
        }
        ///Guardar resultado en un archivo de texto
        void guardarResultado(string contenidoTerminado)
        {
            string fecha = DateTime.Now.ToString();
            fecha = fecha.Replace("/", "-");
            fecha = fecha.Replace(" ", "");
            fecha = fecha.Replace(".", "");
            fecha = fecha.Replace(":", "-");
            using (var saveFile = new System.IO.StreamWriter("Resultados-" + fecha + ".txt"))
            {
                saveFile.WriteLine(contenidoTerminado);
                //MessageBox.Show("Despues de escribir el contenido");
            }
            using (var saveFile = new System.IO.StreamWriter("Resultados.txt"))
            {
                saveFile.WriteLine(contenidoTerminado);
                //MessageBox.Show("Despues de escribir el contenido");
            }
            //MessageBox.Show("Terminar Leer Archivos");
        }

        /// <summary>
        /// Sintactico, funciones sintacticas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>




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
            /*string r;
            openFileDialog1.ShowDialog();
            System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
            r = file.ReadLine();
            Console.WriteLine("Contenido:\n");
            Console.WriteLine(r.ToString());
            richTextBox1.Text = r.ToString();
            nombreArchivo = openFileDialog1.FileName;
            //Console.WriteLine(nombreArchivo);*/
            string contentFile;

            //Se muestra el explorador de archivos para buscar el archivo
            openFileDialog1.ShowDialog();

            string aux = openFileDialog1.FileName;
            if (aux != null && aux != "")
            {
                System.IO.StreamReader file = new System.IO.StreamReader(aux);
                contentFile = file.ReadToEnd();
                richTextBox1.Text = contentFile;

                //Se actualiza el nombre
                this.nombreArchivo = aux;
                file.Close();
            }
            richTextBox1_TextChanged(sender, e);
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
            cambiosGuardados = true;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            using (var saveFile = new System.IO.StreamWriter(nombreArchivo))
                saveFile.WriteLine(richTextBox1.Text);
            using (var saveFile = new System.IO.StreamWriter(nombreArchivo))
                saveFile.WriteLine(richTextBox1.Text);
            cambiosGuardados = true;
        }

        class comentarios
        {
            public int cierre { get; set; }
            public int apertura { get; set; }
            /*public token(string tipo, string lexema)
            {
                tipo = tipo;
                lexema = lexema;
            }*/

            public int Cierre
            {
                get
                {
                    return cierre;
                }
                set
                {
                    cierre = value;
                }
            }

            public int Apertura
            {
                get
                {
                    return apertura;
                }
                set
                {
                    apertura = value;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int nPos = GetScrollPos(richTextBox1.Handle, (int)ScrollBarType.SbVert);
            nPos <<= 16;
            uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)nPos;
            /*
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
            /*Probando cosas
        }*/

            //Codigo de Fuantos
            if (richTextBox1.Lines.Count() != lineas - 1)
            {
                //this.numLines = richTextBox1.Lines.Count();
                //this.label1.Text = "";
                /*for (int i = 1; i < this.numLines; i++)
                {
                    //  this.label1.Text += (i).ToString() + "\n";
                }*/

                if (richTextBox1.Lines.Count() > this.lineas - 1)
                {
                    while (richTextBox1.Lines.Count() > this.lineas - 1)
                    {
                        var li = this.lineas.ToString();

                        richTextBox4.AppendText(li + Environment.NewLine);
                        this.lineas++;
                    }
                }

                if (richTextBox1.Lines.Count() < this.lineas - 1)
                {
                    while (richTextBox1.Lines.Count() < this.lineas - 1)
                    {
                        List<string> myList = richTextBox4.Lines.ToList();
                        if (myList.Count > 0)
                        {
                            myList.RemoveAt(myList.Count - 2);
                            richTextBox4.Lines = myList.ToArray();
                            richTextBox4.Refresh();
                            this.lineas--;
                            if (this.lineas == 0)
                            {
                                this.lineas++;
                            }
                        }

                    }
                }
                //this.lineas = richTextBox1.Lines.Count();
            }
            cambiosGuardados = false;
            /*Pintar palabras reservadas en richtext*/
            //richTextBox1.Handle.ToInt32;
            //richTextBox1.SelectionStart = 0;
            //richTextBox1.SelectionLength = richTextBox1.TextLength;
            //richTextBox1.SelectionColor = richTextBox1.ForeColor;
            //if(!string.IsNullOrEmpty(richTextBox1.Text))
            //{
            /*
                richTextBox1.SelectAll();
                richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, FontStyle.Regular);
                int ocurrencias = 0;
                string palabra = "int ";
                string palabraModificada = @"\b"+palabra+@"\w*\b";
                foreach(Match coincidencia in Regex.Matches(richTextBox1.Text,palabraModificada,RegexOptions.IgnoreCase))
                {
                    int seleccion = coincidencia.Index;
                    richTextBox1.SelectionStart = seleccion;
                    richTextBox1.SelectionLength = coincidencia.Value.Length;
                    richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, FontStyle.Bold);
                }*/
            string[] test = { "int", "float", "real", "boolean", "if", "else", "then", "while", "until", "main", "end", "do", "cin", "cout" };
            string[] words = { "int ", "float ", "real ", "boolean ", "if ", "else ", "then ", "while ", "until ","main ","end;","do ", "cin ","cout "
            ,"int"+'\n', "float"+'\n', "real"+'\n', "boolean"+'\n', "if"+'\n', "else"+'\n', "then"+'\n', "while"+'\n', "until"+'\n',"main+'\n'","end"+'\n',"do"+'\n', "cin"+'\n',"cout"+'\n'
            ,"int"+'\t', "float"+'\t', "real"+'\t', "boolean"+'\t', "if"+'\t', "else"+'\t', "then"+'\t', "while"+'\t', "until"+'\t',"main+'\t'","end"+'\t',"do"+'\t', "cin"+'\t',"cout"+'\t'};

            string[] wordsWithP = { "if(", "else{", "until(", "while(", "then(", "main(" };

            string[] comentarios = { "//", "/*" };
            // Get the current caret position.
            currentLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
            //currentCol = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(currentLine);
            int posicionActual = richTextBox1.SelectionStart;// - richTextBox1.GetFirstCharIndexFromLine(currentLine);

            Dictionary<int, comentarios> diccionarioCierres = new Dictionary<int, comentarios>();
            Dictionary<int, int> diccionarioAperturas = new Dictionary<int, int>();

            if (!string.IsNullOrEmpty(richTextBox1.Text.ToString()))
            {
                richTextBox4.Focus();
                var richTextBoxAux = new RichTextBox();
                richTextBoxAux = richTextBox1;
                /*Cambios para evitar parpadeo*/
                richTextBoxAux.SelectAll();
                //richTextBox1.Select(richTextBox1.TextLength, 0);
                richTextBoxAux.SelectionColor = Color.Black;
                richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                //richTextBoxAux.Select(posicionActual, 0);
                string textoBuscador = richTextBoxAux.Text;
                bool pintaturaya = false;
                string find = "";
                foreach (string word in /*words*/test)
                {

                    find = word;
                    if (richTextBoxAux.Text.Contains(find))
                    {
                        var matchString = Regex.Escape(find);
                        foreach (Match match in Regex.Matches(richTextBoxAux.Text, matchString))
                        {
                            /*Probando cosas*/
                            if (match.Index > 0)
                            {
                                if (match.Index + find.Length < textoBuscador.Length)
                                {
                                    Char ch = textoBuscador[match.Index - 1];
                                    Char ch2 = textoBuscador[match.Index + find.Length];
                                    if (!Char.IsLetterOrDigit(textoBuscador[match.Index - 1]) && !Char.IsLetterOrDigit(textoBuscador[match.Index + find.Length]))
                                    {
                                        richTextBoxAux.Select(match.Index, find.Length);
                                        richTextBoxAux.SelectionColor = Color.Blue;
                                        richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                                        //richTextBoxAux.Select(posicionActual, 0);
                                        richTextBoxAux.SelectionColor = richTextBoxAux.ForeColor;
                                    }
                                }
                            }
                            if (match.Index + find.Length <= textoBuscador.Length - 1)
                            {
                                Char ch2 = textoBuscador[match.Index + find.Length];
                                if (!Char.IsLetterOrDigit(textoBuscador[match.Index + find.Length]))
                                {
                                    richTextBoxAux.Select(match.Index, find.Length);
                                    richTextBoxAux.SelectionColor = Color.Blue;
                                    richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                                    //richTextBoxAux.Select(posicionActual, 0);
                                    richTextBoxAux.SelectionColor = richTextBoxAux.ForeColor;
                                }
                            }
                            /*richTextBoxAux.Select(match.Index, find.Length);
                            richTextBoxAux.SelectionColor = Color.Blue;
                            richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                            //richTextBoxAux.Select(posicionActual, 0);
                            richTextBoxAux.SelectionColor = richTextBoxAux.ForeColor;*/
                        };
                    }
                }

                /*Casos especiales*/
                richTextBox4.Focus();
                //richTextBoxAux = new RichTextBox();
                //richTextBoxAux = richTextBox1;
                /*Cambios para evitar parpadeo*/
                //richTextBoxAux.SelectAll();
                //richTextBox1.Select(richTextBox1.TextLength, 0);
                //richTextBoxAux.SelectionColor = Color.Black;
                //richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                //richTextBoxAux.Select(posicionActual, 0);
                find = "";
                foreach (string word in wordsWithP)
                {

                    find = word;
                    if (richTextBoxAux.Text.Contains(find))
                    {
                        var matchString = Regex.Escape(find);
                        foreach (Match match in Regex.Matches(richTextBoxAux.Text, matchString))
                        {
                            richTextBoxAux.Select(match.Index, find.Length);
                            richTextBoxAux.SelectionColor = Color.Blue;
                            richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                            //richTextBoxAux.Select(posicionActual, 0);
                            richTextBoxAux.SelectionColor = richTextBoxAux.ForeColor;
                        };
                    }
                }
                /*caso especial main*/

                richTextBox1 = richTextBoxAux;
                /*Multiples lineas*/
                find = "/*";
                string find2 = "*/";
                if (richTextBoxAux.Text.Contains(find))
                {
                    var matchString = Regex.Escape(find);
                    foreach (Match match in Regex.Matches(richTextBoxAux.Text, matchString))
                    {
                        if (!diccionarioCierres.ContainsKey(match.Index))
                        {
                            int ocurrencias1 = match.Length;
                            ocurrencias1 = ocurrencias1 / 2;
                            int busca = 1;
                            int ocurrencias2 = 0;
                            var matchString2 = Regex.Escape(find2);
                            foreach (Match match2 in Regex.Matches(richTextBoxAux.Text, matchString2))
                            {
                                //int ocurrencias2 = match2.Length / 2;
                                if (match2.Index + 2 > match.Index)
                                {
                                    busca += match2.Index - match.Index + 1;
                                    if (busca >= 4)
                                    {
                                        ocurrencias2++;
                                        if (!diccionarioCierres.ContainsKey(match.Index) && !diccionarioCierres.ContainsKey(match2.Index - 1))
                                        {
                                            diccionarioCierres.Add(match2.Index - 1, new comentarios() { Cierre = match2.Index - 1, Apertura = match.Index });
                                            //diccionarioCierres.Add(match2.Index + 1, match2.Index + 1);
                                            //diccionarioAperturas.Add(match.Index, match.Index);
                                        }

                                        break;
                                    }
                                    busca = 1;

                                }


                            }
                            string texto = richTextBoxAux.Text;
                            int i = match.Index - 1;
                            if (i < 0)
                            {
                                i = 0;
                            }
                            for (; texto[i] != '\0' && i < texto.Length - 1 && ocurrencias1 > ocurrencias2; i++)
                            {
                                busca++;
                            }

                            //string texto = richTextBoxAux.Text;

                            //int otroIndex = richTextBox1.Find(richTextBox1.Text,match.Index,richTextBox1.matchCase);
                            /*
                            int i = 9;
                            var a = texto[i];
                            var b = texto[i - 1];
                            for (i = match.Index + 2; (texto[i-2] != '*' && texto[i-1] != '/'/* && texto[i-2] != '*') && texto[i-1] != '\0' && i - 1 < texto.Length - 1; i++)
                            {
                                busca++;
                            }*/
                            richTextBoxAux.Select(match.Index, busca);
                            richTextBoxAux.SelectionColor = Color.Green;
                            richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                            //richTextBoxAux.Select(posicionActual, 0);
                            richTextBoxAux.SelectionColor = richTextBoxAux.ForeColor;
                        }

                    };
                }

                find = "//";
                if (richTextBoxAux.Text.Contains(find))
                {
                    var matchString = Regex.Escape(find);
                    foreach (Match match in Regex.Matches(richTextBoxAux.Text, matchString))
                    {
                        /**/
                        bool banderaSimple = true;
                        foreach (var pos in diccionarioCierres)
                        {
                            if (match.Index > pos.Value.apertura && match.Index < pos.Value.cierre)
                            {
                                banderaSimple = false;
                            }
                        }
                        if (banderaSimple == true)
                        {
                            string texto = richTextBoxAux.Text;
                            int busca = 2;
                            //int otroIndex = richTextBox1.Find(richTextBox1.Text,match.Index,richTextBox1.matchCase);
                            for (int i = match.Index + 1; texto[i] != '\n' && texto[i] != '\0' && i < texto.Length - 1; i++)
                            {
                                busca++;
                            }
                            richTextBoxAux.Select(match.Index, busca);
                            richTextBoxAux.SelectionColor = Color.Green;
                            //richTextBoxAux.Select(richTextBoxAux.TextLength, 0);
                            richTextBoxAux.Select(posicionActual, 0);
                            richTextBoxAux.SelectionColor = richTextBoxAux.ForeColor;
                        }

                    };
                }
                richTextBox1 = richTextBoxAux;
                if (posicionActual >= 0)
                    richTextBox1.Select(posicionActual, 0);

            }
            richTextBox1.Focus();
            //}
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

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void lexico_TextChanged(object sender, EventArgs e)
        {

        }
        //Sintactico

        public class nodo
        {
            public double valor;
            public string nombre;
            public string tipo;
            public string atributo;
            public double atributoDouble;
            public string tipoNodo { get; set; }
            public string tipoVariable { get; set; }
            //public nodo[] hijos = new nodo[3];
            public List<nodo> hijos = new List<nodo>();
            public nodo izquierdo;
            public nodo derecho;

            public  int linea { get; set; }

            public string Atributo
            {
                get
                {
                    return atributo;
                }
                set
                {
                    atributo = value;
                }
            }

            public double Valor
            {
                get
                {
                    return valor;
                }
                set
                {
                    valor = value;
                }
            }

            public string Tipo
            {
                get
                {
                    return tipo;
                }
                set
                {
                    tipo = value;
                }
            }

            public string Nombre
            {
                get
                {
                    return nombre;
                }
                set
                {
                    nombre = value;
                }
            }

            public List<nodo> Hijos
            {
                get
                {
                    return hijos;
                }
                set
                {
                    hijos = value;
                }
            }
        }

        token tokenSintactico = new token();
        int posicionToken = 0;
        string erroresSintacticos = "";
        public void nextToken()
        {
            //match
            if (posicionToken < tokensLexicos.Count)
            {
                tokenSintactico = tokensLexicos[posicionToken];
                posicionToken++;
            }
            else
            {
                tokenSintactico.lexema = "EOF";
            }
        }

        public TreeNode verArbol(nodo arbol, TreeView tree)
        {
            if (arbol != null)
            {
                TreeNode nuevoT = new TreeNode();
                if (arbol.Nombre == null)
                {
                    return null;
                }
                if (arbol.Tipo == "factor" || arbol.Tipo == "ID")
                {
                    string x = arbol.Valor.ToString();
                    nuevoT.Text = arbol.Nombre.ToString();
                    return nuevoT;
                }
                nuevoT.Text = arbol.Nombre.ToString();
                var hijos = arbol.Hijos.Count;
                for (int i = 0; i < hijos; i++)
                {
                    if (arbol.Hijos[i] != null && arbol.Hijos[i].nombre != null)
                        nuevoT.Nodes.Add(verArbol(arbol.Hijos[i], tree));
                }
                return nuevoT;
            }
            return null;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            nextToken();
            nodo raiz = null;
            //2+3+5*10
            raiz = programa();
            raizSemantica = raiz;
            //Console.WriteLine(resultado + "\n");
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            var t = verArbol(raiz, treeView1);
            if (t != null)
                treeView1.Nodes.Add(t);
            treeView1.EndUpdate();
            treeView1.ExpandAll();
            posicionToken = 0;
        }

        public void getToken()
        {
            if (posicionToken < tokensLexicos.Count)
            {
                tokenSintactico = tokensLexicos[posicionToken];
                posicionToken++;
            }
            else
            {

                tokenSintactico.Tipo = "fin";
                tokenSintactico.lexema = "EOF";

            }

        }

        public void error(string c)
        {
            Console.WriteLine("Error de sintaxis {0}", c);
            /*if(tokenSintactico.Columna == 0)
            {
                if(tokenSintactico.Fila > 0)
                {
                    erroresSintacticos += "Error de sintaxis, se esperaba " + c + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                    tokenSintactico.lexema = "ERROR";
                }
            }*/
            erroresSintacticos += "Error de sintaxis, se esperaba " + c + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
            //tokenSintactico.lexema = "ERROR";
            lineaError = tokenSintactico.Fila;
        }

        public void match(string expectedToken)
        {
            if (tokenSintactico == null)
            {
                error(expectedToken);
            }
            else if (tokenSintactico.lexema.Equals(expectedToken))
            {
                getToken();
            }
            // token = getchar();//treae el toke de la entrada estandar, en el copilador lo traera de su lexico
            else
            {
                error(expectedToken);
            }
        }

        public nodo programa()
        {
            erroresSintacticos = "";
            //Inicio del programa, leemos main seguido de {}
            //nextToken();
            nodo t = new nodo();
            //match("main");
            if (tokenSintactico.lexema == "main")
            {
                t.Nombre = tokenSintactico.lexema;
            }
            else
            {
                t.Nombre = "raiz auxiliar";
            }
            //nextToken();
            match("main");
            match("{");
            //if(tokenSintactico.lexema == "{")
            //{
            //nextToken();
            t.Hijos.Add(lista_declaracion());
            t.Hijos.Add(secuencia_sent());
            match("}");
            //return t;
            /*if(tokenSintactico.lexema == "}")
            {
                return t;
            }
            else
            {
                //Error, debe de terminar con un }
            }*/
            //}
            //else
            //{
            //Erro, despues de main se necesita '{'
            //}
            //}
            //else
            //{
            //    erroresSintacticos += "Error de sintaxis, se esperaba main en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
            //    //Error, se necesita iniciar con main
            //}
            //t.Nombre = "main";
            //t.Hijos.Add(lista_declaracion());
            //t.Hijos[1] = secuencia_sent();
            //t.Hijos.Add(secuencia_sent());
            richTextBox2.Text = "";
            richTextBox2.Text = erroresSintacticos;
            return t;
        }

        public nodo lista_declaracion()
        {
            nodo nuevo = new nodo();
            nuevo.Nombre = "declaracion";
            nuevo.tipoNodo = "declaracion";
            while (tokenSintactico.lexema == "int" || tokenSintactico.lexema == "float" || tokenSintactico.lexema == "boolean")
            {
                nodo otro = new nodo();
                otro.Nombre = tokenSintactico.lexema;
                otro.Tipo = "variable";
                otro.linea = tokenSintactico.Fila;
                //Agregando para el semantico
                otro.tipoNodo = tokenSintactico.lexema;
                nextToken();
                nodo primero = new nodo();
                primero.Nombre = tokenSintactico.lexema;
                primero.Tipo = "ID";
                primero.linea = tokenSintactico.Fila;
                //Propagacion de tipo
                primero.tipoNodo = otro.tipoNodo;
                primero.tipoVariable = otro.tipoNodo;
                otro.Hijos.Add(primero);
                nextToken();
                var posAnterior = new token();
                while (tokenSintactico.lexema != ";" && tokenSintactico.lexema == ",")
                {
                    nextToken();
                    nodo variable = new nodo();
                    variable.Nombre = tokenSintactico.lexema;
                    variable.Tipo = "ID";
                    variable.linea = tokenSintactico.Fila;
                    variable.tipoNodo = otro.tipoNodo;
                    variable.tipoVariable = otro.tipoNodo;
                    otro.Hijos.Add(variable);
                    posAnterior = tokenSintactico;
                    nextToken();
                }
                //nextToken();
                if (tokenSintactico.lexema == ";")
                {
                    match_nuevo(";");
                }
                else
                {
                    erroresSintacticos += "Error de sintaxis, se esperaba ; en Linea: " + posAnterior.Fila + ", " + Convert.ToInt32(posAnterior.Columna) + "\n";
                }
                nuevo.Hijos.Add(otro);
            }
            return nuevo;
        }
        int lineaError = 0;
        public nodo secuencia_sent()
        {
            nodo temp = new nodo();
            temp.Nombre = "sentencias";
            nodo t = null;//secuencia();
            nodo p = t;

            temp.tipoNodo = "statement";

            while (tokenSintactico.lexema != "end" && tokenSintactico.lexema != "else" && tokenSintactico.lexema != "until" && tokenSintactico.lexema != "EOF" && tokenSintactico.lexema != "}")
            {
                nodo q = new nodo();
                //if(tokenSintactico.lexema == ";")
                //{
                if (tokenSintactico.lexema == "ERROR" || lineaError != 0)
                {
                    //lineaError = tokenSintactico.Fila;
                    while (lineaError == tokenSintactico.Fila)
                    {
                        nextToken();
                    }
                    lineaError = 0;
                }
                else
                {
                    lineaError = 0;
                    //nextToken();
                    if (t != null)
                    {
                        temp.Hijos.Add(t);
                    }
                    q = secuencia();
                    temp.Hijos.Add(q);
                    t = null;
                }
                //lineaError = 0;
                //}
                //else
                //{
                //    erroresSintacticos += "Error de sintaxis, token inesperado:" + tokenSintactico.lexema + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                //    nextToken();
                //}
            }
            return temp;
        }

        public nodo secuencia()
        {
            nodo temp = new nodo();
            switch (tokenSintactico.lexema)
            {
                case "if":
                    temp = ifPrueba();
                    break;
                case "do":
                    temp = repeat();
                    match_nuevo(";");
                    break;
                case "cin":
                    temp = cin();
                    match_nuevo(";");
                    break;
                case "cout":
                    temp = cout();
                    match_nuevo(";");
                    break;
                case "while":
                    temp = while_();
                    break;
                case "{":
                    temp = bloque();
                    break;
                default:
                    if (tokenSintactico.Tipo == "ID")//a++;  a:=2;
                    {
                        temp = asignar();
                        if (lineaError == 0)
                            match_nuevo(";");
                    }
                    //else if(tokenSintactico.lexema != "else" && tokenSintactico.lexema != "end" && tokenSintactico.lexema != "until" && tokenSintactico.lexema != "}")
                    //{
                    //erroresSintacticos += "Error de sintaxis, token inesperado:" + tokenSintactico.lexema + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                    //    lineaError = tokenSintactico.Fila;
                    //nextToken();
                    //error , token no valido en Linea: Col: 'Unexpected'
                    //}
                    else
                    {
                        lineaError = tokenSintactico.Fila;
                    }
                    break;
            }
            return temp;
        }

        public nodo while_()
        {
            nodo temp = new nodo();
            if (tokenSintactico.lexema == "while")
            {
                temp.Nombre = tokenSintactico.lexema;
                //nextToken();
                //while(a+b
                //{
                match("while");
                match("(");
                temp.Hijos.Add(exp());
                match(")");
                if (tokenSintactico.lexema == "{")
                {
                    temp.Hijos.Add(bloque());
                }
                else
                {
                    erroresSintacticos += "Error de sintaxis, token inesperado:" + tokenSintactico.lexema + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                    //error
                }
            }
            return temp;
        }

        public nodo bloque()
        {
            nodo temp = new nodo();
            /*if(tokenSintactico.lexema == "{")
            {
                nextToken();
                temp = secuencia_sent();
                if(tokenSintactico.lexema == "}")
                {
                    nextToken();
                }
                else
                {
                    //error
                    return null;
                }
            }*/
            match("{");
            temp = secuencia_sent();
            match("}");
            return temp;
        }

        public nodo ifPrueba()
        {
            nodo temp = new nodo();
            //Agregando para el semantico
            temp.tipoNodo = "statement";
            temp.Nombre = tokenSintactico.lexema;
            temp.linea = tokenSintactico.Fila;
            match("if");
            temp.Hijos.Add(exp_then());
            match_nuevo("then");
            temp.Hijos.Add(secuencia_sent());
            if (tokenSintactico.lexema == "else")
            {
                nodo else_ = new nodo();
                else_.Nombre = tokenSintactico.lexema;
                else_.linea = tokenSintactico.Fila;
                nextToken();
                //temp.Hijos[2] = secuencia_sent();
                else_.Hijos.Add(secuencia_sent());
                temp.Hijos.Add(else_);
            }
            match_nuevo("end");
            return temp;
        }

        public nodo exp_then()
        {
            var bandera = true;
            nodo temp = new nodo();
            while (bandera == true)
            {
                if (!match_nuevo("("))
                    break;
                temp = exp();
                if (!match_nuevo(")"))
                    break;
                bandera = false;
                //if(a>2 then
                //cout X;
            }
            lineaError = 0;
            return temp;
        }

        public bool match_nuevo(string token)
        {
            if (tokenSintactico == null)
            {
                error(token);
                return false;
            }
            else if (tokenSintactico.lexema.Equals(token))
            {
                getToken();
                return true;
            }
            // token = getchar();//treae el toke de la entrada estandar, en el copilador lo traera de su lexico
            else
            {
                error_prueba(token);
                return false;
            }
            return true;
        }

        public void error_prueba(string c)
        {
            Console.WriteLine("Error de sintaxis {0}", c);
            /*if(tokenSintactico.Columna == 0)
            {
                if(tokenSintactico.Fila > 0)
                {
                    erroresSintacticos += "Error de sintaxis, se esperaba " + c + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                    tokenSintactico.lexema = "ERROR";
                }
            }*/
            erroresSintacticos += "Error de sintaxis, se esperaba " + c + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
            lineaError = tokenSintactico.Fila;
            //tokenSintactico.lexema = "ERROR";
        }

        public nodo if_stmt()
        {
            nodo temp = new nodo();
            temp.Nombre = tokenSintactico.lexema;
            temp.linea = tokenSintactico.Fila;
            match("if");
            match("(");
            temp.Hijos.Add(exp());
            match(")");
            match("then");
            temp.Hijos.Add(secuencia_sent());
            if (tokenSintactico.lexema == "else")
            {
                nodo else_ = new nodo();
                else_.Nombre = tokenSintactico.lexema;
                else_.linea = tokenSintactico.Fila;
                nextToken();
                //temp.Hijos[2] = secuencia_sent();
                else_.Hijos.Add(secuencia_sent());
                temp.Hijos.Add(else_);
            }
            match("end");
            return temp;
            if (tokenSintactico.lexema == "if")
            {
                nextToken();
                //temp.Hijos[0] = exp();
                match("(");
                temp.Hijos.Add(exp());
                match(")");
                if (tokenSintactico.lexema == "then")
                {
                    nextToken();
                    //temp.Hijos[1] = secuencia_sent();
                    temp.Hijos.Add(secuencia_sent());
                    if (tokenSintactico.lexema == "else")
                    {
                        nodo else_ = new nodo();
                        else_.Nombre = tokenSintactico.lexema;
                        nextToken();
                        //temp.Hijos[2] = secuencia_sent();
                        else_.Hijos.Add(secuencia_sent());
                        temp.Hijos.Add(else_);
                    }
                    /*if (tokenSintactico.lexema == "end")
                    {
                        nextToken();
                    }
                    else
                    {
                        //error
                    }*/
                    match("end");
                }
                else
                {

                }
            }
            else
            {

            }
            return temp;
        }

        public nodo repeat()
        {
            nodo temp = new nodo();
            //Agregando para semantico
            temp.tipo = "statement";

            temp.tipoNodo = "statement";

            temp.Nombre = tokenSintactico.lexema;
            temp.linea = tokenSintactico.Fila;
            if (tokenSintactico.lexema == "do")
            {
                nextToken();
                //temp.Hijos[0] = secuencia_sent();
                temp.Hijos.Add(secuencia_sent());
                //nextToken();
                if (tokenSintactico.lexema == "until")
                {
                    nodo until = new nodo();
                    until.Nombre = tokenSintactico.lexema;
                    until.linea = tokenSintactico.Fila;
                    nextToken();
                    //temp.Hijos[1] = exp();
                    until.Hijos.Add(exp());
                    temp.Hijos.Add(until);
                }
                else
                {
                    //error
                }
            }
            else
            {
                //error
            }
            return temp;
        }

        public nodo asignar()
        {
            nodo nuevo = new nodo();
            nodo temp = new nodo();
            if (tokenSintactico.Tipo == "ID")
            {
                temp.Nombre = tokenSintactico.lexema;
                //PROBANDO
                temp.Tipo = "ID";
                //FIN PROBANDO
                temp.linea = tokenSintactico.Fila;
                nextToken();
                if (tokenSintactico.lexema == "++" || tokenSintactico.lexema == "--")
                {
                    switch (tokenSintactico.lexema)
                    {
                        case "++":
                            nuevo = new nodo();
                            nuevo.Nombre = ":=";
                            nuevo.linea = tokenSintactico.Fila;
                            nuevo.Hijos.Add(temp);
                            nodo suma = new nodo();
                            suma.Nombre = "+";
                            nodo derecha = new nodo();
                            derecha.Nombre = "1";
                            derecha.Valor = 1;
                            suma.Hijos.Add(temp);
                            suma.Hijos.Add(derecha);
                            nuevo.Hijos.Add(suma);
                            nextToken();
                            temp = nuevo;
                            break;
                        case "--":
                            nuevo = new nodo();
                            nuevo.Nombre = ":=";
                            nuevo.linea = tokenSintactico.Fila;
                            nuevo.Hijos.Add(temp);
                            suma = new nodo();
                            suma.Nombre = "-";
                            derecha = new nodo();
                            derecha.Nombre = "1";
                            derecha.Valor = 1;
                            suma.Hijos.Add(temp);
                            suma.Hijos.Add(derecha);
                            nuevo.Hijos.Add(suma);
                            nextToken();
                            temp = nuevo;
                            break;
                    }
                }
                else if (tokenSintactico.lexema == ":=")
                {
                    nuevo.Nombre = tokenSintactico.lexema;
                    nuevo.tipoNodo = "statement";
                    nuevo.linea = tokenSintactico.Fila;
                    match_nuevo(":=");
                    nuevo.Hijos.Add(temp);
                    //nuevo.Hijos[0] = exp();
                    nuevo.Hijos.Add(exp());
                }
                else
                {
                    //match_nuevo(":=");
                    lineaError = tokenSintactico.Fila;
                    erroresSintacticos += "Error de sintaxis, token desconocido " + tokenSintactico.lexema + " en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                }
                //nuevo.Nombre = tokenSintactico.lexema;
                //nextToken();
                //nuevo = new nodo();
                //Error
            }
            else
            {
                erroresSintacticos += "Error de sintaxis, se esperaba := en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
                //nextToken();
            }
            return nuevo;
        }

        public nodo cin()
        {
            nodo nuevo = new nodo();
            nuevo.tipoNodo = "statement";
            nuevo.linea = tokenSintactico.Fila;
            while (tokenSintactico.lexema == "cin" && tokenSintactico.lexema != "EOF")
            {
                nuevo.Nombre = tokenSintactico.lexema.ToString();
                nextToken();
                if (tokenSintactico.Tipo == "ID")
                {
                    nodo salida = new nodo();
                    salida.Nombre = tokenSintactico.lexema.ToString();
                    //PROBANDO
                    salida.Tipo = "ID";
                    //FIN PROBANDO
                    salida.linea = tokenSintactico.Fila;
                    nuevo.Atributo = tokenSintactico.lexema.ToString();
                    nuevo.Hijos.Add(salida);
                    nextToken();
                }
            }
            return nuevo;
        }

        public nodo cout()
        {
            nodo nuevo = new nodo();
            nuevo.tipoNodo = "statement";
            nuevo.linea = tokenSintactico.Fila;
            while (tokenSintactico.lexema == "cout" && tokenSintactico.lexema != "EOF")
            {
                nuevo.Nombre = tokenSintactico.lexema.ToString();
                nextToken();
                //nuevo.Hijos[0] = exp();
                nuevo.Hijos.Add(exp());
            }
            return nuevo;
        }

        public nodo exp()
        {
            nodo nuevo;
            nodo temp = new nodo();
            temp = exp_simple();
            while (tokenSintactico.lexema == ">" || tokenSintactico.lexema == "<" || tokenSintactico.lexema == "<=" || tokenSintactico.lexema == ">=" || tokenSintactico.lexema == "!=" || tokenSintactico.lexema == "==" && tokenSintactico.lexema != "EOF")
            {
                nuevo = new nodo();
                nuevo.Nombre = tokenSintactico.lexema.ToString();
                nuevo.tipoNodo = "expresion";
                nuevo.linea = tokenSintactico.Fila;
                nextToken();
                //nuevo.Hijos[0] = temp;
                //nuevo.Hijos[0] = temp;
                nuevo.Hijos.Add(temp);
                nuevo.Valor = temp.Valor;
                //nuevo.Hijos[1] = exp_simple();
                nuevo.Hijos.Add(exp_simple());
                //nuevo.Valor += nuevo.Hijos[1].Valor;
                temp = nuevo;
            }
            return temp;
            //return 1;
        }
        public nodo exp_simple()
        {
            nodo nuevo;
            nodo temp = new nodo();
            temp = term();
            while (tokenSintactico.lexema == "+" || tokenSintactico.lexema == "-" && tokenSintactico.lexema != "EOF")
            {
                switch (tokenSintactico.lexema)
                {
                    case "+":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
                        nuevo.tipoNodo = "expresion";
                        nuevo.linea = tokenSintactico.Fila;
                        nextToken();
                        //nuevo.Hijos[0] = temp;
                        //nuevo.Hijos[0] = temp;
                        nuevo.Hijos.Add(temp);
                        nuevo.Valor = temp.Valor;
                        //nuevo.Hijos[1] = term();
                        nuevo.Hijos.Add(term());
                        //nuevo.Valor += nuevo.Hijos[1].Valor; 
                        nuevo.Valor += nuevo.Hijos.ElementAt(1).Valor;
                        temp = nuevo;

                        break;

                    case "-":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
                        nuevo.tipoNodo = "expresion";
                        nuevo.linea = tokenSintactico.Fila;
                        nextToken();
                        //nuevo.Hijos[0] = temp;
                        nuevo.Hijos.Add(temp);
                        nuevo.Valor = temp.Valor;
                        //nuevo.Hijos[1] = term();
                        nuevo.Hijos.Add(term());
                        //nuevo.Valor -= nuevo.Hijos[1].Valor;
                        nuevo.Valor -= nuevo.Hijos.ElementAt(1).Valor;
                        temp = nuevo;

                        break;
                    default:
                        Console.WriteLine("Nothing");
                        break;
                }
            }
            return temp;
            //return 1;
        }

        public nodo term()
        {
            nodo nuevo = new nodo();
            nodo temp = factor();
            while (tokenSintactico.lexema == "*" || tokenSintactico.lexema == "/" || tokenSintactico.lexema == "%" && tokenSintactico.lexema != "EOF")
            {
                switch (tokenSintactico.lexema)
                {
                    case "*":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
                        nuevo.tipoNodo = "expresion";
                        nuevo.linea = tokenSintactico.Fila;
                        nextToken();
                        //nuevo.Hijos[0] = temp;
                        nuevo.Hijos.Add(temp);
                        nuevo.Valor = temp.Valor;
                        //nuevo.Hijos[1] = factor();
                        nuevo.Hijos.Add(factor());
                        //nuevo.Valor *= nuevo.Hijos[1].Valor;
                        nuevo.Valor *= nuevo.Hijos.ElementAt(1).Valor;
                        temp = nuevo;

                        break;
                    case "/":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
                        nuevo.tipoNodo = "expresion";
                        nuevo.linea = tokenSintactico.Fila;
                        nextToken();
                        //nuevo.Hijos[0] = temp;
                        nuevo.Hijos.Add(temp);
                        nuevo.Valor = temp.Valor;
                        //nuevo.Hijos[1] = factor();
                        nuevo.Hijos.Add(factor());
                        //nuevo.Valor *= nuevo.Hijos[1].Valor;
                        nuevo.Valor /= nuevo.Hijos.ElementAt(1).Valor;
                        temp = nuevo;

                        break;
                    case "%":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
                        nuevo.tipoNodo = "expresion";
                        nuevo.linea = tokenSintactico.Fila;
                        nextToken();
                        //nuevo.Hijos[0] = temp;
                        nuevo.Hijos.Add(temp);
                        nuevo.Valor = temp.Valor;
                        //nuevo.Hijos[1] = factor();
                        nuevo.Hijos.Add(factor());
                        //nuevo.Valor *= nuevo.Hijos[1].Valor;
                        nuevo.Valor %= nuevo.Hijos.ElementAt(1).Valor;
                        temp = nuevo;

                        break;
                    default:
                        Console.WriteLine("Nothing");
                        break;
                }
            }
            return temp;
            //return 1;
        }

        public nodo factor()
        {
            nodo nuevo = new nodo();
            nodo temp = fin();
            while (tokenSintactico.lexema == "^" && tokenSintactico.lexema != "EOF")
            {
                switch (tokenSintactico.lexema)
                {
                    case "^":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
                        nuevo.tipoNodo = "expresion";
                        nuevo.linea = tokenSintactico.Fila;
                        nextToken();
                        //nuevo.Hijos[0] = temp;
                        nuevo.Hijos.Add(temp);
                        nuevo.Valor = temp.Valor;
                        //nuevo.Hijos[1] = fin();
                        nuevo.Hijos.Add(fin());
                        //nuevo.Valor = Math.Pow(nuevo.Valor,nuevo.Hijos[1].Valor);
                        nuevo.Valor = Math.Pow(nuevo.Valor, nuevo.Hijos.ElementAt(1).Valor);
                        temp = nuevo;

                        break;
                }
            }
            return temp;
            //return 1;
        }

        public nodo fin()
        {
            nodo temp = new nodo();
            if (tokenSintactico.lexema == "(")
            {
                //nextToken();
                match("(");
                temp = exp();
                //if (tokenSintactico.lexema == ")")
                //    nextToken();
                match(")");
                //nextToken();
            }
            else if (tokenSintactico.Tipo == "ID")
            {
                temp.Tipo = "ID";
                temp.Nombre = tokenSintactico.lexema.ToString();
                temp.tipoNodo = "expresion";
                temp.linea = tokenSintactico.Fila;
                nextToken();
            }
            else if (tokenSintactico.Tipo == "Entero" || tokenSintactico.Tipo == "Flotante")
            {
                int x = 0;

                //Int32.TryParse(tokenSintactico.lexema, out x);
                temp.Tipo = "factor";
                temp.tipoNodo = "expresion";
                temp.Nombre = tokenSintactico.lexema.ToString();
                temp.Valor = Convert.ToDouble(tokenSintactico.lexema);
                temp.linea = tokenSintactico.Fila;
                //Agregar tipo INT o FLOAT
                if (tokenSintactico.Tipo == "Entero")
                    temp.tipoVariable = "int";
                else
                    temp.tipoVariable = "float";

                nextToken();
            }
            else
            {
                lineaError = tokenSintactico.Fila;
                //erroresSintacticos += "Error de sintaxis, error inesperado en Linea: " + tokenSintactico.Fila + ", " + tokenSintactico.Columna + "\n";
            }
            return temp;
        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged_1(object sender, EventArgs e)
        {
            //Errores Sintacticos
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// Parte Semantica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public nodo raizSemantica;
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            //Limpiamos variables antes de la siguiente ejecucion
            hashTable = new Dictionary<string, HashTableItem>();
            erroresSemanticos = "";
            richTextBox3.Text = null;
            //Console.WriteLine(resultado + "\n");
            ArbolSemantico.Nodes.Clear();
            ArbolSemantico.BeginUpdate();
            //Creacion de la HashTable

            //PROBANDO COSAS
            /*for (int i = 0; i < raizSemantica.hijos.ElementAt(0).hijos.Count(); i++)
                create_hashTable(raizSemantica.hijos.ElementAt(0).hijos.ElementAt(i));*/
            recorrido(raizSemantica);
            
            var t = verArbolSemantico(raizSemantica, ArbolSemantico);
            if (t != null)
                ArbolSemantico.Nodes.Add(t);
            ArbolSemantico.EndUpdate();
            ArbolSemantico.ExpandAll();
            richTextBox3.Text = erroresSemanticos;
            posicionToken = 0;

            printHashTable();
            //Recorrer resto del arbol
        }

        public string erroresSemanticos;

        Dictionary<string, HashTableItem> hashTable = new Dictionary<string, HashTableItem>();

        public int location = 0;

        public double resultadoPruebaOperaciion = 0;

        public void recorrido(nodo arbol)
        {
            switch(arbol.nombre)
            {
                case "declaracion":
                    foreach(var rama in arbol.hijos)
                        create_hashTable(rama);
                    break;
                case ":=":
                    if(lookUp(arbol.hijos.ElementAt(0).nombre))//Ignora asignaciones a variables no declaradas
                    {
                        //Actualizar sus valores
                        arbol.hijos.ElementAt(0).tipoVariable = hashTable[arbol.hijos.ElementAt(0).nombre].tipoVariable;
                        arbol.hijos.ElementAt(0).valor = hashTable[arbol.hijos.ElementAt(0).nombre].valorActual;
                        
                        var resultadoPruebaOperaciion = calculos(arbol.hijos.ElementAt(1));
                        //Checar tipo de ambos hijos
                        string resultado = checkNode(arbol.hijos.ElementAt(0), arbol.hijos.ElementAt(1));
                        string actual = arbol.hijos.ElementAt(0).tipoVariable;
                        if (actual == resultado)
                        {
                            arbol.hijos.ElementAt(0).valor = resultadoPruebaOperaciion;
                            //Agregando el valor obtenido a las asignaciones
                            arbol.valor = resultadoPruebaOperaciion;
                            arbol.tipoVariable = resultado;
                            actualizarHashTable(arbol.hijos.ElementAt(0));
                        }
                        else
                        {
                            //ERROR
                            erroresSemanticos += "El tipo de la variable " + arbol.hijos[0].nombre + 
                                " no corresponde al valor asignado, en linea " + arbol.hijos.ElementAt(0).linea + "\n";
                        }
                    }
                    else
                    {
                        erroresSemanticos += "La variable " + arbol.hijos[0].nombre + " en la linea " + arbol.linea + " no ha sido declarada\n";
                    }
                    break;
                case "cin":
                    insert_node(arbol.hijos.ElementAt(0));
                    break;
                case "cout":
                    arbol.valor = calculos(arbol.hijos.ElementAt(0));
                    break;
                case "if":
                case "until":
                case "while":
                    expressionCheck(arbol.hijos.ElementAt(0));
                    break;
            }
            if(arbol.hijos.Count() > 0)
            {
                foreach(var rama in arbol.hijos)
                {
                    recorrido(rama);
                }
            }

        }

        public void expressionCheck(nodo t)
        {
            nodo b = new nodo();
            nodo a = new nodo();
            a.valor = calculos(t.hijos.ElementAt(0));
            b.valor = calculos(t.hijos.ElementAt(1));
            t.tipoVariable = "booleano";
            switch (t.nombre)
            {
                case "==":
                    if(a.valor == b.valor)
                        t.valor = 1;
                    else
                        t.valor = 0;
                    break;
                case "!=":
                    if (a.valor != b.valor)
                        t.valor = 1;
                    else
                        t.valor = 0;
                    break;
                case "<=":
                    if (a.valor <= b.valor)
                        t.valor = 1;
                    else
                        t.valor = 0;
                    break;
                case ">=":
                    if (a.valor >= b.valor)
                        t.valor = 1;
                    else
                        t.valor = 0;
                    break;
                case ">":
                    if (a.valor > b.valor)
                        t.valor = 1;
                    else
                        t.valor = 0;
                    break;
                case "<":
                    if (a.valor < b.valor)
                        t.valor = 1;
                    else
                        t.valor = 0;
                    break;
            }
        }

        public TreeNode verArbolSemantico(nodo arbol, TreeView tree)
        {
            if (arbol != null)
            {
                TreeNode nuevoT = new TreeNode();

                //if (tree.Nodes.Count == 0)
                //    tree.Nodes.Add(arbol.Nombre.ToString());
                //else
                //    tree.Nodes[0].Nodes.Add(arbol.Nombre.ToString());
                //Probando cosas
                if (arbol.Nombre == null)
                {
                    return null;
                }
                if (arbol.Tipo == "factor" || arbol.Tipo == "ID")
                {
                    string x = arbol.Valor.ToString();
                    //nuevoT.Nodes.Add("Nuevo");
                    if (arbol.Tipo == "ID")
                    {
                        switch(arbol.tipoVariable)
                        {
                            case "int":
                                nuevoT.Text = arbol.Nombre.ToString() + ".Tipo: " + arbol.tipoVariable + " (" + arbol.valor.ToString("N0") + ")";
                                break;
                            case "float":
                                nuevoT.Text = arbol.Nombre.ToString() + ".Tipo: " + arbol.tipoVariable + " (" + arbol.valor.ToString("F") + ")";
                                break;
                            default:
                                nuevoT.Text = arbol.Nombre.ToString() + ".Tipo: SIN TIPO (" + arbol.valor.ToString("N0") + ")";
                                break;
                        }
                        
                        //insert_node(arbol);
                    }
                    else
                    {
                        nuevoT.Text = arbol.Nombre.ToString();
                    }
                    //nuevoT.Name = arbol.Nombre.ToString();
                    //nuevoT.Nodes.Add(x.ToString());
                    return nuevoT;
                }
                //PROBANDO
                if(arbol.nombre == "+" || arbol.nombre == "-" || arbol.nombre == "*" 
                    || arbol.nombre == "/" || arbol.nombre == "%" || arbol.nombre == "^" || arbol.nombre == ":=")
                {
                    switch(arbol.tipoVariable)
                    {
                        case "int":
                            nuevoT.Text = arbol.Nombre.ToString() + " (" + arbol.Valor.ToString("N0") + ")";
                            break;
                        case "float":
                            nuevoT.Text = arbol.Nombre.ToString() + " (" + arbol.Valor.ToString("F") + ")";
                            break;
                        default:
                            nuevoT.Text = arbol.Nombre.ToString() + " (" + arbol.Valor.ToString() + ")";
                            break;
                    }
                    
                }
                else if(arbol.nombre == ">" || arbol.nombre == "<" || arbol.nombre == ">=" 
                    || arbol.nombre == "<=" || arbol.nombre == "==" || arbol.nombre == "!=")
                {
                    if(arbol.valor == 0)
                        nuevoT.Text = arbol.Nombre.ToString() + "Tipo: Boolean (False)";
                    else
                        nuevoT.Text = arbol.Nombre.ToString() + "Tipo: Boolean (True)";
                }
                /*
                else if(arbol.nombre == ":=")
                {
                    resultadoPruebaOperaciion = calculos(arbol.hijos.ElementAt(1));
                    arbol.hijos.ElementAt(0).valor = resultadoPruebaOperaciion;
                    actualizarHashTable(arbol.hijos.ElementAt(0));
                    nuevoT.Text = arbol.nombre;
                }*/
                else
                {
                    nuevoT.Text = arbol.Nombre.ToString();
                }

                //nuevoT.Text = arbol.Nombre.ToString(); //+ ".Valor." + arbol.Valor.ToString();



                //nuevoT.Name = arbol.Nombre.ToString();
                //nuevoT.Nodes.Add(arbol.Nombre.ToString());
                //nuevoT.Nodes[0].Nodes.Add(verArbol(arbol.Hijos[0], tree));
                //nuevoT.Nodes[0].Nodes.Add(verArbol(arbol.Hijos[1], tree));
                var hijos = arbol.Hijos.Count;
                for (int i = 0; i < hijos; i++)
                {
                    if (arbol.Hijos[i] != null && arbol.Hijos[i].nombre != null)
                        nuevoT.Nodes.Add(verArbolSemantico(arbol.Hijos[i], tree));
                }
                //if(arbol.Hijos[0] != null)
                //    nuevoT.Nodes.Add(verArbol(arbol.Hijos[0], tree));
                //if (arbol.Hijos[1] != null)
                //    nuevoT.Nodes.Add(verArbol(arbol.Hijos[1], tree));
                //treeView1.Nodes[0].Nodes;

                //Console.WriteLine("Nodo -> " + arbol.Nombre + "\n");
                //Console.WriteLine("Valor -> " + arbol.Valor + "\n");

                //tree = verArbol(arbol.Hijos[0], tree);
                //tree = verArbol(arbol.Hijos[1], tree);

                return nuevoT;
            }
            return null;
        }

        public class HashTableItem
        {
            public string nombre { get; set; }
            public List<int> lineas { get; set; }
            public int localidad { get; set; }
            public string tipoVariable { get; set; }
            public double valorActual { get; set; }
        }

        public void create_hashTable(nodo t)
        {
            for (int i = 0; i < t.hijos.Count(); i++)
            {
                if (!lookUp(t.hijos.ElementAt(i).nombre))
                {
                    hashTable.Add(t.hijos.ElementAt(i).nombre, new HashTableItem
                    {
                        nombre = t.hijos.ElementAt(i).nombre,
                        lineas = new List<int>(),
                        localidad = location++,
                        tipoVariable = t.hijos.ElementAt(i).tipoNodo,
                        valorActual = 0
                    });
                    hashTable[t.hijos.ElementAt(i).nombre].lineas.Add(t.hijos.ElementAt(i).linea);
                    //Propagacion de tipo en variables
                    t.hijos.ElementAt(i).tipoNodo = t.tipoNodo;
                }
                else
                {
                    //Error, se declaro la misma variable dos veces
                    erroresSemanticos += "Variable " + t.hijos.ElementAt(i).nombre + " duplicada en la linea " + t.hijos.ElementAt(i).linea + "\n";
                }
            }
        }

        public bool lookUp(string nombre)
        {
            if(hashTable.ContainsKey(nombre))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void traverse(nodo t, string recorrido)
        {
            if (t != null)
            {
                try
                {
                    if (recorrido == "pre")
                        //preProc(t);
                    for (int i = 0; i < t.hijos.Count(); i++)
                    {
                        traverse(t.hijos.ElementAt(i),"pre");
                    }
                    //if (recorrido == "post")
                        //postProc(t);

                }
                catch (Exception e) { }
            }
        }

        public void insert_node(nodo t)
        {
            if(!lookUp(t.nombre))
            {
                //Error no esta declarada la variable
                erroresSemanticos += "La variable " + t.nombre + " en la linea " + t.linea + " no ha sido declarada\n";
            }
            else
            {
                //Si se encuentra, agrega la linea donde la encontro
                hashTable[t.nombre].lineas.Add(t.linea);
                //Propagacion de tipo
                t.tipoVariable = hashTable[t.nombre].tipoVariable;
                t.Valor = hashTable[t.nombre].valorActual;
            }
            /*
            if(t != null)
            {
                switch(t.tipoNodo)
                {
                    case "statement" :
                        switch(t.nombre)
                        {
                            case ":=":
                            case "cin":
                                if(lookUp(t.nombre))
                                {
                                    //Se actualza la linea en la que aparecio
                                }
                                else
                                {
                                    //Error, encontramos una variable que no esta en la hashTable
                                }
                                break;
                        }
                        break;
                }
            }*/
        }

        public void printHashTable()
        {
            //Agregando el GRID VIEW
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Nombre", "Nombre");
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns.Add("Location", "Location");
            dataGridView1.Columns[1].Width = 60;
            
            dataGridView1.Columns.Add("Ultimo Valor", "Ultimo Valor");
            dataGridView1.Columns[2].Width = 60;
            dataGridView1.Columns.Add("Numero de ocurrencias", "Numero de ocurrencias");
            dataGridView1.Columns[3].Width = 60;
            dataGridView1.Columns.Add("Lineas", "Lineas");
            dataGridView1.Columns[4].Width = 200;

            Console.WriteLine("Nombre\tLocation\tUltimo Valor\tNumero de Veces\tLineas\n");
            foreach(var item in hashTable)
            {
                string lineas = "";
                Console.Write(item.Value.nombre + "\t" + item.Value.localidad + "\t");
                Console.Write("\t " + item.Value.valorActual);
                Console.Write("\t " + "(" + item.Value.lineas.Count() + ")\t");
                foreach(var i in item.Value.lineas)
                {
                    Console.Write(i + " ");
                    lineas += i + ",";
                }
                Console.Write("\n");

                dataGridView1.Rows.Add(item.Value.nombre, item.Value.localidad.ToString(), item.Value.valorActual.ToString()
                    , item.Value.lineas.Count().ToString(), lineas);
            }

            
        }

        public string checkNode(nodo a, nodo b)
        {
            if(a.tipoVariable == b.tipoVariable)
            {
                return a.tipoVariable;
            }
            else
            {
                return "float";
            }
        }

        public double calculos(nodo raiz)
        {
            if(raiz.hijos.Count > 0)
            {
                switch(raiz.nombre)
                {
                    case "+":
                        raiz.valor = calculos(raiz.hijos.ElementAt(0)) + calculos(raiz.hijos.ElementAt(1));
                        //Propagacion de tipo en  operaciones
                        raiz.tipoVariable = checkNode(raiz.hijos.ElementAt(0), raiz.hijos.ElementAt(1));
                        if(raiz.tipoVariable == "int")
                            raiz.Valor = Math.Truncate(raiz.Valor);
                        break;
                    case "-":
                        raiz.valor = calculos(raiz.hijos.ElementAt(0)) - calculos(raiz.hijos.ElementAt(1));
                        raiz.tipoVariable = checkNode(raiz.hijos.ElementAt(0), raiz.hijos.ElementAt(1));
                        if (raiz.tipoVariable == "int")
                            raiz.Valor = Math.Truncate(raiz.Valor);
                        break;
                    case "*":
                        raiz.valor = calculos(raiz.hijos.ElementAt(0)) * calculos(raiz.hijos.ElementAt(1));
                        raiz.tipoVariable = checkNode(raiz.hijos.ElementAt(0), raiz.hijos.ElementAt(1));
                        if (raiz.tipoVariable == "int")
                            raiz.Valor = Math.Truncate(raiz.Valor);
                        break;
                    case "/":
                        raiz.valor = calculos(raiz.hijos.ElementAt(0)) / calculos(raiz.hijos.ElementAt(1));
                        raiz.tipoVariable = checkNode(raiz.hijos.ElementAt(0), raiz.hijos.ElementAt(1));
                        if (raiz.tipoVariable == "int")
                            raiz.Valor = Math.Truncate(raiz.Valor);
                        break;
                    case "%":
                        raiz.valor = calculos(raiz.hijos.ElementAt(0)) % calculos(raiz.hijos.ElementAt(1));
                        raiz.tipoVariable = checkNode(raiz.hijos.ElementAt(0), raiz.hijos.ElementAt(1));
                        if (raiz.tipoVariable == "int")
                            raiz.Valor = Math.Truncate(raiz.Valor);
                        break;
                    case "^":
                        raiz.valor = Math.Pow(calculos(raiz.hijos.ElementAt(0)),calculos(raiz.hijos.ElementAt(1)));
                        raiz.tipoVariable = checkNode(raiz.hijos.ElementAt(0), raiz.hijos.ElementAt(1));
                        if (raiz.tipoVariable == "int")
                            raiz.Valor = Math.Truncate(raiz.Valor);
                        break;
                }
                return raiz.valor;
            }
            else
            {
                if(raiz.Tipo == "ID")
                {
                    if (hashTable.ContainsKey(raiz.nombre))
                    {
                        insert_node(raiz);
                        //Propagacion de tipo y valor actual
                        raiz.tipoVariable = hashTable[raiz.nombre].tipoVariable;
                        raiz.valor = hashTable[raiz.nombre].valorActual;
                        return hashTable[raiz.nombre].valorActual;
                    }
                    else
                        return 0; //Se uso variable que no se encuentra en la hashtable
                }
                else
                {
                    return raiz.valor;
                }
            }
        }

        public void actualizarHashTable(nodo t)
        {
            if(hashTable.ContainsKey(t.nombre))
            {
                hashTable[t.nombre].valorActual = t.valor;
                hashTable[t.nombre].lineas.Add(t.linea);

            }
            else
            {
                erroresSemanticos += "La variable " + t.nombre + " en la linea " + t.linea + " no ha sido declarada\n";
            }
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// CODIGO INTERMEDIO
        /// </summary>
        public string codigoIntermedioTexto = "";
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            codigoIntermedioTexto = "";
            if(!string.IsNullOrEmpty(erroresSemanticos) || !string.IsNullOrEmpty(erroresSintacticos))
            {
                MessageBox.Show("No puede compilar hasta arreglar los errores.");
            }
            else
            {
                emitLoc = 0;
                highEmitLoc = 0;
                codigoIntermedio(raizSemantica.hijos[1]);
            }
            richTextBox6.Text = codigoIntermedioTexto;
            guardarIntermedio(codigoIntermedioTexto);
        }
        //DEFINE
        public int mp = 6;
        public int ac = 0;
        public int ac1 = 1;
        public int gp = 5;
        public static int tmpOffset = 0;
        public void codigoIntermedio(nodo tree)
        {
            emitRM("LD", mp, 0, ac, "Load max address");
            emitRM("ST", ac, 0, ac, "Clear location 0");
            foreach (var hijo in tree.hijos)
            {
                cGen(hijo);
            }
            emitRO("HALT", 0, 0, 0, "");
        }

        public int st_lookUp(string nombre)
        {
            if (hashTable.ContainsKey(nombre))
            {
                return hashTable[nombre].localidad;
            }
            else
            {
                return -1;
            }
        }

        public void cGen(nodo tree)
        {
            if(tree != null)
            {
                switch(tree.tipoNodo)
                {
                    case "expresion":
                        genExp(tree);
                        break;
                    case "statement":
                        genStmt(tree);
                        break;
                }
            }
        }

        public void genStmt(nodo tree)
        {
            int savedLoc1, savedLoc2, currentLoc;
            int loc;
            if(tree != null)
            {
                /*if(tree.hijos.Count > 0)
                {*/
                    switch (tree.nombre)
                    {
                        case ":=":
                            cGen(tree.hijos[1]);
                            loc = st_lookUp(tree.hijos[0].nombre);
                            emitRM("ST", ac, loc, gp, "Asigna valor guardado");
                            break;
                        case "cin":
                            emitRO("IN", ac, 0, 0, "Read integer value");
                            loc = st_lookUp(tree.hijos[0].nombre);
                            emitRM("ST", ac, loc, gp, "read: store value");
                            break;
                        case "cout":
                            cGen(tree.hijos[0]);
                            emitRO("OUT", ac, 0, 0, "Write ac");
                            break;
                        case "if":
                            cGen(tree.hijos[0]);
                            savedLoc1 = emitSkip(1);

                            foreach(var hijo in tree.hijos[1].hijos)
                            {
                                cGen(hijo);
                            }
                            //cGen(tree.hijos[1].hijos[0]);

                            savedLoc2 = emitSkip(1);
                            currentLoc = emitSkip(0);
                            emitBackup(savedLoc1);
                            emitRM_Abs("JEQ", ac, currentLoc, "if: jmp to else");
                            emitRestore();
                            if(tree.hijos.Count > 2)
                            {
                                //cGen(tree.hijos[2].hijos[0].hijos[0]);

                                foreach(var hijo in tree.hijos[2].hijos[0].hijos)
                                {
                                    cGen(hijo);
                                }

                                currentLoc = emitSkip(0);
                                emitBackup(savedLoc2);
                                emitRM_Abs("LDA", pc, currentLoc, "jmp to end");
                                emitRestore();
                            }
                            break;
                        case "do":
                            savedLoc1 = emitSkip(0);
                            foreach(var hijo in tree.hijos[0].hijos[0].hijos)
                            {
                                cGen(hijo);
                            }
                            //cGen(tree.hijos[0].hijos[0].hijos[0]);
                            cGen(tree.hijos[1].hijos[0]);
                            emitRM_Abs("JEQ", ac, savedLoc1, "repeat: jmp back to body");
                            break;
                        default:
                            break;
                    }
                //}
            }
        }

        public void genExp(nodo tree)
        {
            int loc;
            switch(tree.tipo)
            {
                case "ID":
                    loc = st_lookUp(tree.nombre);
                    emitRM("LD", ac, loc, gp, "Load id value");
                    break;
                case "factor":
                    emitRM("LDC", ac, (int)tree.Valor, 0, "load const");
                    break;
                default:
                    if(tree.hijos.Count == 2)
                    {
                        cGen(tree.hijos[0]);
                        emitRM("ST", ac, tmpOffset--, mp, "Op: push left");
                        cGen(tree.hijos[1]);
                        emitRM("LD", ac1, ++tmpOffset, mp, "Op: load left");
                        switch(tree.nombre)
                        {
                            case "+":
                                emitRO("ADD", ac, ac1, ac, "op +");
                                break;
                            case "-":
                                emitRO("SUB", ac, ac1, ac, "op -");
                                break;
                            case "*":
                                emitRO("MUL", ac, ac1, ac, "op *");
                                break;
                            case "/":
                                emitRO("DIV", ac, ac1, ac, "op /");
                                break;
                            case "^":
                                for (int i = 0; i < tree.hijos[1].valor;i++)
                                {
                                    cGen(tree.hijos[0]);
                                    emitRM("ST", ac, tmpOffset--, mp, "Op: push left");
                                    cGen(tree.hijos[1]);
                                    emitRM("LD", ac1, ++tmpOffset, mp, "Op: load left");
                                    emitRO("MUL", ac, ac1, ac, "op *");
                                }
                                break;
                            case "<":
                                emitRO("SUB", ac, ac1, ac, "op <");
                                emitRM("JLT", ac, 2, pc, "br if true");
                                emitRM("LDC", ac, 0, ac, "false case");
                                emitRM("LDA", pc, 1, pc, "unconditional jmp");
                                emitRM("LDC", ac, 1, ac, "true case");
                                break;
                            case "==":
                                emitRO("SUB", ac, ac1, ac, "op ==");
                                emitRM("JEQ", ac, 2, pc, "br if true");
                                emitRM("LDC", ac, 0, ac, "false case");
                                emitRM("LDA", pc, 1, pc, "unconditional jmp");
                                emitRM("LDC", ac, 1, ac, "true case");
                                break;
                            case "<=":
                                emitRO("SUB", ac, ac1, ac, "op ==");
                                emitRM("JLE", ac, 2, pc, "br if true");
                                emitRM("LDC", ac, 0, ac, "false case");
                                emitRM("LDA", pc, 1, pc, "unconditional jmp");
                                emitRM("LDC", ac, 1, ac, "true case");
                                break;
                            case ">=":
                                emitRO("SUB", ac, ac1, ac, "op ==");
                                emitRM("JGE", ac, 2, pc, "br if true");
                                emitRM("LDC", ac, 0, ac, "false case");
                                emitRM("LDA", pc, 1, pc, "unconditional jmp");
                                emitRM("LDC", ac, 1, ac, "true case");
                                break;
                            case ">":
                                emitRO("SUB", ac, ac1, ac, "op ==");
                                emitRM("JGT", ac, 2, pc, "br if true");
                                emitRM("LDC", ac, 0, ac, "false case");
                                emitRM("LDA", pc, 1, pc, "unconditional jmp");
                                emitRM("LDC", ac, 1, ac, "true case");
                                break;
                            case "!=":
                                emitRO("SUB", ac, ac1, ac, "op ==");
                                emitRM("JNE", ac, 2, pc, "br if true");
                                emitRM("LDC", ac, 0, ac, "false case");
                                emitRM("LDA", pc, 1, pc, "unconditional jmp");
                                emitRM("LDC", ac, 1, ac, "true case");
                                break;
                        }
                    }
                    break;
            }
        }

        void guardarIntermedio(string contenidoTerminado)
        {
            string fecha = DateTime.Now.ToString();
            fecha = fecha.Replace("/", "-");
            fecha = fecha.Replace(" ", "");
            fecha = fecha.Replace(".", "");
            fecha = fecha.Replace(":", "-");
            using (var saveFile = new System.IO.StreamWriter("CodigoIntermedio-" + fecha + ".tm"))
            {
                saveFile.WriteLine(contenidoTerminado);
                //MessageBox.Show("Despues de escribir el contenido");
            }
            using (var saveFile = new System.IO.StreamWriter("CodigoIntermedio.tm"))
            {
                saveFile.WriteLine(contenidoTerminado);
                //MessageBox.Show("Despues de escribir el contenido");
            }
            //MessageBox.Show("Terminar Leer Archivos");
        }

        /// <summary>
        /// Codigo de CODE
        /// </summary>
        public int emitLoc = 0;
        public int highEmitLoc = 0;
        public int pc = 7;
        public void emitRO(string op, int r, int s, int t, string comentario)
        {
            codigoIntermedioTexto += emitLoc++.ToString()+":  " + op.ToString() + "  " + r.ToString() + "," + s.ToString() + "," + t.ToString() + "\n";
            if (highEmitLoc < emitLoc)
                highEmitLoc = emitLoc;
        }

        public void emitRM(string op, int r, int d, int s, string comentario)
        {
            codigoIntermedioTexto += emitLoc++.ToString() + ":  " + op.ToString() + "  " + r.ToString() + "," + d.ToString() + "(" + s.ToString() + ")" + "\n";
            if (highEmitLoc < emitLoc)
                highEmitLoc = emitLoc;
        }

        public int emitSkip(int howMany)
        {
            int i = emitLoc;
            emitLoc += howMany;
            if (highEmitLoc < emitLoc)
                highEmitLoc = emitLoc;
            return i;
        }

        public void emitBackup(int loc)
        {
            //if(loc > highEmitLoc)
            emitLoc = loc;
        }

        public void emitRestore(/*void*/)
        {
            emitLoc = highEmitLoc;
        }

        public void emitRM_Abs(string op, int r, int a, string comentario)
        {
            codigoIntermedioTexto += emitLoc.ToString() + ":  " + op + "  " + r.ToString() + "," + (a - (emitLoc + 1)).ToString() + "(" + pc.ToString() + ")" + "\n";
            ++emitLoc;
            if (highEmitLoc < emitLoc)
                highEmitLoc = emitLoc;
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}