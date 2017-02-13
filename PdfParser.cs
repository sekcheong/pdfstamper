using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pdfstamper.Objects;

namespace pdfstamper
{
	public class PdfParser
	{
		private PdfLexer _lexer;

		public PdfParser(string fileName)
		{
			FileStream file = File.Open(fileName, FileMode.Open);
			init(file);
		}

		public PdfParser(Stream file)
		{
			init(file);
		}

		private void init(Stream file)
		{
			_lexer = new PdfLexer(file);
		}

		public PdfObject Parse(long offset)
		{
			if (offset >= 0) {
				_lexer.SetPosition(offset);
			}
			PdfToken t = _lexer.GetNextToken();
			return new PdfObject();
		}

		public PdfObject Parse()
		{
			return Parse(-1);
		}

	}
}
