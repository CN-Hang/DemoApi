using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DemoApi.DbContextLocators;

using DemoApi.Entities;

using Microsoft.EntityFrameworkCore;

namespace DemoApi.DbContexts
{
    public class SimpleDbContext : DbContext
    {
        public SimpleDbContext(DbContextOptions<SimpleDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DynamicLoadDbSet(modelBuilder);
            // 获取所有继承自 IEntityBase 的实体类型
            var entityBaseType = typeof(IEntityBase);
            var entityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && entityBaseType.IsAssignableFrom(t));

            // 为每个实体类型应用 ConfigurableEntityBase 配置
#error the exception was thrown 
            // What I'm trying to accomplish here is to enable the Configure method inherited from the ConfigurableEntityBase class
            // to take effect through reflection so that each entity can be configured individually
            // without having to write very long code in DbContext
            foreach (var entityType in entityTypes)
            {
                var method = typeof(ModelBuilder).GetMethods()
                    .First(m => m.Name == "Entity" && m.IsGenericMethod)
                    .MakeGenericMethod(entityType);

                var entityBuilder = method.Invoke(modelBuilder, null);
                // entityType 继承了TDbcontetxtlocator，所以获取泛型参数
                var dbContextLocatorType = entityType?.BaseType?.GenericTypeArguments.Where(a => a.GetInterface(nameof(IDbContextLocator)) != null).FirstOrDefault();
                if (entityType == null || dbContextLocatorType == null)
                    continue;
                var configType = typeof(ConfigurableEntityBase<,>).MakeGenericType(entityType, dbContextLocatorType);
                var configInstance = Activator.CreateInstance(configType);

                var configureMethod = configType.GetMethod("Configure");
                configureMethod?.Invoke(configInstance, new[] { entityBuilder });
            }

            base.OnModelCreating(modelBuilder);
        }

        private void DynamicLoadDbSet(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.ExportedTypes)
            {
                if (type.IsClass && type != typeof(EntityBase<SystemDbContextLocator>) && typeof(EntityBase<SystemDbContextLocator>).IsAssignableFrom(type))
                {
                    var method = modelBuilder.GetType().GetMethods().Where(x => x.Name == "Entity").FirstOrDefault();

                    if (method != null)
                    {
                        method = method.MakeGenericMethod(new Type[] { type });
                        method.Invoke(modelBuilder, null);
                    }
                }
            }
        }

    }
}
