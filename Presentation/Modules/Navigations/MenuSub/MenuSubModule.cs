using Unity;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Aksl.Modules.MenuSub.ViewModels;
using Aksl.Modules.MenuSub.Views;

namespace Aksl.Modules.MenuSub
{
    public class MenuSubModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public MenuSubModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MenuSubHubView>();

            containerRegistry.RegisterForNavigation<IndustryMenuSubHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(MenuSubHubView).ToString(),
                                                () => this._container.Resolve<MenuSubHubViewModel>());

            ViewModelLocationProvider.Register(typeof(IndustryMenuSubHubView).ToString(),
                                                 () => this._container.Resolve<IndustryMenuSubHubViewModel>());
        }
        #endregion
    }
}
