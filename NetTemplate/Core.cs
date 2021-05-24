using Ju.Services;

namespace NetTemplate
{
	public static class Core
	{
		// Methods to get services from the service container

		public static T Get<T>() => ServiceContainer.Get<T>();
		public static T Get<T>(string id) => ServiceContainer.Get<T>(id);

		// Core services

		public static IEventBusService Event => ServiceContainer.Get<IEventBusService>();
		public static ITaskService Task => ServiceContainer.Get<ITaskService>();
		public static ICoroutineService Coroutine => ServiceContainer.Get<ICoroutineService>();
		public static ICacheService Cache => ServiceContainer.Get<ICacheService>();

		// ... add more shorthands as you need
	}
}
