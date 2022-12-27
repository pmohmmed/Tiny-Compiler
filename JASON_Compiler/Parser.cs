using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }

        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Prog());
            program.Children.Add(Main_Function()); /*mostafa function*/
            MessageBox.Show("Success");
            return program;
        }
        Node Prog()
        {
            Node prog = new Node("Prog");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.Integer)
                {
                    prog.Children.Add(Function_Statment()); /*mostafa function*/
                    prog.Children.Add(Prog());
                    return prog;
                }
                else
                {
                    return null;
                }

            }
            return prog;
        }
        Node Function_Statment()
        {
            Node fstate_node = new Node("Function_Statment");

            fstate_node.Children.Add(Functions_Declaration());
            fstate_node.Children.Add(Functions_Body());

            return fstate_node;
        }

        Node Functions_Declaration()
        {
            Node fdeclar_node = new Node("Function_Declaration");

            fdeclar_node.Children.Add(Data_Type());
            fdeclar_node.Children.Add(match(Token_Class.Idenifier));
            fdeclar_node.Children.Add(match(Token_Class.LParanthesis));
            fdeclar_node.Children.Add(Parameters());
            fdeclar_node.Children.Add(match(Token_Class.RParanthesis));


            return fdeclar_node;


        }

        Node Functions_Body()
        {
            Node fbody_node = new Node("Function_Body");

            fbody_node.Children.Add(match(Token_Class.LCurlyB));
            fbody_node.Children.Add(Multi_Stats());
            fbody_node.Children.Add(ReturnStatement()); //weloo


            fbody_node.Children.Add(match(Token_Class.RCurlyB));


            return fbody_node;


        }
        Node Main_Function()
        {
            Node mfuncion_node = new Node("Main_Function");

            mfuncion_node.Children.Add(Data_Type());
            mfuncion_node.Children.Add(match(Token_Class.Main));
            mfuncion_node.Children.Add(match(Token_Class.LParanthesis));
            mfuncion_node.Children.Add(match(Token_Class.RParanthesis));
            mfuncion_node.Children.Add(Functions_Body());


            return mfuncion_node;
        }
        Node Function_Call()
        {
            Node funcioncall_node = new Node("Main_Function");

            funcioncall_node.Children.Add(match(Token_Class.Idenifier));


            funcioncall_node.Children.Add(match(Token_Class.LParanthesis));
            funcioncall_node.Children.Add(Input());
            funcioncall_node.Children.Add(match(Token_Class.RParanthesis));



            return funcioncall_node;

        }
        Node Input()
        {
            Node input_node = new Node("Input");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                input_node.Children.Add(match(Token_Class.Idenifier));
                input_node.Children.Add(Input_Dash());
            }


            return input_node;
        }
        Node Input_Dash()
        {
            Node input_node = new Node("Input");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                input_node.Children.Add(match(Token_Class.Comma));
                input_node.Children.Add(match(Token_Class.Idenifier));
                input_node.Children.Add(Input_Dash());
            }


            return input_node;
        }
        Node Data_Type()
        {
            Node dtype_node = new Node("Data_Type");
            if (InputPointer < TokenStream.Count)
            {
                switch (TokenStream[InputPointer].token_type)
                {
                    case Token_Class.Integer:

                        dtype_node.Children.Add(match(Token_Class.Integer));
                        break;
                    case Token_Class.Float:

                        dtype_node.Children.Add(match(Token_Class.Float));
                        break;
                    case Token_Class.String:

                        dtype_node.Children.Add(match(Token_Class.String));
                        break;
                }
            }
            return dtype_node;
        }
        Node Parameters()
        {
            Node parameter_node = new Node("Parameters");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                parameter_node.Children.Add(Data_Type());
                parameter_node.Children.Add(match(Token_Class.Idenifier));


                parameter_node.Children.Add(Parameters_Dash());
            }


            return parameter_node;


        }

        Node Parameters_Dash()
        {
            Node parameter_node = new Node("Parameters");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parameter_node.Children.Add(match(Token_Class.Comma));
                parameter_node.Children.Add(Data_Type());
                parameter_node.Children.Add(match(Token_Class.Idenifier));

                parameter_node.Children.Add(Parameters_Dash());
            }


            return parameter_node;


        }
        Node Multi_Stats()
        {
            Node mstat_node = new Node("Multi_Stats");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Idenifier
                || TokenStream[InputPointer].token_type == Token_Class.StringValue
                || TokenStream[InputPointer].token_type == Token_Class.Constant
                || TokenStream[InputPointer].token_type == Token_Class.Idenifier
                || TokenStream[InputPointer].token_type == Token_Class.Integer
                || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String
                || TokenStream[InputPointer].token_type == Token_Class.Write
                || TokenStream[InputPointer].token_type == Token_Class.Read
                || TokenStream[InputPointer].token_type == Token_Class.If
                || TokenStream[InputPointer].token_type == Token_Class.Repeat))
            {
                mstat_node.Children.Add(Statments());
                mstat_node.Children.Add(Multi_Stats());

            }
            return mstat_node;
        }

        Node Statments()
        {
            Node stats_node = new Node("Statments");
            if (InputPointer < TokenStream.Count)
            {

                switch (TokenStream[InputPointer].token_type)
                {
                    case Token_Class.Integer:
                    case Token_Class.Float:
                    case Token_Class.String:

                        stats_node.Children.Add(Declaration_statement());// weloo
                        break;
                    case Token_Class.Write:
                        stats_node.Children.Add(WriteStatement()); // weloo
                        break;
                    case Token_Class.Read:
                          stats_node.Children.Add(Read_Statement()); // hisham
                        break;
                    case Token_Class.If:
                        stats_node.Children.Add(If_Statment());
                        break;
                    case Token_Class.Repeat:
                        stats_node.Children.Add(RepeatStatement());//hisham
                        break;
                    case Token_Class.Idenifier:
                        if (InputPointer < TokenStream.Count - 1)
                        {
                            if (TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp)
                            {
                                stats_node.Children.Add(Assignment_Statement());//hisham
                                break;
                            }
                        }
                        stats_node.Children.Add(Expression());
                        break;
                    case Token_Class.StringValue:
                    case Token_Class.LParanthesis:
                    case Token_Class.Constant:

                        stats_node.Children.Add(Expression());
                        break;



                }
            }
            return stats_node;
        }

        //----

        Node Equation()
        {
            Node equation = new Node("equation");
            equation.Children.Add(Factor());
            equation.Children.Add(Equation_Dash());
            return equation;
        }
        Node Equation_Dash()
        {
            Node equation_dash = new Node("equation_dash");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    equation_dash.Children.Add(Addop());
                    equation_dash.Children.Add(Factor());
                    equation_dash.Children.Add(Equation_Dash());
                    return equation_dash;
                }
                else
                {
                    return null;
                }
            }
            return equation_dash;
        }
        Node Factor()
        {
            Node factor = new Node("factor");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    factor.Children.Add(match(Token_Class.LParanthesis));
                    factor.Children.Add(Equation());
                    factor.Children.Add(match(Token_Class.RParanthesis));
                    factor.Children.Add(Factor_dash());
                }
                else
                {
                    factor.Children.Add(Term());
                    factor.Children.Add(Factor_dash());
                }
            }
            return factor;
        }
        Node Factor_dash()
        {
            Node factor_dash = new Node("factor_dash");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    factor_dash.Children.Add(Multop());
                    factor_dash.Children.Add(Equation());
                    factor_dash.Children.Add(Factor_dash());
                    return factor_dash;
                }
                else
                {
                    return null;
                }
            }
            return factor_dash;
        }
        Node Term()
        {
            Node term = new Node("term");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Constant)
                {
                    term.Children.Add(match(Token_Class.Constant));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer < TokenStream.Count)
                {

                    if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                    {
                        term.Children.Add(Function_Call());

                    }
                    else
                    {
                        term.Children.Add(match(Token_Class.Idenifier));
                    }
                }
            }
            return term;
        }
        Node Multop()
        {
            Node multop = new Node("multop");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    multop.Children.Add(match(Token_Class.MultiplyOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp) ;
                {
                    multop.Children.Add(match(Token_Class.DivideOp));
                }
            }
            return multop;

        }
        Node Addop()
        {
            Node addop = new Node("addop");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    addop.Children.Add(match(Token_Class.PlusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp) ;
                {
                    addop.Children.Add(match(Token_Class.MinusOp));
                }
            }

            return addop;

        }
        Node Expression()
        {
            Node expression = new Node("expression");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    expression.Children.Add(match(Token_Class.String));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    expression.Children.Add(Term());
                }
                else
                {
                    expression.Children.Add(Equation());
                }
            }
            return expression;
        }
        Node If_Statment()
        {
            Node if_statment = new Node("if_statment");
            if_statment.Children.Add(match(Token_Class.If));
            if_statment.Children.Add(Condition_Statement()); /*hisham function*/
            if_statment.Children.Add(match(Token_Class.Then));
            if_statment.Children.Add(Multi_Stats());
            if_statment.Children.Add(Else_If_Statment());
            if_statment.Children.Add(Else());
            return if_statment;
        }
        Node Else_If_Statment()
        {
            Node else_if_statment = new Node("else_if_statment");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ElseIF)
                {

                    else_if_statment.Children.Add(match(Token_Class.ElseIF));
                    else_if_statment.Children.Add(Condition_Statement()); /*hisham function*/
                    else_if_statment.Children.Add(match(Token_Class.Then));
                    else_if_statment.Children.Add(Multi_Stats()); /*mostafa function*/
                    else_if_statment.Children.Add(Else_If_Statment());
                    return else_if_statment;
                }
                else
                {
                    return null;
                }
            }
            return else_if_statment;
        }
        Node Else()
        {
            Node eelse = new Node("else");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    eelse.Children.Add(Multi_Stats()); /*mostafa function*/
                    eelse.Children.Add(match(Token_Class.End));
                }
                else
                {
                    eelse.Children.Add(match(Token_Class.End));
                }
            }
            return eelse;
        }


        Node Declaration_statement()
        {
            Node node = new Node("Declaration_statement");
            Token_Class tokenClass = TokenStream[InputPointer].token_type;
            if (InputPointer < TokenStream.Count)
            {
                if (tokenClass == Token_Class.Integer)
                {
                    node.Children.Add(match(Token_Class.Integer));
                    node.Children.Add(IdList());
                }
                else if (tokenClass == Token_Class.Float)
                {
                    node.Children.Add(match(Token_Class.Float));
                    node.Children.Add(IdList());
                }
                else if (tokenClass == Token_Class.String)
                {
                    node.Children.Add(match(Token_Class.String));
                    node.Children.Add(IdList());
                }
            }
            return node;
        }
        Node IdList()
        {
            Node node = new Node("IdList");
            Token_Class tokenClass = TokenStream[InputPointer].token_type;
            if (InputPointer < TokenStream.Count && tokenClass == Token_Class.Idenifier)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(IdListDash());

            }
            return node;
        }
        Node IdListDash()
        {
            Node node = new Node("IdListDash");
            Token_Class tokenClass = TokenStream[InputPointer].token_type;
            if (InputPointer < TokenStream.Count)
            {
                if (tokenClass == Token_Class.Comma)
                {
                    node.Children.Add(match(Token_Class.Comma));
                    node.Children.Add(match(Token_Class.Idenifier));
                    node.Children.Add(IdListDash());
                }
                else
                    return null;
            }
            return node;
        }
        Node WriteStatement()
        {
            Node node = new Node("WriteStatement");
            Token_Class tokenClass = TokenStream[InputPointer].token_type;
            if (InputPointer < TokenStream.Count && tokenClass == Token_Class.Write)
            {
                node.Children.Add(match(Token_Class.Write));
                node.Children.Add(Expression());
                node.Children.Add(match(Token_Class.Semicolon));
            }
            return node;
        }
        Node ReturnStatement()
        {
            Node node = new Node("ReturnStatement");
            Token_Class tokenClass = TokenStream[InputPointer].token_type;
            if (InputPointer < TokenStream.Count && tokenClass == Token_Class.Return)
            {
                node.Children.Add(match(Token_Class.Return));
                node.Children.Add(Expression());
                node.Children.Add(match(Token_Class.Semicolon));
            }
            return node;
        }
        Node RepeatStatement()
        {
            Node node = new Node("RepeatStatement");
            Token_Class tokenClass = TokenStream[InputPointer].token_type;
            if (InputPointer < TokenStream.Count && tokenClass == Token_Class.Repeat)
            {
                node.Children.Add(match(Token_Class.Repeat));
                node.Children.Add(Multi_Stats());
                node.Children.Add(match(Token_Class.Until));
                node.Children.Add(Condition_Statement());
            }
            return node;
        }


        // hisham

        Node Read_Statement()
        {
            Node node = new Node("Read_Statment");

            node.Children.Add(match(Token_Class.Read));
            node.Children.Add(Expression());
            node.Children.Add(match(Token_Class.Semicolon));

            return node;
            

        }
        Node Condition_Operator()
        {
            Node node = new Node("Condition_Operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                {
                    node.Children.Add(match(Token_Class.LessThanOp));

                }
                else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                {
                    node.Children.Add(match(Token_Class.GreaterThanOp));

                }
                else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                {
                    node.Children.Add(match(Token_Class.EqualOp));

                }
                else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
                {
                    node.Children.Add(match(Token_Class.NotEqualOp));

                }
            }
            return node;


        }

     


        Node Assignment_Statement()
        {
            Node node = new Node("Assignment_Statement");
          
            node.Children.Add(match(Token_Class.Idenifier));
            node.Children.Add(match(Token_Class.AssignOp));
            node.Children.Add(Expression());
            node.Children.Add(match(Token_Class.Semicolon));

        
            return node;

        }


        Node Condition_Statement()
        {
            Node node = new Node("Condition_Statement");
            node.Children.Add(Condition());
            node.Children.Add(Op());
            return node;

        }// Implement your logic here
        Node Op()
        {
            Node node = new Node("op");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOP)
                {
                    node.Children.Add(match(Token_Class.AndOP));
                    node.Children.Add(Condition_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.OrOP)
                {
                    node.Children.Add(match(Token_Class.OrOP));
                    node.Children.Add(Condition_Statement());
                }
            }
            return node;
        }
        Node Condition()
        {
            Node node = new Node("Condition");
            node.Children.Add(match(Token_Class.Idenifier));
            node.Children.Add(Condition_Operator());
            node.Children.Add(Term());
            return node;
        }
        // Implement your logic here



        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
