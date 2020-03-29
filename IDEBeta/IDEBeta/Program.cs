using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    static class Program
    {
        public class token
        {
            string tipo;
            string lexema;
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
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
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
            string[] argumentos = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //leerArchivo("Prueba.txt");
            if (argumentos.Count() > 1)
            {
                //MessageBox.Show("Entro a modo consola.");
                MessageBox.Show(argumentos.Count().ToString());
                MessageBox.Show(argumentos[1].ToString());
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
                //MessageBox.Show("Entre a leer Archivo");                
                System.IO.StreamReader file = new System.IO.StreamReader(nombreArchivo);
                contenido = file.ReadToEnd();
                file.Close();
                //MessageBox.Show("Depues de leer el archivo");
                MessageBox.Show("Size" + contenido.Length.ToString());
                /*char ch = 'a';
                for(int i = 0;i<contenido.Length; i++)
                {
                    ch = siguienteCaracter(posicion++);
                    MessageBox.Show(((int)ch).ToString());
                }
                return;*/
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
                                else if(ch.ToString() == "\n" || ch.ToString() == "" || (int)ch == 13)
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
                                    MessageBox.Show("Entre al igual");
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
                                    estado = Estado.FinLectura;
                                    tokenActual = Token.Operador;
                                }
                                else
                                {
                                    MessageBox.Show("Entre al ELSE");
                                    guardar = false;
                                    estado = Estado.Final;
                                    lexemaActual = "";
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
                                    guardar = false;
                                    estado = Estado.Final;
                                    tokenActual = Token.Entero;
                                }
                                break;
                            case Estado.Punto:
                                if (!Char.IsDigit(ch))
                                {
                                    //guardar = false;
                                    posicion--;
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
                                    guardar = false;
                                    break;
                                }
                                estado = Estado.Final;
                                tokenActual = Token.Operador;
                                posicion--;
                                break;
                            case Estado.ComentarioLinea:
                                //MessageBox.Show("Entre en comentario de linea");
                                if(ch.ToString() == "\n")
                                {
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
                                tokenActual = Token.Operador;
                                break;
                            case Estado.Mas:
                                estado = Estado.Final;
                                if (ch.ToString() == "+")
                                {
                                    tokenActual = Token.Incremento;
                                    break;
                                }
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
                                estado = Estado.Final;
                                tokenActual = Token.Error;
                                break;
                            case Estado.Comparacion:
                                if(ch.ToString() == "=")
                                {
                                    MessageBox.Show("Entre a la igualacion");
                                    tokenActual = Token.Comparacion;
                                    estado = Estado.Final;
                                    break;
                                }
                                posicion--;
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
                    MessageBox.Show("Operador: "+tokenActual.ToString());
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
                        resultado += '\n';
                        MessageBox.Show(resultado);
                        lexemaActual = "";
                    }
                    //MessageBox.Show("Posicion despues de Final " + posicion);
                    //MessageBox.Show("Posicion " + posicion);
                    //resultado += "\n";
                    /*Checar palabras reservadas*/
                }
                MessageBox.Show("Justo despues de salir");
                MessageBox.Show(estado.ToString());
                if(estado != Estado.Final && !string.IsNullOrEmpty(lexemaActual))
                {
                    if (reservadas.ContainsKey(lexemaActual))
                    {
                        tokenActual = Token.PalabraReservada;
                    }
                    MessageBox.Show("Entre en IF final");
                    MessageBox.Show("tokenActual " + tokenActual.ToString());
                    MessageBox.Show("LexemaActual" + lexemaActual.ToString());
                    resultado += tokenActual.ToString();
                    resultado += "->";
                    resultado += lexemaActual;
                    resultado += '\n';
                    MessageBox.Show(resultado);
                }
                if (!string.IsNullOrEmpty(lexemaActual))
                {
                    resultado += tokenActual.ToString();
                    resultado += "->";
                    resultado += lexemaActual;
                    resultado += '\n';
                    MessageBox.Show(resultado);
                    lexemaActual = "";
                }
                MessageBox.Show("Sali del while");
                guardarResultado(resultado);
            }
            char siguienteCaracter(int posicionActual)
            {
                if(posicionActual == contenido.Length)
                {
                    MessageBox.Show("Entre a el size");
                    //MessageBox.Show("tokenActual " + tokenActual.ToString());
                    //fuera = tokenActual;
                    return (char)3;
                }
                var ch = contenido[posicionActual];
                if (ch == null)
                {
                    MessageBox.Show("Entre a el else de ch");
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
                    MessageBox.Show("Despues de escribir el contenido");
                }
                MessageBox.Show("Terminar Leer Archivos");
            }
        }
    }
}
