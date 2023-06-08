using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace Compilador
{
    public partial class Default : System.Web.UI.Page
    {
        private string textoArchivo;
        protected Literal litErroresSintacticos;

        private List<Token> tokens;
        private List<TokenErroresSintacticos> errorSintactico;
        private List<TokenErroresLexicos> errorLexico;
        private List<Codigo> codigos;
        public int index = 0;
        private bool bandera = true;

        protected void AdjuntarBtn_Click(object sender, EventArgs e)
        {
            if (archivoUpload.HasFile)
            {
                // Leer el contenido del archivo y guardarlo en una variable
                using (StreamReader sr = new StreamReader(archivoUpload.PostedFile.InputStream))
                {
                    textoArchivo = sr.ReadToEnd();
                }
            }

            if (!string.IsNullOrEmpty(textoArchivo))
            {
                // Crear una lista para almacenar los tokens
                tokens = new List<Token>();
                errorSintactico = new List<TokenErroresSintacticos>();
                errorLexico = new List<TokenErroresLexicos>();

                codigos = new List<Codigo>();

                // Expresión regular para el análisis léxico
                Regex lexerRegex = new Regex(@"\b[a-zA-Z_]\w*\b|\b\d+\b(?![a-zA-Z])|==|!=|>=|<=|[+\-*/%<>&|^\?~(){}]|(?<!=)=(?!=)|\b(?:abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|using static|virtual|void|volatile|while|where)\b");

                // Encontrar todas las coincidencias en el texto
                MatchCollection matches = lexerRegex.Matches(textoArchivo);

                // Procesar los tokens en el orden correcto
                foreach (Match match in matches)
                {
                    string valor = match.Value;// Aqui se guarda el valor
                    int posicion = match.Index; // Aqui se guarda su posicion

                    // Identificar el tipo de token en base a las subexpresiones regulares
                    if (Regex.IsMatch(valor, @"\b[a-zA-Z_]\w*\b"))
                    {
                        string identificadorMinusculas = valor.ToLower();
                        if (!Regex.IsMatch(identificadorMinusculas, @"\b(?:abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|using static|virtual|void|volatile|while|where)\b"))
                        {
                            if (valor == "var")  // Agregar esta condición para el token ";"
                            {
                                tokens.Add(new Token("Variable", valor, posicion));
                            }

                            else
                            {
                                if (!Regex.IsMatch(valor, @"\b[a-zA-Z_]\w*\b"))
                                {
                                    errorLexico.Add(new TokenErroresLexicos("Empieza con numero", valor, posicion));

                                }
                                else
                                {
                                    tokens.Add(new Token("Identificador", valor, posicion));

                                }
                            }
                            
                        }
                        else
                        {
                            if (valor.ToUpper() == valor)
                            {
                                // errorSintactico.Add(new TokenErroresSintacticos("Error", "Palabra reservada incorrecta"));
                                errorLexico.Add(new TokenErroresLexicos("Palabra reservada escrita incorrectamente", valor, posicion));

                            }
                            tokens.Add(new Token("Palabra Reservada", valor, posicion));
                        }
                    }
                    else if (Regex.IsMatch(valor, @"\b\d+\b(?![a-zA-Z])"))
                    {
                        tokens.Add(new Token("Numero", valor, posicion));
                    }

                    else if (Regex.IsMatch(valor, @"[+\-*/%<>=!&|^\?~(){}]"))
                    {
                        switch (valor)
                        {
                            case "+":
                                tokens.Add(new Token("OperadorSuma", valor, posicion));
                                break;
                            case "-":
                                tokens.Add(new Token("OperadorResta", valor, posicion));
                                break;
                            case "*":
                                tokens.Add(new Token("OperadorMultiplicacion", valor, posicion));
                                break;
                            case "/":
                                tokens.Add(new Token("OperadorDivision", valor, posicion));
                                break;
                            case "=":
                                tokens.Add(new Token("OperadorIgualdad", valor, posicion));
                                break;
                            case "(":
                                tokens.Add(new Token("Apertura", valor, posicion));
                                break;
                            case ")":
                                tokens.Add(new Token("Cierre", valor, posicion));
                                break;
                            case "<":
                                tokens.Add(new Token("OperadorMenorQue", valor, posicion));
                                break;
                            case ">":
                                tokens.Add(new Token("OperadorMayorQue", valor, posicion));
                                break;
                            case ">=":
                                tokens.Add(new Token("OperadorMayoroIgual", valor, posicion));
                                break;

                            case "==":
                                tokens.Add(new Token("OperadorComparacion", valor, posicion));
                                break;
                            case "{":
                                tokens.Add(new Token("LlaveApertura", valor, posicion));
                                break;
                            case "}":
                                tokens.Add(new Token("LlaveCierre", valor, posicion));
                                break;
                        }
                    }
                    else
                    {
                        errorLexico.Add(new TokenErroresLexicos("Error Token No reconocido", valor, posicion));
                    }
                }



                // Ordenar los tokens por posición
                tokens = tokens.OrderBy(t => t.Posicion).ToList();

                // Construir la cadena HTML con los resultados de los tokens
                string htmlTokens = string.Empty;
                foreach (Token token in tokens)
                {
                    htmlTokens += "<tr>";
                    htmlTokens += $"<td>{token.Tipo}</td>";
                    htmlTokens += $"<td>{token.Valor}</td>";
                    htmlTokens += $"<td>{token.Posicion}</td>";
                    htmlTokens += "</tr>";
                }

                litTokens.Text = htmlTokens;

                // Construir la cadena HTML con los errorSintactico léxicos

                // Construir la cadena HTML con los errorSintactico sintacticos
                string hmtlErroresLexicos = string.Empty;
                foreach (TokenErroresLexicos errorLex in errorLexico)
                {
                    hmtlErroresLexicos += "<tr>";
                    hmtlErroresLexicos += $"<td>{errorLex.Tipo}</td>";
                    hmtlErroresLexicos += $"<td>{errorLex.Valor}</td>";
                    hmtlErroresLexicos += "</tr>";
                }

                litErroresLexicos.Text = hmtlErroresLexicos;


                Programa();

                // Construir la cadena HTML con los errorSintactico sintacticos
                string htmlErrorSintactico = string.Empty;
                foreach (TokenErroresSintacticos error in errorSintactico)
                {
                    htmlErrorSintactico += "<tr>";
                    htmlErrorSintactico += $"<td>{error.Tipo}</td>";
                    htmlErrorSintactico += $"<td>{error.Valor}</td>";
                    htmlErrorSintactico += "</tr>";
                }

                litErroresSintacticos.Text = htmlErrorSintactico;

                if (bandera)
                {
                    //seccion para la generacion de codigo en c++, si no hay errrores muestra la parte final
                    codigos.Add(new Codigo("return 0;"));

                    codigos.Add(new Codigo("}"));

                    string htmlCodigo = string.Empty;
                    foreach (Codigo codigo in codigos)
                    {

                        htmlCodigo += "<tr>";
                        htmlCodigo += $"<td>{codigo.CodigoI}</td>";
                        htmlCodigo += "</tr>";
                    }

                    GeneracionCodigo.Text = htmlCodigo;
                }
                
            }
        }


        public void Programa()
        {
            if (tokens[index].Valor != "begin")
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la palabra reservada 'begin'"));
                return;
            }
            codigos.Add(new Codigo("#include " + "&lt;iostream&gt;"));
            codigos.Add(new Codigo("using namespace std;"));
            codigos.Add(new Codigo("int main() {"));

            index++;

            while (index < tokens.Count)
            {

                if (tokens[index].Tipo == "Palabra Reservada")
                {
                    break;
                }

                if (tokens[index].Tipo == "Variable")
                {

                    bool hayErrores = Declaraciones();

                    if (hayErrores)
                    {
                        bandera = false;
                        return;
                    }


                }
                else
                {
                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba una variable que despues del begin declares las variables, no se detecto el 'var'"));
                    return;
                }

                if ((index) + 1 == tokens.Count)
                {
                    break;
                }
            }
            //segundo while para las sentencias
            while (index < tokens.Count)
            {
                if (tokens[index].Valor == "end")
                {
                    break;
                }
                //se quito la validacion de si es palabra reservada

                if (tokens[index].Tipo == "Palabra Reservada")
                {

                    bool hayErrores = Sentencias();

                    if (hayErrores)
                    {
                        bandera = false;
                        return;
                    }


                }
                else
                {
                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaban sentencias,¿"));
                    return;
                }

                if ((index) + 1 == tokens.Count)
                {
                    break;
                }
            }
            // index--;
            if (tokens[index].Valor != "end")
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la palabra reservada 'end'"));
                return;
            }

            //index++;
        }

        public bool Sentencias()
        {
            bool hayErrores = false;
            errorSintactico.Add(new TokenErroresSintacticos("Error", "Entraste a Sentencias'"));



            if (tokens[index].Valor == "if" || tokens[index].Valor == "while")
            {
                index++;
                if (tokens[index].Valor == "(")
                {
                    index++;
                    bool errorComparacion = Comparacion();

                    if (errorComparacion)
                    {
                        hayErrores = true;
                        return hayErrores;
                    }
                }
                else
                {
                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la apertura para condicional"));
                    hayErrores = true;
                    return hayErrores;
                }

            }
            else if (tokens[index].Valor == "do")
            {
                index++;
                bool errorComparacion = ValidarDo();

                if (errorComparacion)
                {
                    hayErrores = true;
                    return hayErrores;
                }

            }
            else
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba una estructura de control valida"));
                hayErrores = true;
                return hayErrores;
            }

            


            return hayErrores;


        }

        public bool ValidarDo()
        {
            bool hayErrores = false;

            if (tokens[index].Valor == "{")
            {
                index++;
                if (tokens[index].Valor == "printf")
                {
                    index++;
                    if (tokens[index].Tipo == "Identificador" || tokens[index].Tipo == "Numero")
                    {
                        index++;
                        if (tokens[index].Valor == "}")
                        {
                            index++;
                            if (tokens[index].Valor == "while")
                            {
                                index++;
                                if (tokens[index].Valor == "(")
                                {
                                    index++;
                                    if (tokens[index].Tipo == "Identificador")
                                    {
                                        index++;
                                        if (tokens[index].Tipo == "OperadorComparacion" ||
                                                 tokens[index].Tipo == "OperadorMayoroIgual" ||
                                                 tokens[index].Tipo == "OperadorMayorQue" ||
                                                 tokens[index].Tipo == "OperadorMenorQue")
                                        {
                                            index++;

                                            if (tokens[index].Tipo == "Identificador" || tokens[index].Tipo == "Numero")
                                            {
                                                index++;
                                                if (tokens[index].Valor  == ")")
                                                {
                                                    index++;
                                                    codigos.Add(new Codigo(tokens[index - 11].Valor + " " + tokens[index - 10].Valor));
                                                    codigos.Add(new Codigo("cout << " + tokens[index - 8].Valor + ";"));

                                                    codigos.Add(new Codigo(tokens[index - 7].Valor + " " + tokens[index - 6].Valor + " " + tokens[index - 5].Valor + " " + tokens[index - 4].Valor + " " + tokens[index - 3].Valor + " " + tokens[index - 2].Valor + " " + tokens[index - 1].Valor+ ";"));
                                                }
                                                else
                                                {
                                                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un el parentesis de cierre"));
                                                    hayErrores = true;
                                                    return hayErrores;
                                                }
                                            }
                                            else
                                            {
                                                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un identificador o numero valido"));
                                                hayErrores = true;
                                                return hayErrores;
                                            }

                                        }
                                        else
                                        {
                                            errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un operando valido"));
                                            hayErrores = true;
                                            return hayErrores;
                                        }

                                    }
                                    else
                                    {
                                        errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un identificador"));
                                        hayErrores = true;
                                        return hayErrores;
                                    }


                                }
                                else
                                {
                                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba el parentesis de apertura en el DO while"));
                                    hayErrores = true;
                                    return hayErrores;
                                }
                            }
                            else
                            {
                                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la la palabra reservada while"));
                                hayErrores = true;
                                return hayErrores;
                            }

                        }
                        else
                        {
                            errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la llave de cierre"));
                            hayErrores = true;
                            return hayErrores;
                        }
                    }                  
                    else
                    {
                        errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un identificador o un numero"));
                        hayErrores = true;
                        return hayErrores;
                    }
                }
                else
                {
                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la palabra Printf"));
                    hayErrores = true;
                    return hayErrores;
                }

            }
            else
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la apertura en el condicional Do {"));
                hayErrores = true;
                return hayErrores;
            }

            return hayErrores;
        }

        public bool Comparacion()
        {
            bool hayErrores = false;

            errorSintactico.Add(new TokenErroresSintacticos("Error", "Entraste a Comparacion'"));


            if (tokens[index].Tipo == "Identificador")
            {
                index++;
                if (tokens[index].Tipo == "OperadorComparacion" ||
                    tokens[index].Tipo == "OperadorMayoroIgual" ||
                    tokens[index].Tipo == "OperadorMayorQue" ||
                    tokens[index].Tipo == "OperadorMenorQue")
                {
                    index++;
                    //string Operando = operador();
                    if (tokens[index].Tipo == "Identificador" || tokens[index].Tipo == "Numero")
                    {
                        index++;
                        if (tokens[index].Valor == ")")
                        {
                            index++;
                            if (tokens[index].Valor == "{")
                            {
                                index++;
                                if (tokens[index].Valor == "printf")
                                {
                                    index++;
                                    if (tokens[index].Tipo == "Identificador" || tokens[index].Tipo == "Numero")
                                    {
                                        index++;
                                        if (tokens[index].Valor == "}")
                                        {
                                            codigos.Add(new Codigo(tokens[index - 9].Valor + " " + tokens[index - 8].Valor + " " + tokens[index - 7].Valor + " " + tokens[index - 6].Valor + " " + tokens[index - 5].Valor + " " + tokens[index - 4].Valor + " " + tokens[index - 3].Valor  ));
                                            codigos.Add(new Codigo( "cout << " + tokens[index - 1].Valor+ ";"));
                                            codigos.Add(new Codigo(tokens[index - 0].Valor));
                                            index++;
                                        }
                                        else
                                        {
                                            errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba llave Cierre"));
                                            hayErrores = true;
                                            return hayErrores;
                                        }
                                    }
                                    else
                                    {
                                        errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un identificador o dato valido"));
                                        hayErrores = true;
                                        return hayErrores;
                                    }
                                }
                                else
                                {
                                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba 'printf'"));
                                    hayErrores = true;
                                    return hayErrores;
                                }

                            }
                            else
                            {
                                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba llave apertura"));
                                hayErrores = true;
                                return hayErrores;
                            }
                        }
                        else
                        {
                            errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba el operando de cierre."));
                            hayErrores = true;
                            return hayErrores;
                        }
                    }
                    else
                    {
                        errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba el dato despues operador relacional."));
                        hayErrores = true;
                        return hayErrores;
                    }
                }
                else
                {
                    errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba Un operador relacional"));
                    hayErrores = true;
                    return hayErrores;
                }
            }
            else
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba la apertura para condicional"));
                hayErrores = true;
                return hayErrores;
            }

            return hayErrores;
        }


        String operador()
        {
            String operandorelegido = "";

            switch (operandorelegido)
            {
                case "<":
                    operandorelegido = "<";
                    break;
                case ">":
                    operandorelegido = ">";
                    break;
                case "=":
                    operandorelegido = "==";
                    break;
                case "<=":
                    operandorelegido = "<=";
                    break;
                case ">=":
                    operandorelegido = ">=";
                    break;

            }
            return operandorelegido;
        }
        public bool Declaraciones()
        {
            bool hayErrores = false;
            errorSintactico.Add(new TokenErroresSintacticos("Error", "Entraste a declaraciones'"));

            

            if (tokens[index].Valor == "var")
            {
                index++;

                Identificador();

            }
            else
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Palabra reservada incorrecta 'var'"));
                hayErrores = true;
                return hayErrores;
            }



            if (tokens[index].Valor == "=")
            {
                index++;
            }
            else
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba el operador de asignación '='"));
                hayErrores = true;
                return hayErrores;
            }



            if (tokens[index].Tipo == "Numero" || tokens[index].Tipo == "Identificador")
            {
                codigos.Add(new Codigo( "int" + " " + tokens[index - 2].Valor  +  " "+ tokens[index - 1].Valor + " " + tokens[index].Valor+ ";"));
                index++;
                

            }
            else
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un número o identificador después del operador de asignación"));
                hayErrores = true;
                return hayErrores;
            }



            return hayErrores;
        }

        public void Identificador()
        {
            if (tokens[index].Tipo != "Identificador")
            {
                errorSintactico.Add(new TokenErroresSintacticos("Error", "Se esperaba un identificador"));
                index++;
                return;
            }

            index++;
        }







        public class Token
        {
            public string Tipo { get; set; }
            public string Valor { get; set; }
            public int Posicion { get; set; }

            public Token(string tipo, string valor, int posicion)
            {
                Tipo = tipo;
                Valor = valor;
                Posicion = posicion;
            }
        }

        public class TokenErroresSintacticos
        {
            public string Tipo { get; set; }
            public string Valor { get; set; }

            public TokenErroresSintacticos(string tipo, string valor)
            {
                Tipo = tipo;
                Valor = valor;
            }
        }

        public class TokenErroresLexicos
        {
            public string Tipo { get; set; }
            public string Valor { get; set; }
            public int Posicion { get; set; }

            public TokenErroresLexicos(string tipo, string valor, int posicion)
            {
                Tipo = tipo;
                Valor = valor;
                Posicion = posicion;
            }
        }

        public class Codigo
        {
            public string CodigoI { get; set; }


            public Codigo(string codigos)
            {
                CodigoI = codigos;

            }



            public class Nodo
            {
                public string Tipo { get; set; }

                public List<Nodo> Hijos { get; set; }
            }
        }
    }
}
