using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Unity;

using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;
using Aksl.Toolkit.Services;

namespace Aksl.Modules.Shell.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IDialogViewService _dialogViewService;
        private object _currentView;
        //private object _activeView = default;
        #endregion

        #region Constructors
        public ShellViewModel(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogViewService dialogViewService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _dialogViewService = dialogViewService;

            RegisterContentChangedEvents(); 
        }
        #endregion

        #region Properties
        private bool _isPaneOpen =true;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (SetProperty<bool>(ref _isPaneOpen, value))
                {
                    _eventAggregator.GetEvent<OnHamburgerMenuBarPaneOpenEvent>().Publish(new() {  IsPaneOpen  = _isPaneOpen });
                }
            }
        }
        #endregion

        #region Register ContentChanged Event
        private void RegisterContentChangedEvents()
        {
            _eventAggregator.GetEvent<OnContentChangedViewEvent>().Subscribe(async (ccve) =>
            {
                try
                {
                    string viewTypeAssemblyQualifiedName = ccve.CurrentMenuItem.ViewName;
                    Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    // var view = _container.Resolve(viewType);
                    if (viewType is not null)
                    {
                        IRegion region = _regionManager.Regions[RegionNames.ShellContentRegion];
                        var viewName = viewType.Name;

                        _currentView = region.Views.FirstOrDefault(v => v.GetType() == viewType);
                        if (_currentView is null)
                        {
                            _currentView = region.GetView(viewType.FullName);
                        }

                        if (_currentView is not null)
                        {
                            if (ccve.CurrentMenuItem.IsCacheable)
                            {
                                region.Activate(_currentView);
                            }
                            else
                            {
                                region.Remove(_currentView);

                                AddView();
                            }
                        }
                        else
                        {
                            AddView();
                        }

                        void AddView()
                        {
                            if (IsSelectedOnInitialize())
                            {
                                _regionManager.RequestNavigate(RegionNames.ShellContentRegion, viewName);
                            }
                            else if (CanAddView())
                            {
                                NavigationParameters navigationParameters = new()
                                {
                                    { "CurrentMenuItem", ccve.CurrentMenuItem }
                                };

                                _regionManager.RequestNavigate(RegionNames.ShellContentRegion, viewName, navigationParameters);
                            }
                            else
                            {
                                NavigationParameters navigationParameters = new()
                                {
                                    { "CurrentMenuItem", ccve.CurrentMenuItem }
                                };

                                _regionManager.RequestNavigate(RegionNames.ShellContentRegion, viewName, navigationParameters);
                            }
                        }

                        bool IsSelectedOnInitialize() => !string.IsNullOrEmpty(ccve.CurrentMenuItem.ModuleName) && ccve.CurrentMenuItem.IsSelectedOnInitialize;

                        bool CanAddView() => !string.IsNullOrEmpty(ccve.CurrentMenuItem.ModuleName) && ccve.CurrentMenuItem.SubMenus.Count == 0;
                    }
                    else
                    {
                       await _dialogViewService.AlertAsync(message: $"Unable to find \"{viewTypeAssemblyQualifiedName}\".", title: $"Error:Missing Type");
                    }
                }
                catch (Exception ex)
                {
                    await _dialogViewService.AlertAsync(message: $"Unable to loading \"{ccve.CurrentMenuItem.ModuleName}\" module.: \"{ex.Message}\"", title: "Error: Load Module");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion
    }
}
