using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LyticsSitecoreConnector.Data.Implementation;
using LyticsSitecoreConnector.Data.Interface;
using LyticsSitecoreConnector.Pipeline;
using LyticsSitecoreConnector.Service.Interface;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;

namespace LyticsSitecoreConnector.Service.Implementation
{
	public class SimpleLyticsService : ILyticsService
	{
		private Dictionary<string, HashSet<string>> _segmentDef = new Dictionary<string, HashSet<string>>();
		public IEnumerable<ILyticsSegment> GetAllSegments()
		{
			List<ILyticsSegment> ret = new List<ILyticsSegment>();
			WebClient wc = new WebClient();
			dynamic data =
				JsonConvert.DeserializeObject<ExpandoObject>(
					wc.DownloadString(string.Format("{0}/api/segment?access_token={1}", LyticsContext.RootAddress, LyticsContext.AccessKey)));
			if (data != null && data.data != null)
				foreach (dynamic segment in data.data)
				{
					try
					{
						if (segment.kind == "segment")
							ret.Add(new SimpleLyticsSegment()
							{
								Id = segment.id,
								Name = segment.name,
								SlugName = segment.slug_name
							});
					}
					catch (RuntimeBinderException ex)
					{
						//means it's not a segment and we don't want it.
					}
				}
			return ret;
		}

		public HashSet<string> GetCurrentUserSegmentIds()
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies["ly_segs"];
			HashSet<string> ret = new HashSet<string>();
			if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
			{
				ret.UnionWith(JsonConvert.DeserializeObject<Dictionary<string, string>>(HttpUtility.UrlDecode(cookie.Value)).Keys);
			}
			return ret;
		}

		public void IntegrateLyticsRules()
		{
			Database db = Factory.GetDatabase("master", false);
			if (db != null)
			{
				Dictionary<string, ID> populatedSegments = new Dictionary<string, ID>();
				Item lyticsSegmentFolder = db.GetItem(Constants.SegmentRuleFolder);
				if (lyticsSegmentFolder != null)
				{
					using (new SecurityDisabler())
					{
						foreach (Item scSegment in lyticsSegmentFolder.Children)
							if (!populatedSegments.ContainsKey(scSegment["Segment Id"]))
								populatedSegments.Add(scSegment["Segment Id"], scSegment.ID);
						foreach (ILyticsSegment segment in GetAllSegments().Where(x => !string.IsNullOrWhiteSpace(x.Name)))
						{
							Item item;
							if (populatedSegments.ContainsKey(segment.Id))
							{
								item = db.GetItem(populatedSegments[segment.Id]);
								populatedSegments.Remove(segment.Id);
							}
							else
							{
								string name = ItemUtil.ProposeValidItemName(segment.Name, "Unknown Segment");
								item = lyticsSegmentFolder.Add(name, new TemplateID(new ID(Constants.SegmentTemplateId)));
							}
							if (item != null)
								using (new EditContext(item))
								{
									item["Segment Id"] = segment.Id;
									item["Segment Name"] = segment.SlugName;
									item.Appearance.ReadOnly = true;
								}
						}
						foreach (string key in populatedSegments.Keys)
							db.GetItem(populatedSegments[key]).Delete();
					}
				}
			}
		}
	}
}
