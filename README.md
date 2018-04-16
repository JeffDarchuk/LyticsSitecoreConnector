# Lytics Connector for Sitecore
This is a module that connects to Lytics to provide a hook for real time Lytics segment personalization using the rules engine in Sitecore.
### How does it work?
Set up your site to track user activities with Lytics as you would normally. The connector, once active, will automatically pull all active Lytics segments to be available for Sitecore personalization.
### Getting started

Important: Before starting, be sure the Sitecore site has Lytics tracking up and running.

1.  Install the nuget package [defined here](https://www.nuget.org/packages/LyticsSitecoreConnector/)
2.  The nuget package will deliver a lytics.config file to the project at app_config/include/lytics.config
3.  Modify this config file with your Lytics key and define how to acquire the Lytics ID
    1.  Usually found in a user's cookies
    2.  Customizable through adding a custom pipeline processor to expose the user's Lytics ID
4.  On first load the connector will detect that it needs to install itself and will place the necessary Sitecore items to facilitate the personalization. This includes a node for segments, the rules conditions, and editing the conditional rendering rules definition to add Lytics rules.
5.  Personalize your content using Sitecore, as you would using other available rules un the rules set editor
