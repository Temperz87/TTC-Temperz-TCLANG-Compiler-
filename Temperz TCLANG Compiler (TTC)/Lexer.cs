using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Lexer
{
    private static string[] operators = new string[] { "+", "-", "*", "/", "%", "|", "&", "^", "~", "||", "&&", "!", "==", "!=", "<", "<=", ">", ">=", "=", "+=", "-=", "*=", "/=", "++", "--", ".", "->" }; // I hope I didn't miss one!
    private static string[] keywords = new string[] { "bool", "break", "calloc", "char", "const", "continue", "else", "elseif", "enum", "false", "for", "float", "free", "goto", "if", "int", "lengthof", "malloc", "nextbyte", "outputbyte", "realloc", "return", "sizeof", "static", "string", "struct", "true", "typeof", "void", "while" }; // I definitley missed one of these
    private static string[] punctuators = new string[] { "{", "}", ";", "(", ")", "[", "]", "," };

    public static IEnumerable<Token>GetTokens(string[] code)
    {
        foreach (string line in code)
        {
            string currToken = line;

            IEnumerable<Token> findTokens(string[] list, TokenType tokenType)
            {
                foreach (string toFind in list)
                {
                    while (currToken.IndexOf(toFind) != -1)
                    {
                        yield return new Token(toFind, tokenType);

                        int startIdx = currToken.IndexOf(toFind);
                        int endIdx = startIdx + toFind.Length;

                        currToken = (startIdx > 0 ? currToken.Substring(0, startIdx) + (" ") : "") + (endIdx < currToken.Length ? currToken.Substring(endIdx + 1) : "");
                    }
                }
            }

            int commentIdx = currToken.IndexOf("//");
            if (commentIdx != -1)
                currToken = currToken.Substring(0, commentIdx);

            /* TODO: Figure out what special characters are
            while (currToken.IndexOf("\\") != -1)
            {
                int startIdx = currToken.IndexOf("\\");
                int endIdx = startIdx + 2;
                string token = "" + currToken[startIdx] + currToken[startIdx + 1];
                tokens.Add(new Token(token, TokenType.SpecialCharacter));

                currToken = (startIdx > 0 ? currToken.Substring(0, startIdx) + (" ") : "") + (endIdx < currToken.Length ? currToken.Substring(endIdx) : "");
            }
            */

            while (currToken.IndexOf("\"") != -1)
            {
                int startIdx = currToken.IndexOf("\"");
                int endIdx = startIdx + currToken.Substring(startIdx + 1).IndexOf("\"") + 1;
                string token = currToken.Substring(startIdx, endIdx - startIdx + 1);
                yield return new Token(token, TokenType.Literal);

                currToken = (startIdx > 0 ? currToken.Substring(0, startIdx) + (" ") : "") + (endIdx < currToken.Length ? currToken.Substring(endIdx + 1) : "");
            }

            foreach (Token t in findTokens(operators, TokenType.Operator))
                yield return t;
            foreach (Token t in findTokens(keywords, TokenType.Keyword))
                yield return t;
            foreach (Token t in findTokens(punctuators, TokenType.Punctuator))
                yield return t;

            foreach (string identifier in Regex.Split(currToken, @"\s"))
            {
                if (string.IsNullOrEmpty(identifier))
                    continue;

                char startChar = identifier[0];
                if ((char)(startChar & ~32) >= 'A' && (char)(startChar & ~32) <= 'Z' || startChar == '_')
                    yield return new Token(identifier, TokenType.Identifier);
                else if (int.TryParse(identifier, out _))
                    yield return new Token(identifier, TokenType.Constant);
                else
                    throw new Exception("Failed to find valid token type for " + identifier);
            }
        }
    }
}

public enum TokenType
{
    Identifier,
    Operator,
    Constant,
    Keyword,
    Literal,
    Punctuator,
    SpecialCharacter
}

public struct Token
{
    public string Name;
    public TokenType Type;

    public Token(string name, TokenType type)
    {
        this.Name = name;
        this.Type = type;
    }
}