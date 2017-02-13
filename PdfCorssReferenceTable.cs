using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfstamper.Objects;

namespace pdfstamper
{
	public class PdfCorssReferenceTable
	{
		private List<PdfObject> _objects = new List<PdfObject>();

		public int firstObject {get;set;}

		public int Count
		{
			get { return _objects.Count; }
		}
	}
}
