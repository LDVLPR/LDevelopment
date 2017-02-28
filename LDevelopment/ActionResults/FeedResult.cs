using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace LDevelopment.ActionResults
{
    public class FeedResult : ActionResult
    {
        private SyndicationFeed Feed { get; }

        public FeedResult(SyndicationFeed feed)
        {
            Feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.ContentType = "application/rss+xml";

            if (Feed != null)
            {
                var formatter = new Rss20FeedFormatter(Feed);

                using (var writer = new XmlTextWriter(response.Output))
                {
                    writer.Formatting = Formatting.Indented;

                    formatter.WriteTo(writer);
                }
            }
        }
    }
}