using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{
	public class PdfIndirectReference : PdfObject
	{
		public PdfIndirectReference() : base(ObjectType.INDIRECT_REFERENCE) { }
		public int Number { get; set; }
		public int Generation { get; set; }
		public PdfObject Object { get; set; }
	}
}
