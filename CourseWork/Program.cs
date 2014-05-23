using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CourseWork
{
	public static class Converter
	{
		public static void ConvertStandartToRegions(string fnameIn, string fnameOut)
		{
			var fin = new StreamReader(fnameIn);
			var fout = new StreamWriter(fnameOut);
			const int bufferSize = 5000;
			var buff = new char[bufferSize];
			string residual = "";

			string lastPerson = "";
			var adjacentRegions = new List<string>();
	
			while(!fin.EndOfStream)
			{
				fin.ReadBlock(buff, 0, bufferSize);

				var buffer = new string(buff);
				var strs = new List<string>();

				if(residual != "" && residual[residual.Length - 1] == ']')
					strs.Add(residual);
				strs.AddRange(buffer.Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
				if(residual != "" && residual[residual.Length - 1] != ']')
					strs[0] = residual + strs[0];
				residual = strs.Last();

				string bufferOut = "";

				for(int i = 0; i < strs.Count - 1; i++)
				{
					string[] edge = strs[i].Split(new[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
					if(edge.Count() != 3 || edge[2][3] == '4')
						continue;
					if(edge[0][0] == 'f')
					{
						bufferOut += edge[0] + " -> " + edge[1] + "	" + "1\r\n";
					}
					if(edge[0][0] == 'p')
					{
						if(lastPerson != edge[0])
						{
							for(var regIn = 0; regIn < adjacentRegions.Count; regIn++)
								for(var regOut = regIn + 1; regOut < adjacentRegions.Count; regOut++)
								{
									bufferOut += adjacentRegions[regIn] + " -> " + adjacentRegions[regOut] + "	" + 1.0 / (adjacentRegions.Count - 1.0) + "\r\n";
									bufferOut += adjacentRegions[regOut] + " -> " + adjacentRegions[regIn] + "	" + 1.0 / (adjacentRegions.Count - 1.0) + "\r\n";
								}
							adjacentRegions = new List<string>();
						}
						adjacentRegions.Add(edge[1]);
					}
					lastPerson = edge[0];
				}
				fout.Write(bufferOut);
			}

			fin.Close();
			fout.Close();
		}
	}

	internal static class Program
	{
		private static void Main()
		{
			var okvedStat = new OKVEDStatistic();

//			okvedStat.CountingOKVEDStandartMetrics("okveds", "okveds.edges", "okvedsStandartMetric");
            okvedStat.WriteRegionEdgesForOkvedinRegion("okveds", "okveds.edges", "okvedsNevedomayaHrenInside", true);
            okvedStat.WriteRegionEdgesForOkvedinRegion("okveds", "okveds.edges", "okvedsNevedomayaHrenOutside", false);

//			okvedStat.WriteTopOkvedsForRegion("okveds", "okvedsFrequencyRegions");
//			okvedStat.WriteFrequencyOkveds("okveds", "okvedsFrequency");

//			var seq = new Sequences("BG.all.v2.sort");
//			seq.Counting();
//			seq.WriteInfo();
//			CountSumK();

		}

		private static void CountSumK()
		{
			var strs = File.ReadAllLines("edges.sort.uniq")
			               .GroupBy(x => Sequences.RightRegion(x.Substring(4, 2)))
			               .Select(x =>
			                       new
				                       {
					                       reg = x.Key,
					                       sumK = x.GroupBy(y => y.Substring(4, 4))
					                               .Select(g => Math.Pow(g.Count(), 2))
					                               .Sum()
				                       })
			               .Select(q => q.reg + "	" + q.sumK);
			File.WriteAllLines("Inspect.Rigth", strs);
		}
		
	}
}
