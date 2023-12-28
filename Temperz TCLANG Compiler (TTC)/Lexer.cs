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

    private const bool DEBUG = false;
    private const bool VERBOSE = false;

    public static IEnumerable<Token> GetTokens(string[] code)
    {
        PriorityQueue<TokenInfo, int> queue = new PriorityQueue<TokenInfo, int>(); // This is the first time i've ever used this cursed data structureww

        foreach (string line in code)
        {
            if (string.IsNullOrEmpty(line))
                continue;


            if (DEBUG)
                Console.WriteLine("Now handling line: " + line);
            string currToken = line;

            IEnumerable<TokenInfo> findTokens(string[] list, TokenType tokenType)
            {
                foreach (string toFind in list)
                {
                    while (currToken.IndexOf(toFind) != -1)
                    {
                        yield return new TokenInfo(toFind, tokenType, currToken.IndexOf(toFind));


                        int startIdx = currToken.IndexOf(toFind);
                        int endIdx = startIdx + toFind.Length;

                        if (DEBUG && VERBOSE)
                        {
                            Console.WriteLine("Found token " + toFind + " at " + currToken.IndexOf(toFind));
                            Console.WriteLine("Curr token before: " + currToken);
                        }
                        currToken = ReplaceWhiteSpace(currToken, startIdx, endIdx);
                        if (DEBUG && VERBOSE)
                            Console.WriteLine("Curr token after: " + currToken);
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
                queue.Enqueue(new TokenInfo(token, TokenType.Literal, currToken.IndexOf(token)), currToken.IndexOf(token));

                //currToken = (startIdx > 0 ? currToken.Substring(0, startIdx) + (" ") : "") + (endIdx < currToken.Length ? currToken.Substring(endIdx) : "");
                currToken = ReplaceWhiteSpace(currToken, startIdx, endIdx + 1);
            }

            foreach (TokenInfo t in findTokens(operators, TokenType.Operator))
                queue.Enqueue(t, t.idx);
            foreach (TokenInfo t in findTokens(keywords, TokenType.Keyword))
                queue.Enqueue(t, t.idx);
            foreach (TokenInfo t in findTokens(punctuators, TokenType.Punctuator))
                queue.Enqueue(t, t.idx);

            foreach (string identifier in Regex.Split(currToken, @"\s"))
            {
                if (string.IsNullOrEmpty(identifier))
                    continue;

                char startChar = identifier[0];
                if ((char)(startChar & ~32) >= 'A' && (char)(startChar & ~32) <= 'Z' || startChar == '_')
                    queue.Enqueue(new TokenInfo(identifier, TokenType.Identifier, currToken.IndexOf(identifier)), currToken.IndexOf(identifier));
                else if (int.TryParse(identifier, out _))
                    queue.Enqueue(new TokenInfo(identifier, TokenType.Constant, currToken.IndexOf(identifier)), currToken.IndexOf(identifier));
                else
                    throw new Exception("Failed to find valid token type for " + identifier);

                currToken = ReplaceWhiteSpace(currToken, currToken.IndexOf(identifier), currToken.IndexOf(identifier) + identifier.Length);
            }


            while (queue.Count > 0)
            {
                TokenInfo t = queue.Dequeue();
                //Console.WriteLine("Deuqueued " + t.token.Name + " at " + t.idx);
                yield return t.token;
            }

            if (DEBUG)
                Console.WriteLine();
        }
    }

    private static string ReplaceWhiteSpace(string toWhiteSpace, int startIdx, int endIdx)
    {
        if (startIdx < -1)
            throw new Exception("start idx was negative!");
        else if (toWhiteSpace.Length < endIdx)
            throw new Exception("end idx was out of bounds!");
        else if (endIdx < startIdx)
            throw new Exception("end idx was less than start idx!");

        string result = "";
        if (startIdx > 0)
            result = toWhiteSpace.Substring(0, startIdx);
        for (int i = startIdx; i < endIdx; i++)
            result += " ";


        if (endIdx < toWhiteSpace.Length)
            result += toWhiteSpace.Substring(endIdx);
        return result;
    }

    public struct TokenInfo
    {
        public Token token;
        public int idx;

        public TokenInfo(string name, TokenType type, int idx)
        {
            this.token = new Token(name, type);
            this.idx = idx;
        }

        public TokenInfo(Token token, int idx)
        {
            this.token = token;
            this.idx = idx;
        }
    }
}


// TODO: Move this into its own .cs file

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
