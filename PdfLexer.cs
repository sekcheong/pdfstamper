using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pdfstamper.Objects;

namespace pdfstamper
{
	public class PdfLexer
	{
		private Stream _stream = null;
		private bool _backUp = false;
		private int _lastChar;
		private int _currChar;
		private long _tokenStart;
		private List<byte> _buffer;

		private static PdfToken _whiteSpaceToken = new PdfToken(PdfToken.TokenType.SPACE);

		//private static Token _eof = new Token(Token.TokenType.EOF);
		//private static Token _eol = new Token(Token.TokenType.EOL);

		byte[] specialChars = new byte[] {
			1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0,   // 00
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // 10
			1, 0, 0, 0, 0, 2, 0, 0, 2, 2, 0, 0, 0, 0, 0, 2,   // 20
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0,   // 30
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // 40
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 0,   // 50
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // 60
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 0,   // 70
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // 80
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // 90
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // A0
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // B0
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // C0
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // D0
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   // E0
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0    // F0
		};

		byte[] signature = new byte[] { 0x25, 0xE2, 0xE3, 0xCF, 0xD3 };


		//space characters
		private const int CHAR_NULL = 0x00;
		private const int CHAR_TAB = 0x09;
		private const int CHAR_LF = 0x0A;
		private const int CHAR_FF = 0x0C;
		private const int CHAR_CR = 0x0D;
		private const int CHAR_SP = 0x20;

		//special characters
		private const int CHAR_PERCENT = 0x25;
		private const int CHAR_SLASH = 0x2f;
		private const int CHAR_LEFT_PAR = 0x28;
		private const int CHAR_RIGHT_PAR = 0x29;
		private const int CHAR_LEFT_ANGLE = 0x3c;
		private const int CHAR_RIGHT_ANGLE = 0x3e;
		private const int CHAR_LEFT_SQUARE = 0x5b;
		private const int CHAR_RIGHT_SQUARE = 0x5d;
		private const int CHAR_LEFT_BRACE = 0x7b;
		private const int CHAR_RIGHT_BRACE = 0x7d;

		//number prefixes
		private const int CHAR_0 = 0x30;
		private const int CHAR_9 = 0x39;
		private const int CHAR_DOT = 0x2e;
		private const int CHAR_PLUS = 0x2b;
		private const int CHAR_MINUS = 0x2d;

		//end of file
		private const int CHAR_EOF = -1;


		public PdfLexer(Stream stream)
		{
			_stream = stream;
			_buffer = new List<byte>();
		}

		private int readByte()
		{
			if (_backUp) {
				_backUp = false;
				return _currChar;
			}
			else {
				_lastChar = _currChar;
				_currChar = _stream.ReadByte();
				return _currChar;
			}
		}


		private void putBack()
		{
			_backUp = true;
		}


		private bool isWhiteSpace(int ch)
		{
			if (ch < 0) return false;
			return specialChars[(byte)ch] == 1;
		}


		private bool isDelimiter(int ch)
		{
			if (ch < 0) return false;
			return specialChars[(byte)ch] == 2;
		}


		private bool isDigit(int ch)
		{
			if (ch >= CHAR_0 && ch <= CHAR_9) return true;
			return false;
		}


		private bool isNumberPrefix(int ch)
		{
			if (isDigit(ch)) return true;
			if (ch == CHAR_DOT || ch == CHAR_PLUS || ch == CHAR_MINUS) return true;
			return false;
		}


