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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)//Lexico
        {
            if(cambiosGuardados == true)
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
                            else if(ch.ToString() == "\n")
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
                            if(ch.ToString() == "\n")
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
            if(tokensErrorLexicos.Count > 0)
            {
                string lexicos = "";
                foreach(var error in tokensErrorLexicos)
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
            switch(estado)
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
            if(string.IsNullOrEmpty(nombreArchivo))
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
                    
                    if (richTextBox1.Lines.Count()> this.lineas - 1)
                    {
                        while(richTextBox1.Lines.Count() > this.lineas - 1)
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
                                if(this.lineas == 0)
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

            string[] wordsWithP = { "if(","else{","until(","while(","then(","main(" };

            string[] comentarios = { "//", "/*" };
            // Get the current caret position.
            currentLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
            //currentCol = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(currentLine);
            int posicionActual = richTextBox1.SelectionStart;// - richTextBox1.GetFirstCharIndexFromLine(currentLine);

            Dictionary<int, comentarios> diccionarioCierres = new Dictionary<int,comentarios> ();
            Dictionary<int, int> diccionarioAperturas = new Dictionary<int, int>();

            if (!string.IsNullOrEmpty(richTextBox1.Text.ToString()))
            {
                richTextBox4.Focus();
                var richTextBoxAux = new RichTextBox();
                richTextBoxAux= richTextBox1;
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
                            if(match.Index > 0)
                            {
                                if(match.Index + find.Length < textoBuscador.Length)
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
                                    if(busca >= 4)
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
                        foreach(var pos in diccionarioCierres)
                        {
                            if (match.Index > pos.Value.apertura && match.Index < pos.Value.cierre)
                            {
                                banderaSimple = false;
                            }
                        }
                        if(banderaSimple == true)
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
                if(posicionActual >= 0)
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
            public double atributoDoule;
            //public nodo[] hijos = new nodo[3];
            public List<nodo> hijos = new List<nodo>();
            public nodo izquierdo;
            public nodo derecho;

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
        public void nextToken()
        {
            //match
            if(posicionToken < tokensLexicos.Count)
            {
                tokenSintactico = tokensLexicos[posicionToken];
                posicionToken++;
            }
            else
            {
                tokenSintactico.lexema = "EOF";
            }
        }

        public TreeNode verArbol(nodo arbol,TreeView tree)
        {
            if(arbol != null)
            {
                TreeNode nuevoT = new TreeNode();
                
                //if (tree.Nodes.Count == 0)
                //    tree.Nodes.Add(arbol.Nombre.ToString());
                //else
                //    tree.Nodes[0].Nodes.Add(arbol.Nombre.ToString());
                //Probando cosas
                if(arbol.Nombre == null)
                {
                    return null;
                }
                if(arbol.Tipo == "factor" || arbol.Tipo == "ID")
                {
                    string x = arbol.Valor.ToString();
                    //nuevoT.Nodes.Add("Nuevo");
                    nuevoT.Text = arbol.Nombre.ToString();
                    //nuevoT.Name = arbol.Nombre.ToString();
                    //nuevoT.Nodes.Add(x.ToString());
                    return nuevoT;
                }
                nuevoT.Text = arbol.Nombre.ToString();
                //nuevoT.Name = arbol.Nombre.ToString();
                //nuevoT.Nodes.Add(arbol.Nombre.ToString());
                //nuevoT.Nodes[0].Nodes.Add(verArbol(arbol.Hijos[0], tree));
                //nuevoT.Nodes[0].Nodes.Add(verArbol(arbol.Hijos[1], tree));
                var hijos = arbol.Hijos.Count;
                for(int i = 0; i< hijos; i++)
                {
                    if (arbol.Hijos[i] != null && arbol.Hijos[i].nombre != null)
                        nuevoT.Nodes.Add(verArbol(arbol.Hijos[i], tree));
                }
                //if(arbol.Hijos[0] != null)
                //    nuevoT.Nodes.Add(verArbol(arbol.Hijos[0], tree));
                //if (arbol.Hijos[1] != null)
                //    nuevoT.Nodes.Add(verArbol(arbol.Hijos[1], tree));
                //treeView1.Nodes[0].Nodes;
                Console.WriteLine("Nodo -> " + arbol.Nombre + "\n");
                Console.WriteLine("Valor -> " + arbol.Valor + "\n");
                //tree = verArbol(arbol.Hijos[0], tree);
                //tree = verArbol(arbol.Hijos[1], tree);
                
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
            //Console.WriteLine(resultado + "\n");
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            var t = verArbol(raiz,treeView1);
            treeView1.Nodes.Add(t);
            treeView1.EndUpdate();
            posicionToken = 0;
        }

        public nodo programa()
        {
            nodo t = new nodo();
            t.Nombre = "main";
            t.Hijos.Add(lista_declaracion());
            //t.Hijos[1] = secuencia_sent();
            t.Hijos.Add(secuencia_sent());
            return t;
        }

        public nodo lista_declaracion()
        {
            nodo nuevo = new nodo();
            nuevo.Nombre = "declaracion";
            while(tokenSintactico.lexema == "int" || tokenSintactico.lexema == "float" || tokenSintactico.lexema == "boolean")
            {
                nodo otro = new nodo();
                otro.Nombre = tokenSintactico.lexema;
                otro.Tipo = "variable";
                nextToken();
                nodo primero = new nodo();
                primero.Nombre = tokenSintactico.lexema;
                primero.Tipo = "ID";
                otro.Hijos.Add(primero);
                nextToken();
                while (tokenSintactico.lexema != ";" && tokenSintactico.lexema == ",")
                {
                    nextToken();
                    nodo variable = new nodo();
                    variable.Nombre = tokenSintactico.lexema;
                    variable.Tipo = "ID";
                    otro.Hijos.Add(variable);
                    nextToken();
                }
                nextToken();
                nuevo.Hijos.Add(otro);
            }
            return nuevo;
        }

        public nodo secuencia_sent()
        {
            nodo temp = new nodo();
            temp.Nombre = "sentencias";
            nodo t = secuencia();
            nodo p = t;
            while(tokenSintactico.lexema != "end" && tokenSintactico.lexema != "else" && tokenSintactico.lexema != "until" && tokenSintactico.lexema != "EOF")
            {
                nodo q = new nodo();
                if(tokenSintactico.lexema == ";")
                {
                    nextToken();
                    if(t!= null)
                        temp.Hijos.Add(t);
                    q = secuencia();
                    temp.Hijos.Add(q);
                    t = null;
                }
            }
            return temp;
        }

        public nodo secuencia()
        {
            nodo temp = new nodo();
            switch(tokenSintactico.lexema)
            {
                case "if":
                    temp = if_stmt();
                    break;
                case "do":
                    temp = repeat();
                    break;
                case "cin":
                    temp = cin();
                    break;
                case "cout":
                    temp = cout();
                    break;
                default:
                    if(tokenSintactico.Tipo == "ID")
                    {
                        temp = asignar();
                    }
                    else
                    {
                        //error
                    }
                    break;
            }
            return temp;
        }

        public nodo if_stmt()
        {
            nodo temp = new nodo();
            temp.Nombre = tokenSintactico.lexema;
            if (tokenSintactico.lexema == "if")
            {
                nextToken();
                //temp.Hijos[0] = exp();
                temp.Hijos.Add(exp());
                if(tokenSintactico.lexema == "then")
                {
                    nextToken();
                    //temp.Hijos[1] = secuencia_sent();
                    temp.Hijos.Add(secuencia_sent());
                    if(tokenSintactico.lexema == "else")
                    {
                        nodo else_ = new nodo();
                        else_.Nombre = tokenSintactico.lexema;
                        nextToken();
                        //temp.Hijos[2] = secuencia_sent();
                        else_.Hijos.Add(secuencia_sent());
                        temp.Hijos.Add(else_);
                    }
                    if (tokenSintactico.lexema == "end")
                    {
                        nextToken();
                    }
                    else
                    {
                        //error
                    }
                }
            }
            return temp;
        }

        public nodo repeat()
        {
            nodo temp = new nodo();
            temp.Nombre = tokenSintactico.lexema;
            if (tokenSintactico.lexema == "do")
            {
                nextToken();
                //temp.Hijos[0] = secuencia_sent();
                temp.Hijos.Add(secuencia_sent());
                //nextToken();
                if(tokenSintactico.lexema == "until")
                {
                    nodo until = new nodo();
                    until.Nombre = tokenSintactico.lexema;
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
            if(tokenSintactico.Tipo == "ID")
            {
                nuevo.Nombre = tokenSintactico.lexema;
                nextToken();
                if(tokenSintactico.lexema == ":=")
                {
                    nextToken();
                    //nuevo.Hijos[0] = exp();
                    nuevo.Hijos.Add(exp());
                }
                else
                {
                    //Error
                }
            }
            return nuevo;
        }

        public nodo cin()
        {
            nodo nuevo = new nodo();
            while(tokenSintactico.lexema == "cin")
            {
                nuevo.Nombre = tokenSintactico.lexema.ToString();
                nextToken();
                if(tokenSintactico.Tipo == "ID")
                    nuevo.Atributo = tokenSintactico.lexema.ToString();
                nextToken();
            }
            return nuevo;
        }

        public nodo cout()
        {
            nodo nuevo = new nodo();
            while (tokenSintactico.lexema == "cout")
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
            while (tokenSintactico.lexema == ">" || tokenSintactico.lexema == "<" || tokenSintactico.lexema == "<=" || tokenSintactico.lexema == "=>" || tokenSintactico.lexema == "!=" || tokenSintactico.lexema == "==")
            {
                nuevo = new nodo();
                nuevo.Nombre = tokenSintactico.lexema.ToString();
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
            while(tokenSintactico.lexema == "+" || tokenSintactico.lexema == "-" || tokenSintactico.lexema == "++" || tokenSintactico.lexema == "--")
            {
                switch (tokenSintactico.lexema)
                {
                    case "+":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
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
                    case "++":

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
            while(tokenSintactico.lexema == "*" || tokenSintactico.lexema == "/" || tokenSintactico.lexema == "%")
            {
                switch (tokenSintactico.lexema)
                {
                    case "*":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
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
            while(tokenSintactico.lexema == "^")
            {
                switch(tokenSintactico.lexema)
                {
                    case"^":
                        nuevo = new nodo();
                        nuevo.Nombre = tokenSintactico.lexema.ToString();
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
            if(tokenSintactico.lexema == "(")
            {
                nextToken();
                temp = exp();
                if (tokenSintactico.lexema == ")")
                    nextToken();
                //nextToken();
            }
            else if(tokenSintactico.Tipo == "ID")
            {
                temp.Tipo = "ID";
                temp.Nombre = tokenSintactico.lexema.ToString();
                nextToken();
            }
            else
            {
                int x = 0;

                //Int32.TryParse(tokenSintactico.lexema, out x);
                temp.Tipo = "factor";
                temp.Nombre = tokenSintactico.lexema.ToString();
                temp.Valor = Convert.ToDouble(tokenSintactico.lexema);
                nextToken();
            }
            return temp;
        }

    }
}
