using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CourseWork
{
	class OKVEDStatistic
	{

		public void WriteTopOkvedsForRegion(string inFilename, string outFilename, int topCount = 3)
		{
			var result = new Dictionary<string, List<KeyValuePair<string, int>>>();
			var freq = FrequencyOkvedinDifferentRegion(inFilename);
			using(var file = new StreamWriter(outFilename))
			{
				foreach(var region in freq.Keys)
				{
					result[region] = freq[region].OrderBy(x => x.Value).Reverse().Take(topCount).ToList();
					var topOkveds = result[region].Aggregate("", (current, pair) => current + (pair.Key + " "));
					file.WriteLine(region + "	" + topOkveds);
				}
			}
		}

		private Dictionary<string, Dictionary<string, int>> FrequencyOkvedinDifferentRegion(string filename)
		{
			var freq = new Dictionary<string, Dictionary<string, int>>();
			for(int i = 0; i < 100; i++)
			{
				string iStr;
				if(i < 10)
					iStr = "0" + i;
				else
					iStr = i.ToString(CultureInfo.InvariantCulture);
				freq[iStr] = new Dictionary<string, int>();
			}
			using(var file = new StreamReader(filename))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					var args = str.Split('\t');
					var ogrn = new OGRN(args[0]); 
					var okveds = new List<string>(args[1].Split(','));
					if(!freq[ogrn.Region].ContainsKey(okveds[0])) //Берем только основной оквед
						freq[ogrn.Region][okveds[0]] = 0;
					freq[ogrn.Region][okveds[0]]++;
				}
			}
			return freq;
		}

		public void WriteFrequencyOkveds(string inFilename, string outFilename)
		{
			var freq = new Dictionary<string, int>();
			using(var file = new StreamReader(inFilename))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					var args = str.Split('\t');
					var ogrn = new OGRN(args[0]);
					var okveds = new List<string>(args[1].Split(',')); //Берем только основной оквед
					if(!freq.ContainsKey(okveds[0]))
						freq[okveds[0]] = 0;
					freq[okveds[0]]++;
				}
			}
			using(var file = new StreamWriter(outFilename))
				foreach(var pair in freq)
					file.WriteLine(pair.Key + "	" + pair.Value);
		}
	}
}
