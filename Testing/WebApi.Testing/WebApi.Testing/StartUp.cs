using System.Web.Http;
using Owin;
using Swashbuckle.Application;

namespace WebApi.Testing
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "WebApi.Testing");
                c.PrettyPrint();
            }).EnableSwaggerUi(c => { });

            //other config related stuff... like cors, messageHandler, formatters etc
            app.UseWebApi(config);
        }
    }
}