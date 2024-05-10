using PlaylistManager;
using Zenject;

namespace PlayerSniperPlaylistCreator.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ViewControllers.GameplaySetupViewController>().AsSingle();
            Container.Bind<PlaylistDataManager>().FromResolve();
        }
    }
}
