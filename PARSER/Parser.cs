﻿using System.Linq.Expressions;
using System.Net;

namespace Hulk
{
    public static class Parser
    {
        public static (int, Expressions) L(List<Token> tokens, int actual)
        {
            /*
            if (tokens.Count == 1)
            {
                if (tokens[actual].Type == Token.TokenType.EndLine)
                {
                    Error error1 = new TypeError(ErrorCode.LexicalError, " Where is ; ?");
                    App.Error(error1.Text());
                    return (0, null)!;
                }
            }*/
            var result = M(tokens, actual);
           while(tokens[result.Item1].Type != Token.TokenType.EndLine){
            result=M(tokens,result.Item1);
           }
           return result;
            // Error error = new TypeError(ErrorCode.SyntacticError, " Where is ; ?");
            // App.Error(error.Text());
            // return (0, null)!;
        }
        public static (int, Expressions) M(List<Token> tokens, int actual, Expressions last = null!)
        {

            if (tokens[actual].Type == Token.TokenType.Token_Point)
            {
                if (tokens[actual + 1].Type == Token.TokenType.Identifier)
                {
                   Point point_expresion =new Point(tokens[actual],Utils.Coordenas());
                   Utils.Diccionary_Value.Add(tokens[actual+1],Point.GetArgument(point_expresion));
                   Utils.Diccionary_Type.Add(tokens[actual+1], Token.TokenType.Token_Point);
                    return (actual + 2, point_expresion);
                }
            }  
              if(tokens[actual].Type==Token.TokenType.Token_Draw)
              {
                if(tokens[actual+1].Type==Token.TokenType.Identifier)
                {
                  //esto es lo q da error en los 2   
                Utils.Result_Dictionary.Add(Utils.Diccionary_Type[tokens[actual+1]],Utils.Diccionary_Value[tokens[actual+1]]);
                }
              }

              
            if (tokens[actual].Type == Token.TokenType.Print)
            {
                if (tokens[actual + 1].Type == Token.TokenType.Open_Paren)
                {
                    var result = M(tokens, actual + 2, last);
                    if (tokens[result.Item1].Type == Token.TokenType.Close_Paren)
                    {
                        return (result.Item1 + 1, result.Item2);
                    }
                    Error error_paren = new TypeError(ErrorCode.SyntacticError, " Where is ) ?");
                    App.Error(error_paren.Text());
                }
                Error error = new TypeError(ErrorCode.SyntacticError, " Where is ( ?");
                App.Error(error.Text());
            }
            if (tokens[actual].Type == Token.TokenType.Token_If)
            {
                if (tokens[actual + 1].Type == Token.TokenType.Open_Paren)
                {
                    var if_part = M(tokens, actual + 2, last);
                    //  var body = M(tokens, if_part.Item1, last);
                    if (tokens[if_part.Item1].Type == Token.TokenType.Close_Paren)
                    {
                        var body = M(tokens, if_part.Item1 + 1);
                        if (tokens[body.Item1].Type == Token.TokenType.Token_Else)
                        {
                            var else_part = M(tokens, body.Item1 + 1);
                            return (else_part.Item1, new ConditionalExpresions(if_part.Item2, body.Item2, else_part.Item2));
                        }
                        Error error = new TypeError(ErrorCode.SyntacticError, " The Conditional needs a else part");
                        App.Error(error.Text());
                    }
                    Error error_close_paren = new TypeError(ErrorCode.SyntacticError, " Where is ) ?");
                    App.Error(error_close_paren.Text());
                }
                Error error_open_paren = new TypeError(ErrorCode.SyntacticError, " Where is ( ?");
                App.Error(error_open_paren.Text());
            }
            if (tokens[actual].Type == Token.TokenType.Token_Let)
            {
                if (tokens[actual + 1].Type == Token.TokenType.Identifier)
                {
                    if (tokens[actual + 2].Type == Token.TokenType.Token_Equal)
                    {
                        var result_let = M(tokens, actual + 3);
                        Expressions id = new Assignment(tokens[actual + 1], result_let.Item2);

                        if (tokens[result_let.Item1].Type == Token.TokenType.Comma)
                        {
                            tokens.Insert(result_let.Item1 + 1, new Token(Token.TokenType.Token_Let, "let"));
                            return M(tokens, result_let.Item1 + 1);
                            //Expressions id2 = new Assignment(tokens[actual + 1], result.Item2);
                            // Expressions second=new LetExpresions(id,result.Item2);
                            //return (result.Item1, second);
                        }
                        if (tokens[result_let.Item1].Type == Token.TokenType.Token_In)
                        {
                            var result_in = M(tokens, result_let.Item1 + 1);
                            Expressions second = new LetExpresions(id, result_in.Item2);
                            return (result_in.Item1, second);
                        }
                        Error error_in_comma = new TypeError(ErrorCode.SyntacticError, " The part needs a in or comma");
                        App.Error(error_in_comma.Text());
                    }
                    Error error_equal = new TypeError(ErrorCode.SyntacticError, " Where is = ?");
                    App.Error(error_equal.Text());
                }
                Error error = new TypeError(ErrorCode.SyntacticError, " Where is identifier ?");
                App.Error(error.Text());
            }

            return A(tokens, actual, last);
        }
        public static (int, Expressions) A(List<Token> tokens, int actual, Expressions last)
        {
            (int, Expressions) result_z;
            if (tokens[actual].Type == Token.TokenType.Token_Not)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions notexpresion = new Unary(result_B.Item2, Unary.Operators.Not);
                result_z = Z(tokens, result_B.Item1, notexpresion);
            }
            else
            {
                var result_B = B(tokens, actual, last);
                result_z = Z(tokens, result_B.Item1, result_B.Item2);
            }
            return result_z;
        }
        public static (int, Expressions) Z(List<Token> tokens, int actual, Expressions last)
        {
            if (tokens[actual].Type == Token.TokenType.Token_And)
            {
                var result_A = A(tokens, actual + 1, last);
                Expressions andexpresions = new BoolExpresions(last, result_A.Item2, BoolExpresions.OperatorsLogic.And);
                return (result_A.Item1, andexpresions);
            }
            if (tokens[actual].Type == Token.TokenType.Token_Or)
            {
                var result_A = A(tokens, actual + 1, last);
                Expressions orexpresions = new BoolExpresions(last, result_A.Item2, BoolExpresions.OperatorsLogic.Or);
                return (result_A.Item1, orexpresions);
            }
            return (actual, last);
        }
        public static (int, Expressions) B(List<Token> tokens, int actual, Expressions last)
        {
            var result_W = W(tokens, actual, last);
            var result_E = E(tokens, result_W.Item1, result_W.Item2);
            return result_E;
        }
        public static (int, Expressions) E(List<Token> tokens, int actual, Expressions last)
        {
            if (tokens[actual].Type == Token.TokenType.Token_DoubleEqual)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions doubleequalexpresions = new BoolExpresions(last, result_B.Item2, BoolExpresions.OperatorsComparison.DoubleEqual);
                return (result_B.Item1, doubleequalexpresions);
            }

