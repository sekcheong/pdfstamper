using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper
{
	public class PdfToken
	{
		public enum TokenType
		{
			COMMENT,
			SPACE,
			EOL,
			NUMBER,
			NAME,
			KEYWORD,
			STRING,
			BOOL,
			EOF,
			UNKNOWN,
			DICT_START,
			ARRAY_START,
			DICT_END,
			ARRAY_END
		}

		public enum TokenSubType
		{
			NONE,
			REAL,
			INTEGER,
			LITERAL,
			HEX
		}
		
		private byte[] _rawData;

		public PdfToken(TokenType tokenType)
			: this(tokenType, TokenSubType.NONE, null)
		{

		}

		public PdfToken(TokenType tokenType, byte[] rawData)
			: this(tokenType, TokenSubType.NONE, rawData)
		{

		}

		public PdfToken(TokenType tokenType, TokenSubType tokenSubType,  byte[] rawData)
		{
			this.Type = tokenType;
			this.SubType = tokenSubType;
			this.RawData = rawData;
		}


		public PdfToken(TokenType tokenType, byte[] rawData, TokenSubType tokenSubType, double value)
			: this(tokenType, tokenSubType, rawData)
		{
			this.DoubleValue = value;
		}


		public PdfToken(TokenType tokenType, byte[] rawData, TokenSubType tokenSubType, int value)
			: this(tokenType, tokenSubType, rawData)
		{
			this.IntValue = value;
		}


		public TokenType Type { get; private set; }

		public TokenSubType SubType { get; private set; }

		public byte[] RawData
		{
			get { return _rawData; }
			private set { _rawData = value; }
		}

		public double DoubleValue { get; private set; }

		public int IntValue { get; private set; }

		public long LongValue { get; private set; }

		public long Offset { get; internal set; }

		public long Length { get; internal set; }

	}
}