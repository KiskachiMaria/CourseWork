using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CourseWork
{
	class Sequences
	{
		private readonly string fname;
		private int insertInsp;
		private int insertReg;
		private int insertContr;
		private double doubleInsertInsp;
		private double doubleInsertReg;
		private double doubleInsertCountr;
		private int longerInsertInsp;
		private int longerInsertReg;
		private int longerInsertCountr;

		private readonly Dictionary<string, int> regionsInsp;
		private readonly Dictionary<string, int> regionsReg;
		private readonly Dictionary<string, int> regionsCou;
		private readonly Dictionary<string, double> doubleRegionsInsp;
		private readonly Dictionary<string, double> doubleRegionsReg;
		private readonly Dictionary<string, double> doubleRegionsCou;
		private readonly Dictionary<string, int> longerRegionInsp;
		private readonly Dictionary<string, int> longerRegionReg;
		
		public Sequences(string fname)
		{
			this.fname = fname;
			regionsInsp = new Dictionary<string, int>();
			regionsReg = new Dictionary<string, int>();
			regionsCou = new Dictionary<string, int>();
			doubleRegionsInsp = new Dictionary<string, double>();
			doubleRegionsReg = new Dictionary<string, double>();
			doubleRegionsCou = new Dictionary<string, double>();
			longerRegionInsp = new Dictionary<string, int>();
			longerRegionReg = new Dictionary<string, int>();
		}

		public string RightRegion (string reg)
		{
			if(reg == "81")
				return "59";
			if(reg == "84" || reg == "88")
				return "24";
			if(reg == "82")
				return "41";
			if(reg == "85")
				return "38";
			if(reg == "80")
				return "75";
			if(reg == "96")
				return "66";
			if(reg == "98")
				return "78";
			if(reg == "50")
				return "77";
			if(reg == "47")
				return "78";
//			Питер и Москва с облостями
			return reg;
		}

		public void Counting()
		{
			using(var reader = new StreamReader(File.OpenRead(fname)))
			{
				string str;
				string lastOrg = "";
				var links = new List<string>();
				while((str = reader.ReadLine()) != null)
				{
					if(String.IsNullOrWhiteSpace(str))
						continue;
					var edge = str.Split(new[] { ' ', '-', '>', ' ', '	' }, StringSplitOptions.RemoveEmptyEntries);
					if(edge.Count() != 3)
						continue;

					if(lastOrg != edge[0])
					{
						var regionFrom = "";
						var regionTo = "";

						foreach(var link in links)
						{
							regionFrom = RightRegion(lastOrg.Substring(4, 2));
							regionTo = RightRegion(link.Substring(4, 2));
								
							double dummy;
							double.TryParse(link.Substring(15), NumberStyles.Any, CultureInfo.InvariantCulture, out dummy);
							if(lastOrg.Substring(4, 4) == link.Substring(4, 4))//одинаковые инспекции
							{
								insertInsp++;
								doubleInsertInsp += dummy;
								if(!regionsInsp.ContainsKey(regionFrom))
									regionsInsp[regionFrom] = 0;
								regionsInsp[regionFrom]++;
								if(!doubleRegionsInsp.ContainsKey(regionFrom))
									doubleRegionsInsp[regionFrom] = 0;
								doubleRegionsInsp[regionFrom] += dummy;
							}
							if(regionFrom == regionTo)//одинаковые регионы
							{
								insertReg++;
								doubleInsertReg += dummy;
								if(!regionsReg.ContainsKey(regionFrom))
									regionsReg[regionFrom] = 0;
								regionsReg[regionFrom]++;
								if(!doubleRegionsReg.ContainsKey(regionFrom))
									doubleRegionsReg[regionFrom] = 0;
								doubleRegionsReg[regionFrom] += dummy;
							}
							else
							{
								if(!regionsCou.ContainsKey(regionFrom))
									regionsCou[regionFrom] = 0;
								regionsCou[regionFrom]++;
								if(!doubleRegionsCou.ContainsKey(regionFrom))
									doubleRegionsCou[regionFrom] = 0;
								doubleRegionsCou[regionFrom] += dummy;
//								if(!regionsCou.ContainsKey(regionTo))
//									regionsCou[regionTo] = 0;
//								regionsCou[regionTo]++;
//								if(!doubleRegionsCou.ContainsKey(regionTo))
//									doubleRegionsCou[regionTo] = 0;
//								doubleRegionsCou[regionTo] += dummy;
							}
							insertContr++;
							doubleInsertCountr += dummy;
						}

						bool ins = false, reg = false, cou = false;
						foreach(var link in links)
						{
							double dummy;
							double.TryParse(link.Substring(15), NumberStyles.Any, CultureInfo.InvariantCulture, out dummy);
							if(lastOrg.Substring(4, 4) == link.Substring(4, 4))
								ins = true;
							else if(regionFrom == regionTo)
								reg = true;
							else
								cou = true;
							
							if(cou)
								longerInsertCountr++;
							else if(reg)
							{
								longerInsertCountr++;
								longerInsertReg++;
								if(!longerRegionReg.ContainsKey(regionFrom))
									longerRegionReg[regionFrom] = 0;
								longerRegionReg[regionFrom]++;
							}
							else if(ins)
							{
								longerInsertCountr++;
								longerInsertInsp++;
								longerInsertReg++;
								if(!longerRegionInsp.ContainsKey(regionFrom))
									longerRegionInsp[regionFrom] = 0;
								if(!longerRegionReg.ContainsKey(regionFrom))
									longerRegionReg[regionFrom] = 0;
								longerRegionReg[regionFrom]++;
								longerRegionInsp[regionFrom]++;
							}
						}

						links = new List<string>();
					}

					links.Add(edge[1] + "	" + edge[2]);
					lastOrg = edge[0];
					
				}
			}
		}

		public void WriteInfo()
		{
			Console.WriteLine(insertInsp + " " + insertReg + " " + insertContr);
			Console.WriteLine(doubleInsertInsp + " " + doubleInsertReg + " " + doubleInsertCountr);
			Console.WriteLine(longerInsertInsp + " " + longerInsertReg + " " + longerInsertCountr);

			PrintDictionary("Regions.p1.insp", regionsInsp);
			PrintDictionary("Regions.p1.regions", regionsReg);
			PrintDictionary("Regions.p1.country", regionsCou);
			PrintDictionary("Regions.p2.insp", doubleRegionsInsp);
			PrintDictionary("Regions.p2.regions", doubleRegionsReg);
			PrintDictionary("Regions.p2.country", doubleRegionsCou);
			PrintDictionary("Regions.p3.insp", longerRegionInsp);
			PrintDictionary("Regions.p3.regions", longerRegionReg);
		}

		private void PrintDictionary<T>(string filename, Dictionary<string, T> dict)
		{
			using(var file = new StreamWriter(filename))
			{
				foreach(var reg in dict.Keys)
					file.WriteLine(reg + " " + dict[reg]);
			}
		}
	}
}
//f1027708023936 -> f1037700180462    0.0188679245283019