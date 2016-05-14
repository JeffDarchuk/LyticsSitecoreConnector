using System;
using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyticsSitecoreConnector.Service.Interface;

namespace LyticsSitecoreConnector
{
	public static class LyticsContext
	{
		public static ILyticsService Service { get; internal set; }
		public static string AccessKey { get; internal set; }
		public static string RootAddress { get; internal set; }
	}
}
