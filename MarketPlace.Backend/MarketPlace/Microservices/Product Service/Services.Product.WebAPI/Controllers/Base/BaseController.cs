﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProductService
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseController : ControllerBase
    {
        private IMediator? _Mediator;
        protected IMediator Mediator => _Mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        public Guid UserId => !User.Identity.IsAuthenticated ? Guid.Empty : Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
