using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Authentication.Forms;
using System.IO;
using System.Data.SQLite;

namespace Minu
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ConfigureApplicationContainer(container);
            //SQLiteHelper DBHelper;
            // Check if database file exists
            //DBHelper = new SQLiteHelper(@"C:\Users\James\Documents\Visual Studio 2015\Projects\Minu\Minu\bin\BlogDB.sqlite");
            container.Register<SQLiteHelper>(new SQLiteHelper(@"C:\Users\James\Documents\Visual Studio 2015\Projects\Minu\Minu\bin\BlogDB.sqlite"));
        }
    }
}