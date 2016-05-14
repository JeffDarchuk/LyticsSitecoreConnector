using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines;

namespace LyticsSitecoreConnector.Pipeline
{
	public class LyticsPipelineArgs : PipelineArgs
	{
		public string LyticsId;
		public string LyticsField;
	}
}
