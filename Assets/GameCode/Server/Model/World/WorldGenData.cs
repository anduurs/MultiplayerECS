using FNZ.Shared.Model;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FNZ.Server.Model.World
{
	[XmlType("BiomeTileData")]
	public class BiomeTileData
	{
		[XmlElement("tileRef")]
		public string tileRef { get; set; }

		[XmlElement("height")]
		public float height { get; set; }
	}

	[XmlType("WorldGenData")]
	public class WorldGenData : DataDef
	{
		[XmlElement("heightInChunks")]
		public ushort heightInChunks { get; set; }

		[XmlElement("widthInChunks")]
		public ushort widthInChunks { get; set; }

		[XmlElement("chunkSize")]
		public byte chunkSize { get; set; }

		[XmlElement("octaves")]
		public byte octaves { get; set; }

		[XmlElement("roughness")]
		public float roughness { get; set; }

		[XmlElement("layerFrequency")]
		public float layerFrequency { get; set; }

		[XmlElement("layerWeight")]
		public float layerWeight { get; set; }

		[XmlArray("tilesInBiome")]
		[XmlArrayItem(typeof(BiomeTileData))]
		public List<BiomeTileData> tilesInBiome { get; set; }

		[XmlArray("siteRefs")]
		[XmlArrayItem(typeof(string))]
		public List<string> siteRefs { get; set; }
	}
}

