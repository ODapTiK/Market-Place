﻿using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IJwtProvider _jwtProvider;

        public TokenController(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        [HttpPut("Refresh")]
        public async Task<ActionResult<TokenDTO>> RefreshToken([FromBody] TokenDTO tokenDTO)
        {
            var updatedTokenDTO = await _jwtProvider.RefreshToken(tokenDTO);

            return Ok(updatedTokenDTO); 
        }
    }
}
