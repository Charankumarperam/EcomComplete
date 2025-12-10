using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
namespace Interfaces.IManagers
{
    public interface IAuthManager
    {
        Task<Result> Register(Register dto);
        Task<Result<AuthResponse>> Login(Login dto);
    }
}
