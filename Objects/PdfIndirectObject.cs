using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{
	public class PdfIndirectObject : PdfObject
	{
		public PdfIndirectObject() : base(ObjectType.INDIRECT) { }

		public long Offset { get; set; }

		public long Length { get; set; }

		public int Number { get; set; }

		public int Generation { get; set; }

		public string Status { get; set; }

		public PdfObject Object { get; set; }
		
	}
}
