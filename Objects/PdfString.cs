using System;
using System.Collections.Generic;
using System.Text;

namespace pdfstamper.Objects
{
	public class PdfString : PdfObject
	{
		public PdfString() : base(ObjectType.STRING, ObjectSubType.LITERAL) { }
		public PdfString(ObjectSubType subType) : base(ObjectType.STRING, subType) { }
	}
}