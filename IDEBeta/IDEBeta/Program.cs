using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace IDEBeta
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

    public class token
    {
        public string tipo { get; set; }
        public string lexema { get; set; }
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
    }

    static class Program
    {
        
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
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*array de tokens*/
            List<token> tokens = new List<token>();
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
            /*Diccionario caracteres especiales*/
            //+ - * /  % < <= > >= == != := ( ) { } // /**/ ++ -- 
            Dictionary<string, caracteresEspeciales> especiales = new Dictionary<string, caracteresEspeciales>();
            especiales.Add("(", new caracteresEspeciales("("));
            especiales.Add(")", new caracteresEspeciales(")"));
            especiales.Add("{", new caracteresEspeciales("{"));
            especiales.Add("}", new caracteresEspeciales("}"));
            especiales.Add("[", new caracteresEspeciales("["));
            especiales.Add("]", new caracteresEspeciales("]"));
            /*Diccionario de operadores*/
            Dictionary<string, Operadores> operadores = new Dictionary<string, Operadores>();
            operadores.Add("*", new Operadores("*"));
            operadores.Add("%", new Operadores("%"));
            /*Estados*/
            /*0 = inicial
             1 = Entero
             2 = Punto
             3 = Flotante
             10 = finalizo*/
            Estado estado = Estado.Inicio;
            Token tokenActual = Token.Entero;
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
            string[] argumentos = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (argumentos.Count() > 1)
            {
                leerArchivo(argumentos[1]);
                return;
            }
            else
            {
                Application.Run(new Form1());
            }
            //Application.Run(new Form1());
            void leerArchivo(string nombreArchivo)
            {
                List<token> tokensLexicos = new List<token>();
                List<token> tokensErrorLexicos = new List<token>();
                //MessageBox.Show("Entre a leer Archivo");                
                System.IO.StreamReader file = new System.IO.StreamReader(nombreArchivo);
                contenido = file.ReadToEnd();
                file.Close();
                /*Analisis Lexico*/
                char ch = 'a';
                var contenidoChar = contenido.ToCharArray();
                string resultado = "";
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
                                else if (ch.ToString() == " " || (int)ch == 13 || ch.ToString() == "\t")
                                {
                                    guardar = false;
                                    estado = Estado.Inicio;
                                }
                                else if(especiales.ContainsKey(ch.ToString()))
                                {
                                    estado = Estado.Final;
                                    tokenActual = Token.CaracterSimple;
                                }
                                else if(ch.ToString() == "/")
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
                                else if(ch.ToString() == ":")
                                {
                                    estado = Estado.Asignacion;
                                    tokenActual = Token.Asignacion;
                                }
                                else if(ch.ToString() == "=")
                                {
                                    estado = Estado.Comparacion;
                                    tokenActual = Token.Error;
                                    //MessageBox.Show("Entre al igual");
                                }
                                else if(ch.ToString() == "<")
                                {
                                    estado = Estado.Menor;
                                    tokenActual = Token.Menor;
                                }
                                else if(ch.ToString() == ">")
                                {
                                    estado = Estado.Mayor;
                                    tokenActual = Token.Mayor;
                                }
                                else if(ch.ToString() == "!")
                                {
                                    estado = Estado.Diferente;
                                    tokenActual = Token.Diferente;
                                }
                                /*else if(ch.ToString() == "#")//control fin del texto
                                {
                                    guardar = false;
                                    estado = Estado.Final;
                                }*/
                                else if(operadores.ContainsKey(ch.ToString()))
                                {
                                    estado = Estado.Final;
                                    tokenActual = Token.Operador;
                                }
                                else
                                {
                                    //MessageBox.Show("Entre al ELSE");
                                    guardar = false;
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
                                    estado = Estado.Final;
                                    tokenActual = Token.Error;
                                    break;
                                }
                                estado = Estado.Flotante;
                                tokenActual = Token.Flotante;
                                break;
                            case Estado.Flotante:
                                //MessageBox.Show("Char Actual " + ch.ToString());
                                if(!Char.IsDigit(ch))
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
                                if(!Char.IsLetterOrDigit(ch) && (int)ch != 95)
                                {
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
                                if(ch.ToString() == "/")
                                {
                                    estado = Estado.ComentarioLinea;
                                    //lexemaActual.Remove(lexemaActual.Length - 1, 1);
                                    lexemaActual = "";
                                    guardar = false;
                                    break;
                                }
                                if(ch.ToString() == "*")
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
                                posicion--;
                                columna--;
                                break;
                            case Estado.ComentarioLinea:
                                //MessageBox.Show("Entre en comentario de linea");
                                if(ch.ToString() == "\n")
                                {
                                    linea++;
                                    estado = Estado.Inicio;
                                    guardar = false;
                                    break;
                                }
                                //MessageBox.Show("LexemaActual " + lexemaActual.ToString());
                                guardar = false;
                                break;
                            case Estado.ComentarioMultiple:
                                if(ch.ToString() == "*")
                                {
                                    estado = Estado.FinComentarioMultiple;
                                    guardar = false;
                                    break;
                                }
                                if(ch.ToString() == "\n")
                                {
                                    linea++;
                                }
                                guardar = false;
                                break;
                            case Estado.FinComentarioMultiple:
                                if(ch.ToString() == "/")
                                {
                                    estado = Estado.Inicio;
                                    guardar = false;
                                    break;
                                }
                                if(ch.ToString() != "*")
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
                                if(ch.ToString() == "=")
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
                                if(ch.ToString() == "=")
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
                                if(ch.ToString() == "=")
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
                                if(ch.ToString() == "=")
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
                                valorEsperado();
                                tokenActual = Token.Error;
                                estado = Estado.Final;
                                break;
                            default:
                                //MessageBox.Show("Entre en default");
                                estado = Estado.Final;
                                break;
                        }
                        if (guardar && (int)ch!=3)
                        {
                            //MessageBox.Show("Guardamos " + ch.ToString());
                            lexemaActual += ch;
                        }
                    }
                    //MessageBox.Show("Operador: "+tokenActual.ToString());
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
                        resultado += tokenActual.ToString();
                        resultado += "->";
                        resultado += lexemaActual;
                        if (tokenActual == Token.Error)
                        {
                            tokensErrorLexicos.Add(new token()
                            {
                                Tipo = tokenActual.ToString(),
                                Lexema = lexemaActual.ToString() + errorActual + " en linea " + linea + " columna " + columna
                            });
                            resultado += " en linea " + linea + " columna " + columna;
                            //AGREGAR A TOKENS DE ERRORES, CON MENSAJE
                            if (!string.IsNullOrEmpty(errorActual))
                            {
                                resultado += errorActual;
                                errorActual = "";
                            }
                        }
                        resultado += '\n';
                        //MessageBox.Show(resultado);
                        /*Guardamos el token en el array de tokens*/
                        //tokens.Add(new token(tokenActual.ToString(),lexemaActual.ToString()));
                        //lexemaActual = "";
                        tokensLexicos.Add(new token()
                        {
                            Tipo = tokenActual.ToString(),
                            Lexema = lexemaActual.ToString()
                        });
                        lexemaActual = "";
                    }
                    //MessageBox.Show("Posicion despues de Final " + posicion);
                    //MessageBox.Show("Posicion " + posicion);
                    //resultado += "\n";
                    /*Checar palabras reservadas*/
                }
                //MessageBox.Show("Justo despues de salir");
                //MessageBox.Show(estado.ToString());
                if(estado != Estado.Final && !string.IsNullOrEmpty(lexemaActual))
                {
                    if (reservadas.ContainsKey(lexemaActual))
                    {
                        tokenActual = Token.PalabraReservada;
                    }
                    //MessageBox.Show("Entre en IF final");
                    //MessageBox.Show("tokenActual " + tokenActual.ToString());
                    //MessageBox.Show("LexemaActual" + lexemaActual.ToString());
                    resultado += tokenActual.ToString();
                    resultado += "->";
                    resultado += lexemaActual;
                    if (tokenActual == Token.Error)
                    {
                        //tokensErrorLexicos.Add(new token(tokenActual.ToString(), lexemaActual.ToString() + errorActual));
                        tokensErrorLexicos.Add(new token()
                        {
                            Tipo = tokenActual.ToString(),
                            Lexema = lexemaActual.ToString() + errorActual + " en linea " + linea + " columna " + columna
                        });
                        resultado += " en linea " + linea + " columna " + columna;
                        //AGREGAR A TOKENS DE ERRORES, CON MENSAJE
                        if (!string.IsNullOrEmpty(errorActual))
                        {
                            resultado += errorActual;
                            errorActual = "";
                        }
                    }
                    resultado += '\n';
                    tokensLexicos.Add(new token()
                    {
                        Tipo = tokenActual.ToString(),
                        Lexema = lexemaActual.ToString()
                    });
                    lexemaActual = "";
                    //MessageBox.Show(resultado);
                }
                if (!string.IsNullOrEmpty(lexemaActual))
                {
                    resultado += tokenActual.ToString();
                    resultado += "->";
                    resultado += lexemaActual;
                    if (tokenActual == Token.Error)
                    {
                        //tokensErrorLexicos.Add(new token(tokenActual.ToString(), lexemaActual.ToString() + errorActual));
                        tokensErrorLexicos.Add(new token()
                        {
                            Tipo = tokenActual.ToString(),
                            Lexema = lexemaActual.ToString() + errorActual + " en linea " + linea + " columna " + columna
                        });
                        resultado += " en linea " + linea + " columna " + columna;
                        if (!string.IsNullOrEmpty(errorActual))
                        {
                            resultado += errorActual;
                            errorActual = "";
                        }

                        //AGREGAR A TOKENS DE ERRORES, CON MENSAJE
                    }
                    resultado += '\n';
                    //MessageBox.Show(resultado);
                    tokensLexicos.Add(new token()
                    {
                        Tipo = tokenActual.ToString(),
                        Lexema = lexemaActual.ToString()
                    });
                    lexemaActual = "";
                }
                //MessageBox.Show("Sali del while");
                guardarResultado(resultado);
                Console.WriteLine(resultado);
            }
            char siguienteCaracter(int posicionActual)
            {
                if(posicionActual == contenido.Length)
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
                //MessageBox.Show("Terminar Leer Archivos");
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
        }
    }
}