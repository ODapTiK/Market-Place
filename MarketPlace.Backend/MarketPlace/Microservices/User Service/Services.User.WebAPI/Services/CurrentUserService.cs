﻿using System.Security.Claims;

namespace UserService
{
    public class CurrentUserService : ICurrentUserService
    {

        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor contextAccessor) => _contextAccessor = contextAccessor; 

        public Guid UserId { get
            {
                var id = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);
            }
        }    
    }
}
