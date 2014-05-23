using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
	class OGRN
	{
		public string Okved;
		public string Type;
		public string Year;
		public string Region;
		public string Inspection;
		public string Id;
		public string ControlBit;

		public OGRN(string okved)
		{
			Okved = okved;
			Type = okved.Substring(0, 1);
			Year = okved.Substring(1, 2);
			Region = Sequences.RightRegion(okved.Substring(3, 2));
			Inspection = okved.Substring(5, 2);
			if(okved.Length == 13)
			{
				Id = okved.Substring(7, 5);
				ControlBit = okved.Substring(12, 1);
			}
			if(okved.Length == 15)
			{
				Id = okved.Substring(7, 7);
				ControlBit = okved.Substring(14, 1);
			}
		}

	    public static string GetRegion(string okved)
	    {
	        return Sequences.RightRegion(okved.Substring(3, 2));
	    }
	}
}
