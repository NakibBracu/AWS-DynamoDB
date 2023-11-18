using Autofac;
using DynamoDB.Application.Services;
using DynamoDB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDB.Infrastructure
{
    public class InfrastructureModule : Module
    {
        public InfrastructureModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DynamoDBService>().As<IDynamoDBService>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
