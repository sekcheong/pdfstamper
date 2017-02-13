using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{
	public class PdfName : PdfObject
	{
		public PdfName() : base(ObjectType.NAME) { }
	}
}
