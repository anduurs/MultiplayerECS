using System.Collections.Generic;
using System.Xml.Serialization;

namespace FNZ.Shared.Model.World.Tile
{
	[XmlType("TileData")]
	public class TileData : DataDef
	{
		[XmlElement("textureSheet")]
		public string textureSheet { get; set; }

		[XmlElement("textureSheetIndex")]
		public byte textureSheetIndex { get; set; }

		[XmlElement("chanceToSpawnTileObject")]
		public int chanceToSpawnTileObject { get; set; }

		[XmlElement("mapColor")]
		public string mapColor { get; set; } = "#000000";

		[XmlElement("weatherInsulation")]
		public float weatherInsulation { get; set; }

		[XmlArray("tileObjectRefs")]
		[XmlArrayItem(typeof(string))]
		public List<string> tileObjectRefs { get; set; }
	}
}
