using FNZ.Shared.Model.Entity.Components;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FNZ.Shared.Model.Entity
{
	[XmlType("AnimationData")]
	public class AnimationData
	{
		[XmlElement("action")]
		public string action { get; set; }

		[XmlElement("speed")]
		public float speed { get; set; }

		[XmlElement("animPath")]
		public string animPath { get; set; }
	}

	[XmlType("FNEEntityData")]
	public class FNEEntityData : DataDef
	{
		[XmlAttribute("entityType")]
		public string entityType { get; set; }

		[XmlElement("viewIsGameObject")]
		public bool viewIsGameObject { get; set; }

		[XmlElement("onHitEffectPath")]
		public string onHitEffectPath { get; set; }

		[XmlElement("onDeathEffectPath")]
		public string onDeathEffectPath { get; set; }

		[XmlElement("onDeathEffectRef")]
		public string onDeathEffectRef { get; set; }

		[XmlArray("bodyPartEffectPaths")]
		[XmlArrayItem(typeof(string))]
		public List<string> bodyPartEffectPaths { get; set; }

		[XmlArray("components")]
		[XmlArrayItem(typeof(FNEComponentData))]
		public List<FNEComponentData> components { get; set; }

		public T GetComponentData<T>() where T : FNEComponentData
		{
			foreach (var comp in components)
			{
				if (comp is T) return comp as T;
			}

			return default;
		}

		[XmlElement("pathingCost")]
		public byte pathingCost { get; set; }

		[XmlElement("seeThrough")]
		public bool seeThrough { get; set; }

		[XmlElement("blocking")]
		public bool blocking { get; set; } = true;

		[XmlElement("smallCollisionBox")]
		public bool smallCollisionBox { get; set; } = false;

		[XmlElement("heatTransferFactor")]
		public float heatTransferFactor { get; set; }

		[XmlElement("displayName")]
		public string displayName { get; set; }

		[XmlElement("editorCategory")]
		public string editorCategory { get; set; } = "Misc";

		[XmlElement("description")]
		public string description { get; set; }

		[XmlElement("uiManager")]
		public string uiManager { get; set; }

		[XmlElement("viewManager")]
		public string viewManager { get; set; }

		[XmlElement("prefabPath")]
		public string prefabPath { get; set; }

		[XmlArray("animations")]
		[XmlArrayItem(typeof(AnimationData))]
		public List<AnimationData> animations { get; set; }
	}
}

