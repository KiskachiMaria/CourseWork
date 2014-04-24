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

		private IEnumerable<KeyValuePair<string, int>> CountingFrequencyOkveds(string inFilename)
		{
			var freq = new Dictionary<string, int>();
			using(var file = new StreamReader(inFilename))
			{
				string str;
				while((str = file.ReadLine()) != null)
				{
					var args = str.Split('\t');
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
			var freq = CountingFrequencyOkveds(inFilename);
			using(var file = new StreamWriter(outFilename))
				foreach(var pair in freq)
					file.WriteLine(pair.Key + "	" + pair.Value);
		}
		private const string otherOKVED = "\"666\"";
		public void CountingOKVEDStandartMetrics(string inFilenameOkveds, string inFilenameEdges, string outFilename)
		{
			var freq = CountingFrequencyOkveds(inFilenameOkveds).Take(500).ToList();
			var freqDict = freq.ToDictionary(pair => pair.Key, pair => pair.Value);
			var internalEdges = new Dictionary<string, double>();
			var externalEdges = new Dictionary<string, double>();
			freqDict[otherOKVED] = 0;
			using(var reader = new StreamReader(inFilenameEdges))
			{
				string line;
				while((line = reader.ReadLine()) != null)
				{
					var edge = new Edge(line);
					if(!freqDict.ContainsKey(edge.From))
					{
						edge.From = otherOKVED;
						freqDict[otherOKVED]++;
					}
					if(!freqDict.ContainsKey(edge.To))
					{
						edge.To = otherOKVED;
						freqDict[otherOKVED]++;
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
				foreach(var key in internalEdges.Keys.Concat(externalEdges.Keys).Distinct())
				{
					
					var inter =  internalEdges.ContainsKey(key) ? internalEdges[key] : 0;
					var exter =  externalEdges.ContainsKey(key) ? externalEdges[key] : 0;
					var fr = freqDict.ContainsKey(key) ? freqDict[key] : 0;
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
			var e = edge.Split('\t');
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
