using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Proto.AuthUser;

namespace UserService
{
    public class AuthServiceImpl : AuthUserService.AuthUserServiceBase  
    {
        private readonly ICreateAdminUseCase _createAdminUseCase;
        private readonly ICreateManufacturerUseCase _createManufacturerUseCase;
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly IDeleteAdminUseCase _deleteAdminUseCase;
        private readonly IDeleteManufacturerUseCase _deleteManufacturerUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;

        public AuthServiceImpl(ICreateAdminUseCase createAdminUseCase, 
                               ICreateManufacturerUseCase createManufacturerUseCase, 
                               ICreateUserUseCase createUserUseCase, 
                               IDeleteAdminUseCase deleteAdminUseCase, 
                               IDeleteManufacturerUseCase deleteManufacturerUseCase, 
                               IDeleteUserUseCase deleteUserUseCase)
        {
            _createAdminUseCase = createAdminUseCase;
            _createManufacturerUseCase = createManufacturerUseCase;
            _createUserUseCase = createUserUseCase;
            _deleteAdminUseCase = deleteAdminUseCase;
            _deleteManufacturerUseCase = deleteManufacturerUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }

        public async override Task<CreateAdminResponse> CreateAdmin(CreateAdminRequest request, ServerCallContext context)
        {
            var adminDTO = new AdminDTO()
            {
                Id = Guid.Parse(request.Id),
                Name = request.Name,
                Surname = request.Surname,
            };
            try
            {
                var id = await _createAdminUseCase.Execute(adminDTO, context.CancellationToken);

                return new CreateAdminResponse
                {
                    Success = true,
                    Message = $"Admin created successfully, created id: {id}"
                };
            }
            catch (Exception ex)
            {
                return new CreateAdminResponse
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }
        }

        public async override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var userDTO = new UserDTO()
            {
                Id = Guid.Parse(request.Id),
                Name = request.Name,
                Surname = request.Surname,
                BirthDate = request.BirthDate.ToDateTime()
            };
            try
            {
                var id = await _createUserUseCase.Execute(userDTO, context.CancellationToken);

                return new CreateUserResponse
                {
                    Success = true,
                    Message = $"User created successfully, created id: {id}"
                };
            }
            catch (Exception ex)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }
        }

        public async override Task<CreateManufacturerResponse> CreateManufacturer(CreateManufacturerRequest request, ServerCallContext context)
        {
            var manufacturerDTO = new ManufacturerDTO()
            {
                Id = Guid.Parse(request.Id),
                Organization = request.Organization
            };
            try
            {
                var id = await _createManufacturerUseCase.Execute(manufacturerDTO, context.CancellationToken);

                return new CreateManufacturerResponse
                {
                    Success = true,
                    Message = $"Manufacturer created successfully, created id: {id}"
                };
            }
            catch (Exception ex)
            {
                return new CreateManufacturerResponse
                {
                    Success = false,
                    Message = ex.Message,
                };

                throw;
            }
        }

        public async override Task<DeleteEntityResponse> DeleteEntity(DeleteEntityRequest request, ServerCallContext context)
        {
            try
            {
                switch (request.Role)
                {
                    case "User":
                        await _deleteUserUseCase.Execute(Guid.Parse(request.Id), context.CancellationToken);
                        break;
                    case "Admin":
                        await _deleteAdminUseCase.Execute(Guid.Parse(request.Id), context.CancellationToken);
                        break;
                    case "Manufacturer":
                        await _deleteManufacturerUseCase.Execute(Guid.Parse(request.Id), context.CancellationToken);
                        break;
                    default:
                        throw new EntityNotFoundException(nameof(IdentityRole<Guid>), request.Role);
                }

                return new DeleteEntityResponse
                {
                    Success = true,
                    Message = $"Entity \"{request.Role}\"({request.Id}) deleted successfully"
                };
            }
            catch(Exception ex)
            {
                return new DeleteEntityResponse
                {
                    Success = false,
                    Message = ex.Message,
                };

                throw;
            }
        }
    }
}
