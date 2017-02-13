using System;
using System.Collections.Generic;
using System.Text;

namespace pdfstamper.Objects
{
	public class PdfStream : PdfObject
	{
		public PdfStream() : base(ObjectType.STREAM) { }
	}
}