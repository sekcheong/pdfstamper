using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfstamper.Objects
{

	public enum ObjectType
	{
		BOOL,
		NUMBER,		
		STRING,
		NAME,
		ARRAY,
		DICTIONARY,
		STREAM,
		NULL,
		INDIRECT,
		INDIRECT_REFERENCE
	}

	public enum ObjectSubType
	{
		NONE,
		INTEGER,
		REAL,
		LITERAL,
		HEX
	}

	public class PdfObject
	{
		public static PdfObject NullObject = new PdfNullObject();

		public PdfObject() { }

		public PdfObject(ObjectType type) : this(type, ObjectSubType.NONE) { }

		public PdfObject(ObjectType type, ObjectSubType subType)
		{
			this.Type = type;
			this.SubType = subType;
		}

		public ObjectType Type { get; private set; }
		public ObjectSubType SubType { get; private set; }
	}

}
