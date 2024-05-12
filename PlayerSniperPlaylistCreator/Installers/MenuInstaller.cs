using Zenject;

namespace PlayerSniperPlaylistCreator.Installers
{
    internal class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ViewControllers.GameplaySetupViewController>().FromNewComponentAsViewController().AsSingle();
        }
    }
}