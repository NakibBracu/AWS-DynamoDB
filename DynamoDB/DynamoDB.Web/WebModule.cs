using Autofac;
using DynamoDB.Web.Models;


namespace DynamoDB.Web
{
    public class WebModule : Autofac.Module
    {
        public WebModule()
        { }

        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterType<ItemCreateModel>().AsSelf()
            .InstancePerLifetimeScope();
            builder.RegisterType<ItemUpdateModel>().AsSelf()
   .InstancePerLifetimeScope();
            builder.RegisterType<ItemListModel>().AsSelf()
  .InstancePerLifetimeScope();


            base.Load(builder);
        }
    }
}
