namespace LyticsSitecoreConnector.Agent
{
	public class RulesRefreshAgent
	{
		public void run()
		{
			LyticsContext.Service.IntegrateLyticsRules();
		}
	}
}
