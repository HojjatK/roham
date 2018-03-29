using System;
using System.IO;
using System.Text;

namespace Roham.Data.DbUp.ScriptExecutor
{
    /// <summary>
    /// Reads SQL commands from an underlying text stream.
    /// </summary>
    internal abstract class DbCommandReader : StringReader
    {
        private const char nullChar = (char)0;
        private const char endOfLineChar = (char)10;
        private const char singleQuoteChar = (char)39;
        private const char dashChar = '-';
        private const char slashChar = '/';
        private const char starChar = '*';

        private const int failedRead = -1;
        private StringBuilder commandScriptBuilder;
        private char lastChar;
        private char currentChar;

        protected DbCommandReader(string sqlText)
            : base(sqlText)
        {
            commandScriptBuilder = new StringBuilder();
        }

        public void ReadAllCommands(Action<string> handleCommand)
        {
            string commandText = string.Empty;
            while (!HasReachedEnd)
            {
                commandText = ReadCommand();
                if (commandText.Length > 0)
                {
                    handleCommand(commandText);
                }
            }
        }

        public string ReadCommand()
        {
            // Command text in buffer start empty. 
            ResetCommandBuffer();

            while (Read() != failedRead)
            {
                if (IsQuote)
                {
                    ReadQuotedString();
                    continue;
                }
                if (IsBeginningOfDashDashComment)
                {
                    ReadDashDashComment();
                    continue;
                }
                if (IsBeginningOfSlashStarComment)
                {
                    ReadSlashStarComment();
                    continue;
                }
                if (IsBeginningOfGo)
                {
                    if (!ReadGo())
                        continue;
                    // This is the end of the command - return the command text in the buffer.
                    return GetCurrentCommandTextFromBuffer().Trim();
                }
                else
                {
                    WriteCurrentCharToCommandTextBuffer();
                }
            }
            return GetCurrentCommandTextFromBuffer().Trim();
        }

        public override int Read()
        {
            var result = base.Read();
            if (result != failedRead)
            {
                lastChar = this.currentChar;
                currentChar = (char)result;
                return result;
            }
            else
            {
                return result;
            }

        }

        protected char LastChar => lastChar;
        protected char CurrentChar => currentChar;
        protected bool HasReachedEnd => Peek() == -1;
        protected bool IsEndOfLine => CurrentChar == endOfLineChar;
        protected bool IsQuote => CurrentChar == singleQuoteChar;
        protected bool IsWhiteSpace => char.IsWhiteSpace(CurrentChar);

        protected bool IsCurrentCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(CurrentChar, comparisonChar);
        }

        protected bool IsLastCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(LastChar, comparisonChar);
        }

        protected bool IsCharEqualTo(char comparisonChar, char compareTo)
        {
            return char.ToLowerInvariant(comparisonChar) == char.ToLowerInvariant(compareTo);
        }

        protected char PeekChar()
        {
            if (HasReachedEnd)
            {
                return nullChar;
            }
            return (char)Peek();
        }

        private bool IsBeginningOfDashDashComment
        {
            get
            {
                if (CurrentChar != dashChar)
                {
                    return false;
                }
                return Peek() == dashChar;
            }
        }

        private bool IsBeginningOfSlashStarComment => CurrentChar == slashChar && Peek() == starChar;

        private bool IsBeginningOfGo
        {
            get
            {
                bool lastCharIsNullOrEmpty = Char.IsWhiteSpace(LastChar) || lastChar == nullChar;
                bool currentCharIsG = IsCurrentCharEqualTo('g');
                bool nextCharIsO = IsCharEqualTo('o', PeekChar());
                return lastCharIsNullOrEmpty && currentCharIsG && nextCharIsO;
            }
        }

        private bool IsEndOfSlashStarComment => LastChar == starChar && CurrentChar == slashChar;

        private void ReadQuotedString()
        {
            WriteCurrentCharToCommandTextBuffer();
            while (Read() != failedRead)
            {
                WriteCurrentCharToCommandTextBuffer();
                if (IsQuote)
                {
                    return;
                }
            }
        }

        private void ReadDashDashComment()
        {
            // Writes the current dash.
            WriteCurrentCharToCommandTextBuffer();
            // Read until we hit the end of line.
            do
            {
                if (Read() == failedRead)
                {
                    break;
                }
                WriteCurrentCharToCommandTextBuffer();
            }
            while (!IsEndOfLine);
        }

        private void ReadSlashStarComment()
        {
            // Write the current slash.
            WriteCurrentCharToCommandTextBuffer();
            // Read until we find a the ending of the slash star comment,
            // Or a nested slash star comment.
            while (Read() != failedRead)
            {
                if (IsBeginningOfSlashStarComment)
                {
                    // Nested comment found - using recursive call to read it.
                    ReadSlashStarComment();
                }
                else
                {
                    WriteCurrentCharToCommandTextBuffer();
                    if (IsEndOfSlashStarComment)
                    {
                        return;
                    }
                    continue;
                }
            }
        }

        private bool ReadGo()
        {
            var g = currentChar;
            // read to the o.
            if (Read() == failedRead)
            {
                return true;
            }
            // Support terminator.
            var peekChar = PeekChar();
            if (peekChar == ';' || peekChar == '\0')
            {
                Read();
            }
            // Check that the statement is indeed a GO and not text starting with Go
            // If it is not a go, add text to buffer and continue
            else if (!char.IsWhiteSpace(peekChar))
            {
                commandScriptBuilder.Append(g);
                commandScriptBuilder.Append(CurrentChar);
                return false;
            }

            return true;
        }

        private void ResetCommandBuffer()
        {
            lastChar = nullChar;
            currentChar = nullChar;
            commandScriptBuilder.Length = 0;
        }

        private void WriteCurrentCharToCommandTextBuffer()
        {
            commandScriptBuilder.Append(CurrentChar);
        }

        private string GetCurrentCommandTextFromBuffer()
        {
            return commandScriptBuilder.ToString();
        }
    }
}
