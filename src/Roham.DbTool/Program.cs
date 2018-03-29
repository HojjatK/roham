using System;
using System.Linq;
using Autofac;
using Roham.Lib.Logger;
using IOC = Roham.Lib.Ioc;
using Roham.Ioc.Autofac;
using Roham.Data;
using Roham.DbTool.IocModules;

namespace Roham.DbTool
{
    public class Program
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<Program>();

        public static int Main(string[] args)
        {
            string errMessage;
            if (!TryConfigureLogger(args, out errMessage))
            {
                ConsoleLog.WriteError($"Logger cannot be initialized: {errMessage}");
                return 1;
            }
            try
            {
                Log.Debug("++ DbUp entry point");

                int result = Run(args);
                if (result == 0)
                {
                    ConsoleLog.WriteSuccess("Succeed!");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error("DbUp failed", ex);
                ConsoleLog.WriteError($"Failed\r\n{ex.Message}");
                return 1;
            }
            finally
            {
                Log.Debug("-- DbUp exit point");
            }
        }

        private static int Run(string[] args)
        {
            DbOptions options;
            if (!DbOptions.TryParse(args, out options))
            {
                Log.Debug($"Arguments {args} are not valid");
                return 1;
            }

            if (!options.ShowHelp)
            {
                Log.Debug($"Arguments {args} are valid");

                if (!options.IsForce)
                {
                    ConsoleLog.WriteInfo(options.ToString());
                    ConsoleLog.Write(ConsoleColor.White, "Would you like to continue with the above parameters ");
                    ConsoleLog.Write(ConsoleColor.Yellow, " [Y/N]? ");
                    var c = Console.ReadLine();
                    Console.WriteLine();
                    if (!"Y".Equals(c, StringComparison.OrdinalIgnoreCase))
                    {
                        ConsoleLog.WriteWarn("DbUp exited as you didn't confirm to continue");
                        Log.Warn("DbUp didn't run, since user didn't confirm to continue");

                        return 1;
                    }
                }
                return ResolveDbUpRunner(options.ShowSql).Run(options);
            }
            else
            {
                return 1;
            }
        }

        private static bool TryConfigureLogger(string[] args, out string errMessage)
        {
            errMessage = "";
            try
            {
                if (LoggerFactory.Configure())
                {
                    // Change log level to debug if set in arguments
                    if (args.Any(a => a.Trim().ToUpper() == "--DEBUG"))
                    {
                        LoggerFactory.ChangeLogThresholdToDebug();
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                errMessage = ex.Message;
                return false;
            }
        }

        private static IDbToolRunner ResolveDbUpRunner(bool showSql)
        {
            var rootScope = InitializeDependencyResolver(showSql);
            return rootScope.Resolve<IDbToolRunner>();
        }

        private static IOC.ILifetimeScope InitializeDependencyResolver(bool showSql)
        {
            var container = BuildAutofacContainer(showSql);
            var rootScope = container.Resolve<IOC.ILifetimeScope>();
            DbToolDependencyResolver.Initialize(rootScope);

            return rootScope;
        }


        private static IContainer BuildAutofacContainer(bool showSql)
        {
            var builder = new ContainerBuilder();
            var autoRegistration = new IOC.AutoRegistration(AutofacIocFactory.CreateRegistrator(builder));
            autoRegistration
                        .IncludeAssembilesFromTypes(new Type[]
                        {
                            typeof(IOC.IResolver),
                            typeof(AdhocSqlRunner),
                            typeof(Lib.Domain.Persistence.PersistenceUnitOfWorkFactory),
                            typeof(AutofacIocFactory),
                            typeof(Persistence.NHibernate.Conventions.PropertyConvention),
                            typeof(Data.DbUp.UpgradeEngineBuilder),
                            typeof(DbToolRunner)
                        })
                        .IncludeImplementsITypeNameConvention()
                        .IncludeClosingTypeConvention()
                        .ApplyRegistrations();
            builder.RegisterModule(new ConfigModule(DbProviders.SqlServer, showSql));
            builder.RegisterModule(new PersistenceModule());
            builder
                .RegisterType(AutofacIocFactory.GetLifetimeScopeType())
                .As<IOC.IResolver>()
                .As<IOC.ILifetimeScope>()
                .InstancePerLifetimeScope();
            var container = builder.Build();
            return container;
        }

    }
}
