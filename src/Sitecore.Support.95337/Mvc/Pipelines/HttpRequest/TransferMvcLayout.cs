namespace Sitecore.Support.Mvc.Pipelines.HttpRequest
{
  using Sitecore.Diagnostics;
  using Sitecore.Mvc.Configuration;
  using Sitecore.Pipelines.HttpRequest;
  using Sitecore.Security.Authentication;
  using System;
  using System.IO;
  using System.Web;

  [UsedImplicitly]
	public class TransferMvcLayout : HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			Assert.ArgumentNotNull(args, "args");
			string filePath = Context.Page.FilePath;
			if (!string.IsNullOrWhiteSpace(filePath))
			{
				string extension;
				try
				{
					extension = Path.GetExtension(filePath);
				}
				catch (ArgumentException)
				{
					extension = string.Empty;
				}
				if (MvcSettings.IsViewExtension(extension))
				{
					HttpContext current = HttpContext.Current;
					if (!Context.PageMode.IsNormal && AuthenticationHelper.IsAuthenticationTicketExpired())
					{
						current.Response.Redirect("/sitecore/login");
					}
					Tracer.Info("MVC Layout detected - transfering to ASP.NET MVC");
					args.Context.Items["sc::IsContentUrl"] = "true";
					args.AbortPipeline();
				}
			}
		}
	}
}
