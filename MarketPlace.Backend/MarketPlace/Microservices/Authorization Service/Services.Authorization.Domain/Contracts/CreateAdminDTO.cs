﻿namespace AuthorizationService
{
    public class CreateAdminDTO : UserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
    }
}
