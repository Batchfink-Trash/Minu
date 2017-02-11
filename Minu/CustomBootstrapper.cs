using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Authentication.Forms;
using System.IO;
using MongoDB.Driver;

namespace Minu
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        MongoHelper DBHelper = new MongoHelper("blog");
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ConfigureApplicationContainer(container);
            //Register SQLiteHelper when the application begins
            container.Register<MongoHelper>(DBHelper);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // We don't call "base" here to prevent auto-discovery of
            // types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // Here we register our user mapper as a per-request singleton.
            // As this is now per-request we could inject a request scoped
            // database "context" or other request scoped services.
            //Give UserDatabase the application wide SQLiteHelper class
            UserDatabase UD = new UserDatabase(DBHelper);
            container.Register<IUserMapper, UserDatabase>();
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            // At request startup we modify the request pipelines to
            // include forms authentication - passing in our now request
            // scoped user name mapper.
            //
            // The pipelines passed in here are specific to this request,
            // so we can add/remove/update items in them as we please.
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/login",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}