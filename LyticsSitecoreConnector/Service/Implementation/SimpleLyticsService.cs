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
		public IEnumerable<string> GetCurrentUserSegmentIds(string id)
		{
			WebClient wc = new WebClient();
			dynamic data =
				JsonConvert.DeserializeObject<ExpandoObject>(
					wc.DownloadString(string.Format("{0}/api/entity/user/_uids/{1}?access_token={2}", LyticsContext.RootAddress, id, LyticsContext.AccessKey)));
			if (data != null && data.data != null && data.data.segments_all != null)
				foreach (string segment in data.data.segments_all)
				{
					yield return segment;
				}
		}

		public IEnumerable<ILyticsSegment> GetAllSegments()
		{
			WebClient wc = new WebClient();
			dynamic data =
				JsonConvert.DeserializeObject<ExpandoObject>(
					wc.DownloadString(LyticsContext.RootAddress + "/api/segment?access_token=" + LyticsContext.AccessKey));
			if (data != null && data.data != null)
				foreach (dynamic segment in data.data)
				{
					if (!segment.invalid)
						yield return new SimpleLyticsSegment()
						{
							Id = segment.id,
							Name = segment.name
						};
				}
		}

		public HashSet<string> GetCurrentUserSegmentIds()
		{
			for (int i = 0; i < 10 && HttpContext.Current.Items["segments"] == null; i++)
			{
				Thread.Sleep(10);
			}
			var segments = HttpContext.Current.Items["segments"] as HashSet<string>;
			if (segments != null) return segments;
			return new HashSet<string>();
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
							using (new EditContext(item))
							{
								item["Segment Id"] = segment.Id;
								item["Segment Name"] = segment.Name;
								item.Appearance.ReadOnly = true;
							}
						}
						foreach (string key in populatedSegments.Keys)
							db.GetItem(populatedSegments[key]).Delete();
					}
				}
			}
		}

		public void LoadUserSegments()
		{
			var args = new LyticsPipelineArgs();
			var pipeline = CorePipelineFactory.GetPipeline("getLyticsId", string.Empty);
			pipeline.Run(args);
			ProcessUser(args.LyticsId);
		}

		private void ProcessUser(string userId)
		{
			if (!string.IsNullOrWhiteSpace(userId))
			{
				Task.Run(() =>
				{
					HashSet<string> segments = new HashSet<string>();
					WebClient wc = new WebClient();
					dynamic data =
						JsonConvert.DeserializeObject<ExpandoObject>(
							wc.DownloadString(
								$"{LyticsContext.RootAddress}/api/entity/user/_uids/{userId}?access_token={LyticsContext.AccessKey}&fields=none"));
					if (data != null && data.data != null && data.data.segments_all != null)
						foreach (string segment in data.data.segments_all)
						{
							segments.Add(segment);
						}
					HttpContext.Current.Items["segments"] = segments;
				});
			}
		}
	}
}
