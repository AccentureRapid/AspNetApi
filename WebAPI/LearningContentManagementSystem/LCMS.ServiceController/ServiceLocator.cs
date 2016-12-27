using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LCMS.Common;
using LCMS.Common.Logging;
using LCMS.AnnouncementRepository;
using LCMS.Common.Data;
using LCMS.AnnouncementService;
using LCMS.SessionRepository;
using LCMS.SessionService;

namespace LCMS.ServiceController
{
    public class ServiceLocator
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        public static void RegisterTypes(IUnityContainer container)
        {
            // Register your types here
            var assemblies = new Assembly[]
            {
                typeof(IConfigGateway).Assembly,
                typeof(IAnnouncementRepository).Assembly,
                typeof(IAnnouncementService).Assembly,
                typeof(ISessionRepository).Assembly,
                typeof(ISessionService).Assembly
            };

            Func<Type, LifetimeManager> getLifetimeManager = type =>
            {
                return new PerRequestLifetimeManager();
            };

            container.RegisterTypes(AllClasses.FromAssemblies(assemblies),
                                    WithMappings.FromMatchingInterface,
                                    WithName.Default,
                                    getLifetimeManager);

            container.RegisterType<IDatabase, Database>(
                        getLifetimeManager(typeof(Database)),
                        new InjectionConstructor("LCMSDB"));

            TraceRegistrations(container);
        }

        public static T Resolve<T>()
        {
            return container.Value.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return container.Value.Resolve<T>(name);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return container.Value.ResolveAll<T>();
        }

        internal static void TraceRegistrations(IUnityContainer container)
        {
#if DEBUG
            foreach (var registration in container.Registrations.OrderBy(x => x.RegisteredType.FullName))
            {
                RapidLogger.Trace("Unity Registrations",
                    string.Format("{0} => {1} using {2}",
                        registration.RegisteredType.FullName,
                        registration.MappedToType.FullName,
                        registration.LifetimeManagerType.Name
                    )
                );
            }
#endif
        }
    }

}
