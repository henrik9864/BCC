using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCC.Data
{
	class Character
	{
		[Name("Name")]
		public string Name { get; set; }

		[Optional]
		[Name("Main", "Forum Name")]
		public string Main { get; set; }

		[Optional]
		[Name("Corporation")]
		public string Corp { get; set; }

		[Ignore]
		public bool Seat { get; set; } = true;

		[Ignore]
		public bool Discord { get; set; } = true;
	}
}
