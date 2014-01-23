using System;
using System.Web.Mvc;
using Business;
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

            kernel.Bind<ITagService>()
                  .To<TagService>();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return (controllerType == null) ? null : (IController)kernel.Get(controllerType);
        }
    }
}