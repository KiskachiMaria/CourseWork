using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CourseWork
{
	internal class OKVEDStatistic
	{
		public void WriteTopOkvedsForRegion(string inFilename, string outFilename, int topCount = 3)
		{
			var result = new Dictionary<string, List<KeyValuePair<string, int>>>();
			Dictionary<string, Dictionary<string, int>> freq = FrequencyOkvedinDifferentRegion(inFilename);
			using(var file = new StreamWriter(outFilename))
			{
				foreach(string region in freq.Keys)
				{
					result[region] = freq[region].OrderBy(x => x.Value).Reverse().Take(topCount).ToList();
					string topOkveds = result[region].Aggregate("", (current, pair) => current + (pair.Key + " "));
					file.WriteLine(region + "	" + topOkveds);
				}
			}
		}

		public void WriteRegionEdgesForOkvedinRegionSimple(string inFilenameOkveds, string inFilenameEdges, string outFilename, bool isInside, int topCount = 100)
		{
			var topOkveds = CountingFrequencyOkveds(inFilenameOkveds).Take(500).Select(x => x.Key).ToList();
			var mainOkved = new Dictionary<string, string>();
			var countEdges = new Dictionary<string, Dictionary<string, int>>();

			using(var file = new StreamReader(inFilenameOkveds))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					string[] args = str.Split('\t');
					string ogrn = args[0];
					mainOkved[ogrn] = args[1].Split(',')[0];
				}
			}

			for(int i = 0; i < 100; i++)
			{
				string iStr;
				if(i < 10)
					iStr = "0" + i;
				else
					iStr = i.ToString(CultureInfo.InvariantCulture);
				countEdges[iStr] = new Dictionary<string, int>();
			}

			foreach(string region in countEdges.Keys)
				foreach(string okved in topOkveds)
					countEdges[region][okved] = 0;

			using(var file = new StreamReader(inFilenameEdges))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					var edge = new Edge(str);
					if(!mainOkved.ContainsKey(edge.From) || !mainOkved.ContainsKey(edge.To))
						continue;

					var regionFrom = OGRN.GetRegion(edge.From);
					var regionTo = OGRN.GetRegion(edge.To);
					var okvedFrom = mainOkved[edge.From];

					if(!topOkveds.Contains(okvedFrom))
						continue;

					if(isInside && regionFrom == regionTo)
						countEdges[regionFrom][okvedFrom]++;
					if(!isInside && regionFrom != regionTo)
						countEdges[regionFrom][okvedFrom]++;
				}
			}

			using(var file = new StreamWriter(outFilename))
			{
				foreach(var okved in topOkveds)
					file.Write(okved + "\t");
				file.WriteLine();
				foreach(var region in countEdges.Keys)
				{
					file.Write(region + "\t\t");
					foreach(string okved in countEdges[region].Keys)
					{
						file.Write(countEdges[region][okved] + "\t");
					}
					file.WriteLine();
				}
			}
		}

		public void WriteRegionEdgesForOkvedinRegion(string inFilenameOkveds, string inFilenameEdges, string outFilename, bool isInside, int topCount = 100)
		{
			var topOkveds = new Dictionary<string, List<string>>();
			Dictionary<string, Dictionary<string, int>> freq = FrequencyOkvedinDifferentRegion(inFilenameOkveds);
			var mainOkved = new Dictionary<string, string>();
			var countEdges = new Dictionary<string, Dictionary<string, int>>();

			foreach(string region in freq.Keys)
				topOkveds[region] = freq[region].OrderBy(x => x.Value).Reverse().Take(topCount).Select(x => x.Key).ToList();
			using(var file = new StreamReader(inFilenameOkveds))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					string[] args = str.Split('\t');
					string ogrn = args[0];
					mainOkved[ogrn] = args[1].Split(',')[0];
				}
			}

			for(int i = 0; i < 100; i++)
			{
				string iStr;
				if(i < 10)
					iStr = "0" + i;
				else
					iStr = i.ToString(CultureInfo.InvariantCulture);
				countEdges[iStr] = new Dictionary<string, int>();
			}

			foreach(string region in countEdges.Keys)
				foreach(string okved in topOkveds[region])
					countEdges[region][okved] = 0;

			using(var file = new StreamReader(inFilenameEdges))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					var edge = new Edge(str);
					if(!mainOkved.ContainsKey(edge.From) || !mainOkved.ContainsKey(edge.To))
						continue;

					var regionFrom = OGRN.GetRegion(edge.From);
					var regionTo = OGRN.GetRegion(edge.To);
					var okvedFrom = mainOkved[edge.From];

					if(!topOkveds[regionFrom].Contains(okvedFrom))
						continue;

					if(isInside && regionFrom == regionTo)
						countEdges[regionFrom][okvedFrom]++;
					if(!isInside && regionFrom != regionTo)
						countEdges[regionFrom][okvedFrom]++;
				}
			}

			using(var file = new StreamWriter(outFilename))
			{
				foreach(string region in countEdges.Keys)
				{
					string str2;
					var str1 = str2 = region + "\t\t";
					foreach(string okved in countEdges[region].Keys)
					{
						str1 += okved + "\t";
						str2 += countEdges[region][okved] + "\t";
					}
					file.WriteLine(str1);
					file.WriteLine(str2);
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
					string[] args = str.Split('\t');
					var ogrn = new OGRN(args[0]);
					var okveds = new List<string>(args[1].Split(','));
					if(!freq[ogrn.Region].ContainsKey(okveds[0])) //Берем только основной оквед
						freq[ogrn.Region][okveds[0]] = 0;
					freq[ogrn.Region][okveds[0]]++;
				}
			}
			return freq;
		}

		private IEnumerable<KeyValuePair<string, int>> CountingFrequencyOkveds(string inFilename)
		{
			var freq = new Dictionary<string, int>();
			using(var file = new StreamReader(inFilename))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					string[] args = str.Split('\t');
					var okveds = new List<string>(args[1].Split(',')); //Берем только основной оквед
					if(!freq.ContainsKey(okveds[0]))
						freq[okveds[0]] = 0;
					freq[okveds[0]]++;
				}
			}
			return freq.OrderBy(x => x.Value).Reverse().ToList();
		}

		public void WriteFrequencyOkveds(string inFilename, string outFilename)
		{
			IEnumerable<KeyValuePair<string, int>> freq = CountingFrequencyOkveds(inFilename);
			using(var file = new StreamWriter(outFilename))
				foreach(var pair in freq)
					file.WriteLine(pair.Key + "	" + pair.Value);
		}

		private const string OtherOKVED = "\"666\"";

		public void CountingOKVEDStandartMetrics(string inFilenameOkveds, string inFilenameEdges, string outFilename)
		{
			List<KeyValuePair<string, int>> freq = CountingFrequencyOkveds(inFilenameOkveds).Take(500).ToList();
			Dictionary<string, int> freqDict = freq.ToDictionary(pair => pair.Key, pair => pair.Value);
			var internalEdges = new Dictionary<string, double>();
			var externalEdges = new Dictionary<string, double>();
			freqDict[OtherOKVED] = 0;
			using(var reader = new StreamReader(inFilenameEdges))
			{
				string line;
				while((line = reader.ReadLine()) != null)
				{
					var edge = new Edge(line);
					if(!freqDict.ContainsKey(edge.From))
					{
						edge.From = OtherOKVED;
						freqDict[OtherOKVED]++;
					}
					if(!freqDict.ContainsKey(edge.To))
					{
						edge.To = OtherOKVED;
						freqDict[OtherOKVED]++;
					}

					if(!internalEdges.ContainsKey(edge.From))
					{
						internalEdges[edge.From] = 0;
						externalEdges[edge.From] = 0;
					}

					if(edge.From == edge.To)
						internalEdges[edge.From] += edge.Ves;
					else
						externalEdges[edge.From] += edge.Ves;
				}
			}

			using(var writer = new StreamWriter(outFilename))
			{
				foreach(string key in internalEdges.Keys.Concat(externalEdges.Keys).Distinct())
				{
					double inter = internalEdges.ContainsKey(key) ? internalEdges[key] : 0;
					double exter = externalEdges.ContainsKey(key) ? externalEdges[key] : 0;
					int fr = freqDict.ContainsKey(key) ? freqDict[key] : 0;
					writer.WriteLine(key + '\t' + inter + '\t' + exter + '\t' + fr);
				}
			}
		}
	}

	internal class Edge
	{
		public string From;
		public string To;
		public double Ves;

		public Edge(string edge)
		{
			string[] e = edge.Split(new[] {'\t', ' ', '>', '-'}, StringSplitOptions.RemoveEmptyEntries);
			if(e[0][0] == 'f')
				e[0] = e[0].Substring(1);
			if(e[1][0] == 'f')
				e[1] = e[1].Substring(1);
			if(e.Count() == 3)
			{
				From = e[0];
				To = e[1];
				Ves = double.Parse(e[2], NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			if(e.Count() == 2)
			{
				From = e[0];
				To = e[1];
			}
		}
	}
}
