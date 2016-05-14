using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore;
using Sitecore.Pipelines.HttpRequest;

namespace LyticsSitecoreConnector.Pipeline.HttpRequest
{
	public class GatherLyticsUserSegments : HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			LyticsContext.Service.LoadUserSegments();
		}
	}
}
