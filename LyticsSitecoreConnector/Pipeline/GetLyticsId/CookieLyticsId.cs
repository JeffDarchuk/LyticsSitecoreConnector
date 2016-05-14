using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LyticsSitecoreConnector.Pipeline.GetLyticsId
{
	public class CookieLyticsId
	{
		private string cookie;
		public CookieLyticsId(string cookie)
		{
			this.cookie = cookie;
		}
		public void Process(LyticsPipelineArgs args)
		{
			var httpCookie = HttpContext.Current.Request.Cookies[cookie];
			if (httpCookie != null)
				args.LyticsId = httpCookie.Value;
		}
	}
}
