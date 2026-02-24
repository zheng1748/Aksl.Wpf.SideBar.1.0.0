using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Events;

using Aksl.Toolkit.Dialogs;
using Aksl.Toolkit.Services;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

using Aksl.Modules.Shell.ViewModels;
using Aksl.Modules.Shell.Views;
using Aksl.Modules.Account;

using Aksl.Modules.HamburgerMenuSideBar;
using Aksl.Modules.HamburgerMenuNavigationSideBar;
using Aksl.Modules.HamburgerMenuTreeSideBar;

using Aksl.Modules.ExpandHamburgerMenu;

using Aksl.Modules.ExpandHamburgerMenuTab;
using Aksl.Modules.MenuSub;

using Aksl.Modules.Home;
using Aksl.Modules.AirCompresser;
using Aksl.Modules.CoolingTower;
using Aksl.Modules.Others;
using Aksl.Modules.Pipeline;
using Aksl.Modules.Thermometer;
using Aksl.Modules.TabBar;
using Aksl.Modules.ExpandHamburgerMenuNavigationBar;
using Aksl.Modules.ExpandHamburgerMenuTreeBar;
using Aksl.Modules.HamburgerMenuPopupSideBar;

namespace Aksl.Modules.Shell
{
    public partial class App
    {
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register(typeof(ShellView).ToString(), () => Container.Resolve<ShellViewModel>());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            #region Initialize
            var services = new ServiceCollection();
            services.AddOptions();

            string basePath = Directory.GetCurrentDirectory();
            string configPath = Path.Combine(basePath, "Configuration");
            string appSettingsPath = Path.Combine(configPath, "appsettings.json");
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(basePath)
                                                                                   .AddJsonFile(path: appSettingsPath, optional: true, reloadOnChange: false);

            var configuration = configurationBuilder.Build();
            #endregion

            #region Logging
            services.AddLogging(builder =>
            {
                var loggingSection = configuration.GetSection("Logging");
                var includeScopes = loggingSection.GetValue<bool>("IncludeScopes");

                builder.AddConfiguration(loggingSection);

                //加入一个ConsoleLoggerProvider
                //builder.AddConsole(consoleLoggerOptions =>
                //{
                //    consoleLoggerOptions.IncludeScopes = includeScopes;
                //});

                //加入一个DebugLoggerProvider
                builder.AddDebug();
            });
            #endregion

            #region WorkspaceViewEventNameAttribute 
            // var types = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes<WorkspaceViewEventNameAttribute>().Any());
            //var workspaceViewEventName = typeof(EventBase).GetCustomAttributeValue<WorkspaceViewEventNameAttribute, string>(w => w.WorkspaceViewEventName, "OnBuildHamburgerMenuWorkspaceViewEvent");
            #endregion

            var serviceProvider = services.BuildServiceProvider();

            containerRegistry.RegisterInstance<IServiceProvider>(serviceProvider);

            containerRegistry.RegisterSingleton(typeof(IDialogService), typeof(DialogService));
            containerRegistry.RegisterSingleton(typeof(IDialogViewService), typeof(DialogViewService));

            containerRegistry.RegisterDialog<ConfirmView, ConfirmViewModel>();

            RegisterMenuFactoryAsync(containerRegistry).GetAwaiter().GetResult();
           

            RegisterBuildWorkspaceViewEventAsync();
        }

        protected async Task RegisterMenuFactoryAsync(IContainerRegistry containerRegistry)
        {
            try
            {
                MenuService menuService = new (new List<string> {"pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/AllMenus.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/Industry.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/Pipelines.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/Thermometers.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/CoolingTowers.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/AirCompressers.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.SideBar;Component/Data/Others.xml",
                                                                 });

                await menuService.CreateMenusAsync();

                containerRegistry.RegisterInstance<IMenuService>(menuService);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        protected Task RegisterBuildWorkspaceViewEventAsync()
        {
            try
            {
                var eventAggregator = Container.Resolve<IEventAggregator>();

                //SideBar
                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuSideBarWorkspaceViewEvent>();
                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent>();
                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent>();

                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuPopupSideBarWorkspaceViewEvent>();

                _ = eventAggregator.GetEvent<OnBuildIndustryMenuSubWorkspaceViewEvent>();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return Task.CompletedTask;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            _ = moduleCatalog.AddModule(nameof(AccountModule), typeof(AccountModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(typeof(ShellModule).Name, typeof(ShellModule).AssemblyQualifiedName, InitializationMode.WhenAvailable,
                                        dependsOn: [typeof(HamburgerMenuSideBarModule).Name,typeof(HamburgerMenuNavigationSideBarModule).Name, typeof(HamburgerMenuTreeSideBarViewModule).Name, typeof(HamburgerMenuPopupSideBarModule).Name]);

            _ = moduleCatalog.AddModule(typeof(HamburgerMenuSideBarModule).Name, typeof(HamburgerMenuSideBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(typeof(HamburgerMenuNavigationSideBarModule).Name, typeof(HamburgerMenuNavigationSideBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(typeof(HamburgerMenuTreeSideBarViewModule).Name, typeof(HamburgerMenuTreeSideBarViewModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
        
            _ = moduleCatalog.AddModule(nameof(HamburgerMenuPopupSideBarModule), typeof(HamburgerMenuPopupSideBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(ExpandHamburgerMenuModule), typeof(ExpandHamburgerMenuModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(ExpandHamburgerMenuNavigationBarModule), typeof(ExpandHamburgerMenuNavigationBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(ExpandHamburgerMenuTreeBarModule), typeof(ExpandHamburgerMenuTreeBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(ExpandHamburgerMenuTabModule), typeof(ExpandHamburgerMenuTabModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(MenuSubModule), typeof(MenuSubModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(TabBarModule), typeof(TabBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(HomeModule), typeof(HomeModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(PipelineModule), typeof(PipelineModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(ThermometerModule), typeof(ThermometerModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(CoolingTowerModule), typeof(CoolingTowerModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(AirCompresserModule), typeof(AirCompresserModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(OthersModule), typeof(OthersModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override  void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
