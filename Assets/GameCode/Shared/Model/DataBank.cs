using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace FNZ.Shared.Model
{
	public abstract class DataDef
	{
		[XmlElement("nameDef")]
		public string nameDef { get; set; }
	}

	public abstract class DataComponent { }

	public class DataBank
	{
		private static DataBank instance = null;

		private Dictionary<Type, Dictionary<string, DataDef>> data = new Dictionary<Type, Dictionary<string, DataDef>>();

		public static DataBank Instance => instance ?? (instance = new DataBank());

		private DataBank()
		{
			string[] allfiles = Directory.GetFiles("Data/XML", "*.*", SearchOption.AllDirectories);

			foreach (var file in allfiles)
			{
				List<DataDef> input = XMLReader.Load<DataDef>(file);

                if(input == null)
                    continue;

				foreach (var itemData in input)
				{
					if (!data.ContainsKey(itemData.GetType()))
					{
						data.Add(itemData.GetType(), new Dictionary<string, DataDef>());
					}

					data[itemData.GetType()].Add(itemData.nameDef, itemData);
				}
			}
		}

		public T GetData<T>(string nameDef) where T : DataDef
		{
			if (!data.ContainsKey(typeof(T))) Debug.LogError("Couldn't find data type list " + typeof(T) + " in DataBank");
			if (!data[typeof(T)].ContainsKey(nameDef)) Debug.LogError("Couldn't find nameDef " + nameDef + " in DataBank list " + typeof(T));
			return data[typeof(T)][nameDef] as T;
		}

		public List<Type> GetAllTypes()
		{
			List<Type> types = new List<Type>();
			foreach (var type in data.Keys)
			{
				types.Add(type);
			}
			return types;
		}

		public List<T> GetAllDataDefsOfType<T>() where T : DataDef
		{
			if (!data.ContainsKey(typeof(T))) return null;

			List<T> dataDefs = new List<T>();
			foreach (var dataDef in data[typeof(T)].Values)
			{
				T d = dataDef as T;
				dataDefs.Add(d);
			}
			return dataDefs;
		}

		public bool IsNameDefOfType<T>(string nameDef) where T : DataDef
		{
			return DataBank.Instance.GetAllDataDefsOfType<T>().Find(
				t => t.nameDef.Equals(nameDef)
			) != null;
		}

		public List<DataDef> GetAllDataDefsOfType(Type type)
		{
			if (!data.ContainsKey(type)) return null;

			List<DataDef> dataDefs = new List<DataDef>();
			foreach (var dataDef in data[type].Values)
			{
				dataDefs.Add(dataDef);
			}
			return dataDefs;
		}

		
	}
}

