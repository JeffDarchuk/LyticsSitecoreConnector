using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyticsSitecoreConnector.Data.Interface;

namespace LyticsSitecoreConnector.Data.Implementation
{
	public class SimpleLyticsSegment : ILyticsSegment
	{
		public string Name { get; set; }
		public string Id { get; set; }
	}
}
