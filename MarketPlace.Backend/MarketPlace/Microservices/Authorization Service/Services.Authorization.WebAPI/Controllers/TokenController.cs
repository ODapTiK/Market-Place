using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService
{
    [Route("api/token")]
    public class TokenController : BaseController
    {
        private readonly IJwtProvider _jwtProvider;

        public TokenController(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        [HttpPut]
        public async Task<ActionResult<TokenDTO>> RefreshToken([FromBody] TokenDTO tokenDTO)
        {
            var updatedTokenDTO = await _jwtProvider.RefreshToken(tokenDTO);

            return Ok(updatedTokenDTO); 
        }
    }
}
