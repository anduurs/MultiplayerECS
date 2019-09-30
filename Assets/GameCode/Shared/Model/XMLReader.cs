using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace FNZ.Shared.Model
{
	public class XMLReader
	{
		public static IEnumerable<Type> GetAllTypesOf<T>()
		{
			return typeof(T).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
		}

		public static List<T> Load<T>(string path)
		{
			XmlRootAttribute xRoot = new XmlRootAttribute();
			xRoot.ElementName = "Defs";
			xRoot.IsNullable = true;

			IEnumerable<Type> dataTypes = GetAllTypesOf<DataDef>();
			IEnumerable<Type> componentDataTypes = GetAllTypesOf<DataComponent>();

			int i = 0;
			foreach (var type in dataTypes)
			{
				i++;
			}

			foreach (var type in componentDataTypes)
			{
				i++;
			}

			Type[] typesArr = new Type[i];

			i = 0;

			foreach (var type in dataTypes)
			{
				typesArr[i] = type;
				i++;
			}

			foreach (var type in componentDataTypes)
			{
				typesArr[i] = type;
				i++;
			}

			var serializer = new XmlSerializer(typeof(List<T>), null, typesArr, xRoot, null);

			StreamReader reader = new StreamReader(path);

			try
			{
				var obj = (List<T>)serializer.Deserialize(reader);

				return obj;
			}
			catch (InvalidOperationException ioe)
			{

				Debug.LogError(path);
				Debug.LogError(ioe.Message);
				Debug.LogError(ioe.StackTrace);

				throw new InvalidOperationException("XML READ FAILED!");
			}
		}
	}
}

