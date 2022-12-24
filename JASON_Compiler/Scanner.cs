using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
public enum Token_Class
{
    Else, ElseIF, End, If, Integer, Float, String, Repeat, Read, Then, Until, Write,
    Semicolon, Comma, LParanthesis, RParanthesis, LCurlyB, RCurlyB, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, AndOP, OrOP, AssignOp,
    Idenifier, Constant, Return, EndL, Main, StringValue
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.ElseIF);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.EndL);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurlyB);
            Operators.Add("}", Token_Class.RCurlyB);
            Operators.Add(":=", Token_Class.AssignOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOP);
            Operators.Add("||", Token_Class.OrOP);



        }

        public void StartScanning(string SourceCode)
        {

            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();


                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                //if (CurrentChar == '-') Console.WriteLine(CurrentLexeme);

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //Handling identifier and keywords
                {

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {

                        CurrentChar = SourceCode[j];
                        if ((CurrentChar >= 'A' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9'))
                        {
                            CurrentLexeme += CurrentChar;
                            i = j;
                        }
                        else
                        {
                            i = j - 1;
                            break;
                        }

                    }


                    //Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '"')  // string case 
                {

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '\r' || CurrentChar == '\n')
                            break;
                        if (CurrentChar == '"')
                        {
                            CurrentLexeme += CurrentChar;
                            i = j;
                            break;
                        }
                        i = j;
                        CurrentLexeme += CurrentChar;



                    }
                    //checking if the string is not correct 
                    if (CurrentLexeme[CurrentLexeme.Length - 1] != '"')
                        Errors.Error_List.Add(CurrentLexeme);
                    else
                    {
                        // Console.WriteLine(CurrentLexeme);
                        FindTokenClass(CurrentLexeme);
                    }


                }

                else if ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '.')  // numbers case;
                {

                    int dotcounter = 0;
                    //bool thereISLetter = false;
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];


                        if ((CurrentChar >= '0' && CurrentChar <= '9') || (CurrentChar == '.') || (CurrentChar >= 'A' && CurrentChar <= 'z'))
                        {
                            if (CurrentChar == '.') dotcounter++;
                            //if((CurrentChar >= 'A' && CurrentChar <= 'z')) thereISLetter = true;
                            CurrentLexeme += CurrentChar;
                            i = j;

                        }

                        else
                        {
                            i = j - 1;
                            break;
                        }

                    }
                    /*
                    if (dotcounter > 1 )
                    {
                        //more thane dot 774.4.3.5 invalid put it in the errorlist
                        // Console.WriteLine("oh dude what are you doint ");
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    else
                    {
                        //Console.WriteLine(CurrentLexeme);
                        FindTokenClass(CurrentLexeme);
                    }*/
                    FindTokenClass(CurrentLexeme);
                }

                else if (CurrentChar == '/' && i < SourceCode.Length - 1 && SourceCode[i + 1] == '*')  // comment case
                {
                    CurrentLexeme += '*';
                    for (j = i + 2; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '*' && j < SourceCode.Length - 1 && SourceCode[j + 1] == '/')
                        {
                            CurrentLexeme += '*';
                            CurrentLexeme += '/';
                            i = j + 1;
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                    }
                    if (CurrentLexeme.Length >= 2 && CurrentLexeme[CurrentLexeme.Length - 1] == '/' && CurrentLexeme[CurrentLexeme.Length - 2] == '*')
                    {
                        //Console.WriteLine(CurrentLexeme);

                        // FindTokenClass(CurrentLexeme);
                    }
                    else
                    {
                        i++;
                        //Console.WriteLine("dudeeee again");
                        Errors.Error_List.Add("/*");
                    }
                }
                else if (CurrentChar == '<' && i < SourceCode.Length - 1 && SourceCode[i + 1] == '>')
                {
                    CurrentLexeme += '>';
                    i++;
                    // Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }
                else if ((CurrentChar == '<' || CurrentChar == '>') && i < SourceCode.Length - 1 && SourceCode[i + 1] == '=')
                {
                    CurrentLexeme += '=';
                    i++;
                    // Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '=' || CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '/' || CurrentChar == '*' || CurrentChar == ';' || CurrentChar == ',' || CurrentChar == '>' || CurrentChar == '<')
                {
                    //Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '}' || CurrentChar == '{' || CurrentChar == ')' || CurrentChar == '(')
                {
                    //Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);

                }
                else if (CurrentChar == '&' && i < SourceCode.Length - 1 && SourceCode[i + 1] == '&')
                {
                    CurrentLexeme += '&';
                    i++;
                    // Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }

                else if (CurrentChar == '|' && i < SourceCode.Length - 1 && SourceCode[i + 1] == '|')
                {
                    CurrentLexeme += '|';
                    i++;
                    //Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }


                else if (CurrentChar == ':' && i < SourceCode.Length - 1 && SourceCode[i + 1] == '=')
                {
                    CurrentLexeme += '=';
                    i++;
                    //Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }

                else
                {  //anything with one charachters will be sent ot the findtoken
                    Errors.Error_List.Add(CurrentLexeme);
                }


            }
            JASON_Compiler.TokenStream = Tokens;




            void FindTokenClass(string Lex)
            {
                Token_Class TC;
                Token Tok = new Token();
                Tok.lex = Lex;

                //Is it a reserved word?
                if (ReservedWords.ContainsKey(Lex))
                {
                    Tok.token_type = ReservedWords[Lex];
                    Tokens.Add(Tok);

                }

                //Is it an identifier?
                else if (isIdentifier(Lex))
                {
                    Tok.token_type = Token_Class.Idenifier;
                    Tokens.Add(Tok);

                }
                //Is it a Constant?
                else if (isConstant(Lex))
                {
                    Tok.token_type = Token_Class.Constant;
                    Tokens.Add(Tok);

                }
                //Is it an string?
                else if (isString(Lex))
                {
                    Tok.token_type = Token_Class.StringValue;
                    Tokens.Add(Tok);
                }

                // Is it an operator
                else if (Operators.ContainsKey(Lex))
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);


                }

                //Is it an undefined?
                else
                {
                    Errors.Error_List.Add(Lex);
                }
            }



            bool isIdentifier(string lex)
            {
                bool isValid = true;
                // Check if the lex is an identifier or not.
                var identifierRegx = new Regex(@"^[a-zA-Z]([a-zA-Z\d])*$", RegexOptions.Compiled);
                if (!identifierRegx.IsMatch(lex))
                    isValid = false;

                return isValid;
            }
            bool isConstant(string lex)
            {
                bool isValid = true;
                // Check if the lex is a constant (Number) or not.
                var constantRegx = new Regex(@"^(\+|-)?(\d)+(\.(\d)+)?$", RegexOptions.Compiled);
                if (!constantRegx.IsMatch(lex))
                    isValid = false;


                return isValid;
            }
            bool isString(string lex)
            {
                bool isValid = true;
                // Check if the lex is a constant (Number) or not.
                var stringRegex = new Regex(@"^""[0-9]*[a-zA-Z]*[^""]*""$", RegexOptions.Compiled);
                if (!stringRegex.IsMatch(lex))
                    isValid = false;
                return isValid;
            }
        }

    }
}