using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{
	public class PdfDictionary : PdfObject
	{
		public PdfDictionary() : base(ObjectType.DICTIONARY) { }
	}
}
