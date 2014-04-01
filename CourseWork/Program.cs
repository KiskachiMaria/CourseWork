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

	static class Program
	{
		static void Main()
		{
			var seq = new Sequences("BG.all.v2.sort");
			seq.Counting();
			seq.WriteInfo();

		}

		private static void CountSumK()
		{
			var ss = new Sequences("BG.all.v2.sort");
			var strs = File.ReadAllLines("BG.edges")
				.GroupBy(x => ss.RightRegion(x.Substring(4, 2)))
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

		private static void Old()
		{
			Converter.ConvertStandartToRegions("BigGraph", "Big_Graph_Region_v3");
			Console.WriteLine("Convert to region fineshed");
			return;
			var fin = new StreamReader("Big_Graph_Region_v3");
			var fout = new StreamWriter("Debug_print");
			var foutData = new StreamWriter("data.gv");

			var dict = new double[9999][];

			for(int i = 0; i < 9999; i++)
				dict[i] = new double[9999];


			while(!fin.EndOfStream)
			{
				var readLine = fin.ReadLine();
				if(readLine == null) continue;
				var s = readLine.Split(new[] { '-', '>', '	' }, StringSplitOptions.RemoveEmptyEntries);
				int v1, v2;
				double v3;
				Int32.TryParse(s[0], out v1);
				Int32.TryParse(s[1], out v2);
				Double.TryParse(s[2], out v3);
				dict[v1][v2] += v3;
			}

			for(int i = 1; i < 9999; i++)
			{
				double max = 0.0;
				int ind = -1;
				string s = "";
				for(int j = 1; j < 9999; j++)
				{
					if(dict[i][j] > max)
					{
						max = dict[i][j];
						ind = j;
					}
					if(Math.Abs(dict[i][j] - 0) > 0.00000000000000000000001)
						s += dict[i][j] + "  ";
				}

				fout.WriteLine(i + "(" + ind + ") : " + s);
			}

			for(var i = 1; i < 9999; i++)
			{
				for(var j = 1; j < 9999; j++)
				{
					if(Math.Abs(dict[i][j] - 0) > 0.00000000000000000000001)
						foutData.WriteLine(i + "--" + j + "[weight=" + dict[i][j] + "];");
				}
			}

			fin.Close();
			fout.Close();
			foutData.Close();
		}
	}
}
