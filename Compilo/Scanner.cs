using System;
using System.Collections.Generic;
using System.Globalization;

namespace Compilo
{
    public class Scanner
    {
        private List<Token> tokens { get; } = new();
        private List<string> errors { get; } = new();
        private int start = 0, current = 0, line = 1;

        private static readonly Dictionary<string, TokenType> keywords = new()
        {
            { "and", TokenType.AND }, { "class", TokenType.CLASS },
            { "else", TokenType.ELSE }, { "false", TokenType.FALSE },
            { "for", TokenType.FOR }, { "fun", TokenType.FUN },
            { "if", TokenType.IF }, { "nil", TokenType.NIL },
            { "or", TokenType.OR }, { "print", TokenType.PRINT },
            { "return", TokenType.RETURN }, { "super", TokenType.SUPER },
            { "this", TokenType.THIS }, { "true", TokenType.TRUE },
            { "var", TokenType.VAR }, { "while", TokenType.WHILE }
        };

        private string source { get; }

        public Scanner(string source) => this.source = source;

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else AddToken(TokenType.SLASH);
                    break;
                case ' ': case '\r': case '\t': break;
                case '\n': line++; break;
                case '"': stringy(); break;
                default:
                    if (char.IsDigit(c)) Number();
                    else if (char.IsLetter(c)) Identifier(c);
                    else Error(line, "Unexpected character.");
                    break;
            }
        }

        private void Identifier(char c)
        {
            while (IsAlphaNumeric(Peek())) Advance();
            string text = source[start..current];
            if (!keywords.TryGetValue(text, out TokenType type))
            {
                type = TokenType.IDENTIFIER;
            }
            AddToken(type);
        }

        private bool IsAlphaNumeric(char c) => char.IsLetter(c) || char.IsDigit(c);

        private void Number()
        {
            while (char.IsDigit(Peek())) Advance();
            // Look for fractional part
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                // Consume the '.'
                Advance();
                while (char.IsDigit(Peek())) Advance();
            }
            string numberString = source[start..current];
            if (!double.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out double numberValue))
            {
                Error(line, $"Invalid number literal '{numberString}'.");
                return;
            }
            AddToken(TokenType.NUMBER, numberValue);
        }

        private void stringy()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }
            if (IsAtEnd())
            {
                Error(line, "Unterminated string.");
                return;
            }
            // The closing ".
            Advance();
            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd() || source[current] != expected) return false;
            current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool IsAtEnd() => current >= source.Length;

        private char Advance() => source[current++];

        private void AddToken(TokenType type) => AddToken(type, null);

        private void AddToken(TokenType type, object? literal)
        {
            string text = source[start..current];
            tokens.Add(new Token(type, text, literal, line));
        }

        private void Error(int line, string message)
        {
            errors.Add($"Line {line}: {message}");
        }

        public IReadOnlyList<string> GetErrors() => errors;

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }
            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }
    }
}
