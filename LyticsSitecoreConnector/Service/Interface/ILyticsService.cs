using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LyticsSitecoreConnector.Data.Interface;

namespace LyticsSitecoreConnector.Service.Interface
{
	public interface ILyticsService
	{
		IEnumerable<ILyticsSegment> GetAllSegments();
		HashSet<string> GetCurrentUserSegmentIds();
		void IntegrateLyticsRules();
	}
}
