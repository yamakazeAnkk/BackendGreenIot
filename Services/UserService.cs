using GreenIotApi.Models;
using GreenIotApi.Repositories;
using System.Threading.Tasks;

namespace GreenIotApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        // Inject UserRepository vào Service
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Thêm người dùng mới
        public async Task<User> AddUserAsync(User user)
        {
            return await _userRepository.AddUserAsync(user);
        }
        public async Task<User> GetUserByIdAsync(string documentId)
        {
            return await _userRepository.GetUserByIdAsync(documentId);
        }
    }
}
