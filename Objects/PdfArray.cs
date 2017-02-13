using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{
	public class PdfArray:PdfObject
	{
		public PdfArray() : base(ObjectType.ARRAY) { }
	}
}
