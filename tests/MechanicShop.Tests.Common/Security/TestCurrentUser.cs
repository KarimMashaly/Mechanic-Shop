using MechanciShop.Infrustructure.Identity;
using MechanicShop.Application.Common.Interfaces;

namespace MechanicShop.Tests.Common.Security
{
    public class TestCurrentUser : IUser
    {
        private AppUser? _currentUser;

        public void Returns(AppUser currentUser)
        {
            _currentUser = currentUser;
        }

        public string? Id => _currentUser!.Id ?? UserFactory.CreateUser().Id;
    }
}
