using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.IO;

namespace romajiconvert
{
	class JsonHelper
	{
		public static T Parse<T>(string input)
		{
			DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
			{
				return (T)s.ReadObject(stream);
			}
		}

		public static string Stringify<T>(T input)
		{
			DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream())
			{
				s.WriteObject(stream, input);
				stream.Position = 0;
				StreamReader reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}
	}
}
