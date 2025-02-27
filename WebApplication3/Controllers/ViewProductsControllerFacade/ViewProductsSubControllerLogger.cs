using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace WebApplication3.Controllers.ViewProductsControllerFacade
{
    public class ViewProductsSubControllerLogger
    {
            public void PrintRoutes()
            {
                Trace.WriteLine($@"{GetType().Name}
                Routes:
                /posts
                :slug/listpost
                :slug.html/viewonepost
            ");
            }
    }
}