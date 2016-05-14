using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LyticsSitecoreConnector.Data.Interface
{
	public interface ILyticsSegment
	{
		string Name { get; set; }
		string Id { get; set; }
	}
}
