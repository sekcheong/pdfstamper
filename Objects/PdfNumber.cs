using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{
	public class PdfNumber : PdfObject
	{
		public PdfNumber() : base(ObjectType.NUMBER) { }
		public PdfNumber(ObjectSubType subType) : base(ObjectType.NUMBER, subType) { }
	}
}
