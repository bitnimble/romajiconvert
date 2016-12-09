using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace romajiconvert
{
	public class SongList
	{
		public Song[] songs { get; set; }
	}

	[DataContract]
	public class Song
	{
		[DataMember]
		public int id { get; set; }
		[DataMember]
		public string artist { get; set; }
		[DataMember]
		public string title { get; set; }
		[DataMember]
		public string anime { get; set; }
		[DataMember]
		public bool enabled { get; set; }
		[DataMember]
		public string tag { get; set; }
	}
}