		public PdfToken GetNextToken()
		{
			while (true) {
				
				_tokenStart = this.GetPosition();
				int ch = this.readByte();

				if (ch == -1) {
					return new PdfToken(PdfToken.TokenType.EOF);
				}

				switch (ch) {
					case CHAR_PERCENT:
						skipComment();
						break;

					case CHAR_SLASH:
						return matchName();

					case CHAR_LEFT_ANGLE:
						ch = readByte();
						if (ch == CHAR_LEFT_ANGLE) {
							return new PdfToken(PdfToken.TokenType.DICT_START);
						}
						else {
							putBack();
							return matchHexString();
						}

					case CHAR_RIGHT_ANGLE:
						ch = readByte();
						if (ch == CHAR_LEFT_ANGLE) {
							return new PdfToken(PdfToken.TokenType.DICT_END);
						}
						else {
							putBack();
						}
						break;


					case CHAR_LEFT_SQUARE:
						return new PdfToken(PdfToken.TokenType.ARRAY_START);

					case CHAR_RIGHT_SQUARE:
						return new PdfToken(PdfToken.TokenType.ARRAY_END);

					default:
						if (isWhiteSpace((byte)ch)) {
							return matchWhiteSpaces();
						}
						else if (isNumberPrefix(ch)) {
							return matchNumber(ch);
						}
						else {
							return matchKeyword(ch);
						}
				}
			}
		}

		
		private PdfToken matchArray()
		{
			throw new NotImplementedException();
		}

		
		private PdfToken matchHexString()
		{
			throw new NotImplementedException();
		}


		private PdfToken matchWhiteSpaces()
		{
			while (true) {
				int ch = this.readByte();
				if (!isWhiteSpace(ch)) {
					putBack();
					return _whiteSpaceToken;
				}
			}
		}


		private PdfToken matchKeyword(int ch)
		{
			_buffer.Clear();
			while (true) {
				ch = readByte();
				if (ch == CHAR_EOF || isWhiteSpace(ch) || isDelimiter(ch)) {
					putBack();
					PdfToken token = new PdfToken(PdfToken.TokenType.KEYWORD, _buffer.ToArray());
				}
				else {
					_buffer.Add((byte)ch);
				}
			}
		}


		private PdfToken matchNumber(int ch)
		{
			_buffer.Clear();
			while (true) {
				_buffer.Add((byte)ch);
				ch = readByte();
				if (ch == CHAR_EOF || isWhiteSpace(ch) || isDelimiter(ch)) {

					putBack();

					if (_buffer.Count == 0) {
						throw new Exception("Invalid number object: The number cannot be null.");
					}

					byte[] data = _buffer.ToArray();
					string val = Encoding.ASCII.GetString(data);

					if (val.Contains(".")) {
						try {
							double n = double.Parse(val);
							PdfToken token = new PdfToken(PdfToken.TokenType.NUMBER, data, PdfToken.TokenSubType.REAL, n);
							return token;
						}
						catch (Exception ex) {
							throw new Exception("Invalid number object: '" + val + "' is not a valid number.", ex);
						}
					}
					else {
						try {
							int n = int.Parse(val);
							PdfToken token = new PdfToken(PdfToken.TokenType.NUMBER, data, PdfToken.TokenSubType.INTEGER, n);
							return token;
						}
						catch (Exception ex) {
							throw new Exception("Invalid number object: '" + val + "' is not a valid number.", ex);
						}
					}
				}
			}
		}

		
		private PdfToken matchName()
		{
			_buffer.Clear();
			while (true) {
				int ch = readByte();
				if (ch == CHAR_EOF || isWhiteSpace(ch) || isDelimiter(ch)) {
					putBack();
					if (_buffer.Count == 0) throw new Exception("Invalid name object: The name cannot be null.");
					PdfToken token = new PdfToken(PdfToken.TokenType.NAME, _buffer.ToArray());
					return token;
				}
				else {
					_buffer.Add((byte)ch);
				}
			}
		}

		
		private void skipComment()
		{
			int ch = readByte();
			while (true) {
				ch = readByte();
				if (ch == CHAR_EOF || ch == CHAR_LF || ch == CHAR_LF) {
					return;
				}
			}
		}

		private void SetTokenPosition(PdfToken token)
		{			
			token.Offset = _tokenStart;
			token.Length = this.GetPosition() - _tokenStart;
		}


		public void SetPosition(long offset)
		{
			_backUp = false;
			_buffer.Clear();
			_stream.Seek(offset, SeekOrigin.Begin);
		}


		public long GetPosition()
		{
			if (_backUp) return _stream.Position - 1;
			else return _stream.Position;
		}
	}
}