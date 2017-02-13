using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using pdfstamper.Objects;

namespace pdfstamper
{


	class PdfReader
	{
		
		private int BUFF_LENGTH = 1024;

		private byte[] EOF = new byte[] { 0x25, 0x25, 0x45, 0x4F, 0x46 };           //%%EOF

		//Null (NUL), Tab (HT), Line feed (LF), Form feed (FF), Carriage return (CR), Space (SP)
		private char[] BLANKS = new char[] { '\x00', '\x09', '\x0A', '\x0C', '\x0D', '\x20' };  


		private int findEOFPosition(byte[] data) {
			for (int i = 0; i < data.Length-EOF.Length; i++) {				
				for (int j = 0; j < EOF.Length; j++) {
					if (data[i+j] != EOF[j]) {
						break;
					}
					else {
						//checked the entire length of EOF 
						if (j == EOF.Length - 1) return i;
					}
				}				
			}
			//char x = '\x13';
			return -1;
		}

		
		private string[] createPieces(string footer)
		{
			string[] pieces = footer.Split(new char[] { '\n', '\r' });
			List<string> temp = new List<string>();
			for (int i = 0; i < pieces.Length; i++) {
				string p = pieces[i].Trim();
				if (p.Length > 0) {
					temp.Add(p);
				}
			}
			pieces = temp.ToArray();
			return pieces;
		}


		private void readTrailer(FileStream file, out string trailer, out long xrefOffset)
		{
			trailer = "";
			xrefOffset = -1;

			int length = (file.Length > (long)BUFF_LENGTH) ? BUFF_LENGTH : (int)file.Length;
			byte[] buff = new byte[length];
			file.Seek(file.Length - buff.Length, SeekOrigin.Begin);
			file.Read(buff, 0, buff.Length);
			string result = Encoding.ASCII.GetString(buff);
			result = result.TrimEnd();

			if (!result.EndsWith("%%EOF")) throw new Exception("End of file marker does not exit.");
		
			string[] pieces = createPieces(result);

			int trailerStart = -1;			
			for (int i = pieces.Length - 1; i > 0; i--) {
				if (pieces[i] == "trailer") {
					trailerStart = i;
				}
			}
			if (trailerStart == -1) throw new Exception("Invalid PDF file, trailer does not exit.");

			int startxref = -1;
			StringBuilder sb = new StringBuilder();
			for (int i = trailerStart + 1; i < pieces.Length; i++) {
				if (pieces[i] == "startxref") {
					startxref = i;
					break;
				}
				sb.Append(pieces[i]).Append(" ");
			}
			if (startxref < 0) throw new Exception("Invalid trailer format: startxref is missing.");

			string xref = pieces[startxref + 1];
			xrefOffset = long.Parse(xref);
			trailer = sb.ToString().Trim();						
		}


		private void parseTrailer(string trailer)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			string[] pieces = trailer.Split(BLANKS);
			if (pieces[0] != "<<" || pieces[pieces.Length - 1] != ">>") throw new Exception("Invalid trailer format.");
			Queue<string> tokens = new Queue<string>();
			
			for (int i = 1; i < pieces.Length - 1; i++) {
				tokens.Enqueue(pieces[i]);
			}

			while (tokens.Count > 0) {
				string t = tokens.Dequeue();
				Trace.WriteLine(t);
			}

			Trace.WriteLine(pieces);
		}

		
		public PdfReader(string fileName)
		{			
			FileStream file = File.Open(fileName, FileMode.Open);
			string trailer;
			long xrefOffset;

			readTrailer(file, out trailer, out xrefOffset);

			Trace.WriteLine(trailer);
			
			parseTrailer(trailer);

			byte[] buff = new byte[1000];
			file.Seek(xrefOffset, SeekOrigin.Begin);
			file.Read(buff, 0, buff.Length);
			string result = Encoding.ASCII.GetString(buff);

			Trace.WriteLine(result);
		}


		class Program
		{
			static void Main(string[] args)
			{
				string fileName = @"..\..\data\helloworld_annot.pdf";
				PdfParser parser = new PdfParser(fileName);
				PdfObject obj = parser.Parse(0);
			}
		}
	}
}