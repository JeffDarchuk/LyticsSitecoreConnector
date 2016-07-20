# Lytics Connector For Sitecore
This is a module that connects to lytics to provide a hook for real time lytics segment personalization using the rules engine in sitecore.
### How does it work?
Set up your site to track user activities using Lytics.  Either using default segments or define custom segments in Lytics.  The connector will automatically pull all active Lytics segments to be available for sitecore personalization.
### Getting started
1.  The sitecore site needs to be set up with Lytics tracking.
2.  Install the nuget package for this [defined here](https://www.nuget.org/packages/LyticsSitecoreConnector/)
3.  The nuget package will deliver a Lytics.config file to the project at app_config/include/Lytics.config
4.  Modify this config file with your Lytics key and define how to acquire the Lytics id
  1.  Usually found in a users cookies.
  2.  Cusomizable through adding a custom pipeline processor to expose the user's Lytics id
5.  On first load the connector will detect that it needs to install itself and will place the neccisary sitecore items to facilitate the personalization.  This includes a node for segments, the rules conditions, and editing the conditional rendering rules definition to add Lytics rules.
6.  Personalize your content using sitecore.
