using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMedicalSystem.Core.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string username, string password);
    }
}