            if (tokens[actual].Type == Token.TokenType.Token_LessOrEqual)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions LessOrEquallexpresions = new BoolExpresions(last, result_B.Item2, BoolExpresions.OperatorsComparison.LessOrEqual);
                return (result_B.Item1, LessOrEquallexpresions);
            }

            if (tokens[actual].Type == Token.TokenType.Token_MoreOrEqual)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions MoreOrEqualexpresions = new BoolExpresions(last, result_B.Item2, BoolExpresions.OperatorsComparison.MoreOrEqual);
                return (result_B.Item1, MoreOrEqualexpresions);
            }

            if (tokens[actual].Type == Token.TokenType.Token_NoEqual)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions NoEqualexpresions = new BoolExpresions(last, result_B.Item2, BoolExpresions.OperatorsComparison.NoEqual);
                return (result_B.Item1, NoEqualexpresions);
            }

            if (tokens[actual].Type == Token.TokenType.Token_Less)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions Lessexpresions = new BoolExpresions(last, result_B.Item2, BoolExpresions.OperatorsComparison.Less);
                return (result_B.Item1, Lessexpresions);
            }

            if (tokens[actual].Type == Token.TokenType.Token_More)
            {
                var result_B = B(tokens, actual + 1, last);
                Expressions Moreexpresions = new BoolExpresions(last, result_B.Item2, BoolExpresions.OperatorsComparison.More);
                return (result_B.Item1, Moreexpresions);
            }
            return (actual, last);
        }
        public static (int, Expressions) W(List<Token> tokens, int actual, Expressions last)
        {
            var result_F = F(tokens, actual, last);
            var result_X = X(tokens, result_F.Item1, result_F.Item2);
            return result_X;
        }
        public static (int, Expressions) X(List<Token> tokens, int actual, Expressions last)
        {
            if (tokens[actual].Type == Token.TokenType.Token_Sum)
            {
                var result_W = W(tokens, actual + 1, last);
                Expressions sumexpresions = new ArithmeticBinary(last, result_W.Item2, ArithmeticBinary.Operators.add);
                return (result_W.Item1, sumexpresions);
            }
            if (tokens[actual].Type == Token.TokenType.Token_Dif)
            {
                var result_W = W(tokens, actual + 1, last);
                Expressions difexpresions = new ArithmeticBinary(last, result_W.Item2, ArithmeticBinary.Operators.dif);
                return (result_W.Item1, difexpresions);
            }
            if (tokens[actual].Type == Token.TokenType.Token_Concat)
            {
                var result_W = W(tokens, actual + 1, last);
                Expressions concatexpresions = new ArithmeticBinary(last, result_W.Item2, ArithmeticBinary.Operators.Concat);
                return (result_W.Item1, concatexpresions);
            }
            return (actual, last);
        }
        public static (int, Expressions) F(List<Token> tokens, int actual, Expressions last)
        {
            var result_T = T(tokens, actual, last);
            var result_P = P(tokens, result_T.Item1, result_T.Item2);
            return result_P;
        }
        public static (int, Expressions) P(List<Token> tokens, int actual, Expressions last)
        {
            if (tokens[actual].Type == Token.TokenType.Token_Multi)
            {
                var result_F = F(tokens, actual + 1, last);
                Expressions multexpresions = new ArithmeticBinary(last, result_F.Item2, ArithmeticBinary.Operators.multi);
                return (result_F.Item1, multexpresions);
            }
            if (tokens[actual].Type == Token.TokenType.Token_Div)
            {
                var result_F = F(tokens, actual + 1, last);
                Expressions divexpresions = new ArithmeticBinary(last, result_F.Item2, ArithmeticBinary.Operators.div);
                return (result_F.Item1, divexpresions);
            }
            if (tokens[actual].Type == Token.TokenType.Token_Pow)
            {
                var result_F = F(tokens, actual + 1, last);
                Expressions powexpresions = new ArithmeticBinary(last, result_F.Item2, ArithmeticBinary.Operators.Pow);
                return (result_F.Item1, powexpresions);
            }
            if (tokens[actual].Type == Token.TokenType.Token_Mod)
            {
                var result_F = F(tokens, actual + 1, last);
                Expressions porcentexpresions = new ArithmeticBinary(last, result_F.Item2, ArithmeticBinary.Operators.Mod);
                return (result_F.Item1, porcentexpresions);
            }
            return (actual, last);
        }
        public static (int, Expressions) T(List<Token> tokens, int actual, Expressions last)
        {
            if (tokens[actual].Type == Token.TokenType.Number_Literal)
            {
                return (actual + 1, new Atomic(tokens[actual]));
            }
            if (tokens[actual].Type == Token.TokenType.Chain_Literals)
            {
                return (actual + 1, new Atomic(tokens[actual]));
            }
            if (tokens[actual].Type == Token.TokenType.Token_False)
            {
                return (actual + 1, new Atomic(tokens[actual]));
            }
            if (tokens[actual].Type == Token.TokenType.Token_True)
            {
                return (actual + 1, new Atomic(tokens[actual]));
            }
            if (tokens[actual].Type == Token.TokenType.Token_PI)
            {
                return (actual + 1, new Atomic(tokens[actual]));
            }
            if (tokens[actual].Type == Token.TokenType.Token_Sen)
            {
                var result_W = W(tokens, actual + 1, last);
                return (result_W.Item1, new Unary(result_W.Item2, Unary.Operators.Sen));
            }
            if (tokens[actual].Type == Token.TokenType.Token_Cos)
            {
                var result_W = W(tokens, actual + 1, last);
                return (result_W.Item1, new Unary(result_W.Item2, Unary.Operators.Cos));
            }
            if (tokens[actual].Type == Token.TokenType.Token_Tan)
            {
                var result_W = W(tokens, actual + 1, last);
                return (result_W.Item1, new Unary(result_W.Item2, Unary.Operators.Tan));
            }
            if (tokens[actual].Type == Token.TokenType.Token_Cot)
            {
                var result_W = W(tokens, actual + 1, last);
                return (result_W.Item1, new Unary(result_W.Item2, Unary.Operators.Cot));
            }
            if (tokens[actual].Type == Token.TokenType.Token_Sqrt)
            {
                var result_W = W(tokens, actual + 1, last);
                return (result_W.Item1, new Unary(result_W.Item2, Unary.Operators.Sqrt));
            }
            if (tokens[actual].Type == Token.TokenType.Token_Log)
            {
                var result_W = W(tokens, actual + 1, last);
                return (result_W.Item1, new Unary(result_W.Item2, Unary.Operators.Log));
            }
            if (tokens[actual].Type == Token.TokenType.Open_Paren)
            {
                var result_M = M(tokens, actual + 1, last);
                if (tokens[result_M.Item1].Type == Token.TokenType.Close_Paren)
                {
                    return (result_M.Item1 + 1, result_M.Item2);
                }
                else throw new Exception();
            }
            if (tokens[actual].Type == Token.TokenType.Identifier)
            {
                return (actual + 1, new iDExpresions(tokens[actual]));
            }
            if (tokens[actual].Type == Token.TokenType.Close_Paren)
            {
                return (actual, last);
            }
            if(tokens[actual].Type==Token.TokenType.Token_Point)
            {
             
            }
            throw new Exception();
        }
    }
}