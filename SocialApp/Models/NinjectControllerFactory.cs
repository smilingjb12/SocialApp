using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using Ninject;
using Ninject.Web.Common;

namespace SocialApp.Models
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public NinjectControllerFactory()
        {
            this.kernel = new StandardKernel();
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            kernel.Bind<SocialAppContext>()
                  .To<SocialAppContext>()
                  .InRequestScope();

            kernel.Bind<EmailSender>()
                  .To<EmailSender>();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return (controllerType == null) ? null : (IController)kernel.Get(controllerType);
        }
    }
}