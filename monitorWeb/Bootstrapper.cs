using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.TinyIoc;

namespace monitorWeb
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        public static int Port { get; set; }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Resource", "/Resource"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Viewer", "/Viewer"));
            nancyConventions.StaticContentsConventions.AddFile("/", @"/index.html");
            this.Conventions.StaticContentsConventions.AddFile("/favicon.ico", @"/Resource/favicon.ico");
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            CookieBasedSessions.Enable(pipelines);
            //指定视图所在目录
            this.Conventions.ViewLocationConventions.Add((viewName, model, context) =>
            {
                return string.Concat("Viewer/", viewName);
            });
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
        }

    }
}