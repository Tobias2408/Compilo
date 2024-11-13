using Compilo;
using NUnit.Framework;

namespace ComiloUnitTesting
{
    [TestFixture]
    public class ScannerTests
    {
        private Scanner scanner;

        [SetUp]
        public void Setup()
        {
            // No setup required for now
        }

        [Test]
        public void ScanTokens_SingleCharacterTokens_ReturnsCorrectTokens()
        {
            // Arrange
            string input = "(){},.-+;*";
            Scanner scanner = new Scanner(input);

            // Act
            var tokens = scanner.ScanTokens();

            // Assert
            var expectedTypes = new TokenType[]
            {
                TokenType.LEFT_PAREN,
                TokenType.RIGHT_PAREN,
                TokenType.LEFT_BRACE,
                TokenType.RIGHT_BRACE,
                TokenType.COMMA,
                TokenType.DOT,
                TokenType.MINUS,
                TokenType.PLUS,
                TokenType.SEMICOLON,
                TokenType.STAR,
                TokenType.EOF
            };

            Assert.That(tokens.Count, Is.EqualTo(expectedTypes.Length), "Token count mismatch.");

            for (int i = 0; i < expectedTypes.Length; i++)
            {
                Assert.That(tokens[i].type, Is.EqualTo(expectedTypes[i]), $"Token at index {i} does not match.");
            }
        }



        [Test]
        public void ScanTokens_Numbers_ReturnsNumberTokens()
        {
            // Arrange
            string input = "42 3.1415 0.99";
            Scanner scanner = new Scanner(input);

            // Act
            var tokens = scanner.ScanTokens();

            // Assert
            var expectedTypes = new TokenType[]
            {
        TokenType.NUMBER,
        TokenType.NUMBER,
        TokenType.NUMBER,
        TokenType.EOF
            };

            var expectedLiterals = new double[] { 42, 3.1415, 0.99 };

            Assert.That(tokens.Count, Is.EqualTo(expectedTypes.Length), "Token count mismatch.");

            for (int i = 0; i < expectedLiterals.Length; i++)
            {
                Assert.That(tokens[i].type, Is.EqualTo(expectedTypes[i]), $"Token at index {i} should be NUMBER.");
                Assert.That(tokens[i].literal, Is.EqualTo(expectedLiterals[i]).Within(0.0001), $"Token at index {i} has incorrect literal value.");
            }

            Assert.That(tokens[^1].type, Is.EqualTo(TokenType.EOF), "Last token should be EOF.");
        }




        [Test]
        public void ScanTokens_Operators_ReturnsOperatorTokens()
        {
            // Arrange
            string input = "! != = == < <= > >=";
            Scanner scanner = new Scanner(input);

            // Act
            var tokens = scanner.ScanTokens();

            // Assert
            var expectedTypes = new TokenType[]
            {
                TokenType.BANG,
                TokenType.BANG_EQUAL,
                TokenType.EQUAL,
                TokenType.EQUAL_EQUAL,
                TokenType.LESS,
                TokenType.LESS_EQUAL,
                TokenType.GREATER,
                TokenType.GREATER_EQUAL,
                TokenType.EOF
            };

            Assert.That(tokens.Count, Is.EqualTo(expectedTypes.Length), "Token count mismatch.");

            for (int i = 0; i < expectedTypes.Length; i++)
            {
                Assert.That(tokens[i].type, Is.EqualTo(expectedTypes[i]), $"Token at index {i} does not match.");
            }
        }

        [Test]
        public void ScanTokens_WhitespaceAndNewlines_IgnoresWhitespaceAndCountsLines()
        {
            // Arrange
            string input = "var x = 10;\nvar y = 20;\n\nvar z = x + y;";
            Scanner scanner = new Scanner(input);

            // Act
            var tokens = scanner.ScanTokens();

            // Assert
            // Expected tokens:
            // var, x, =, 10, ;,
            // var, y, =, 20, ;,
            // var, z, =, x, +, y, ;,
            // EOF

            var expectedTypes = new TokenType[]
            {
                TokenType.VAR,
                TokenType.IDENTIFIER,
                TokenType.EQUAL,
                TokenType.NUMBER,
                TokenType.SEMICOLON,

                TokenType.VAR,
                TokenType.IDENTIFIER,
                TokenType.EQUAL,
                TokenType.NUMBER,
                TokenType.SEMICOLON,

                TokenType.VAR,
                TokenType.IDENTIFIER,
                TokenType.EQUAL,
                TokenType.IDENTIFIER,
                TokenType.PLUS,
                TokenType.IDENTIFIER,
                TokenType.SEMICOLON,

                TokenType.EOF
            };

            var expectedLiterals = new object?[]
            {
                null,
                null,
                null,
                10.0,
                null,

                null,
                null,
                null,
                20.0,
                null,

                null,
                null,
                null,
                null,
                null,
                null,
                null
            };

            Assert.That(tokens.Count, Is.EqualTo(expectedTypes.Length), "Token count mismatch.");

            for (int i = 0; i < expectedTypes.Length; i++)
            {
                Assert.That(tokens[i].type, Is.EqualTo(expectedTypes[i]), $"Token at index {i} does not match.");
                if (tokens[i].literal != null)
                {
                    Assert.That(tokens[i].literal, Is.EqualTo(expectedLiterals[i]), $"Token at index {i} has incorrect literal value.");
                }
            }

            // Check line numbers
            // "var x = 10;" -> line 1
            // "var y = 20;" -> line 2
            // "var z = x + y;" -> line 4 (after two newlines)
            Assert.That(tokens[0].line, Is.EqualTo(1), "First 'var' should be on line 1.");
            Assert.That(tokens[5].line, Is.EqualTo(2), "Second 'var' should be on line 2.");
            Assert.That(tokens[10].line, Is.EqualTo(4), "Third 'var' should be on line 4.");
            Assert.That(tokens[^1].line, Is.EqualTo(4), "EOF should be on line 4.");
        }

        [Test]
        public void ScanTokens_UnexpectedCharacters_ReportsErrors()
        {
            // Arrange
            string input = "@ # $ %";
            Scanner scanner = new Scanner(input);

            // Act
            var tokens = scanner.ScanTokens();

                // To be implenmented
        }


        [Test]
        public void ScanTokens_StringLiterals_ReturnsStringTokens()
        {
            // Arrange
            string input = "\"Hello, World!\" \"Another string\"";
            Scanner scanner = new Scanner(input);

            // Act
            var tokens = scanner.ScanTokens();

            // Assert
            var expectedTypes = new TokenType[]
            {
        TokenType.STRING,
        TokenType.STRING,
        TokenType.EOF
            };

            var expectedLiterals = new string?[]
            {
        "Hello, World!",
        "Another string",
        null
            };

            Assert.That(tokens.Count, Is.EqualTo(expectedTypes.Length), "Token count mismatch.");

            for (int i = 0; i < expectedTypes.Length; i++)
            {
                Assert.That(tokens[i].type, Is.EqualTo(expectedTypes[i]), $"Token at index {i} does not match.");
                if (tokens[i].literal != null)
                {
                    Assert.That(tokens[i].literal, Is.EqualTo(expectedLiterals[i]), $"Token at index {i} has incorrect literal value.");
                }
            }
        }

    }
}
