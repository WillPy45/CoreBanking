using CoreBankingWebApplication.Common;
using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Infraestructure.PasswordHashers;
using CoreBankingWebApplication.Models.Entities;

using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CoreBankingWebApplication.Services
{
    using CreateResult = Result<CreateProfileResponseDto, CreateProfileError>;
    using UpdateResult = Result<string, UpdateProfileError>;
    public class ProfileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        public ProfileService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<CreateProfileResponseDto, CreateProfileError>> CreateAsync(CreateProfileRequestDto request)
        {
            if (await _dbContext.Profiles.AnyAsync(e => e.UserName == request.UserName))
            {
                return CreateResult.Fail(CreateProfileError.UserNameAlreadyExists, "User name already exists.");
            }

            string hashedPassword = _passwordHasher.HashPassword(request.Password);

            Profile profile = new()
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = RoleType.Visitor,
                CreatedAtUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            await _dbContext.Profiles.AddAsync(profile);
            await _dbContext.SaveChangesAsync();

            return CreateResult.Success(new CreateProfileResponseDto()
            {
                Id = profile.Id,
                UserName = profile.UserName,
                Email = profile.Email
            });

        }

        public async Task<UpdateResult> UpdateAsync(UpdateProfileRequestDto requestDto)
        {

            var profilesRecord = await _dbContext.Profiles.Where(e =>
                e.Id == requestDto.Id
                || e.UserName == requestDto.NewName
                || e.Email == requestDto.NewEmail)
                .ToListAsync();

            Profile? profile = profilesRecord.Find(e => e.Id == requestDto.Id);
            bool emailAlreadyExists = profilesRecord.Exists(e => e.Email == requestDto.NewEmail);
            bool userNameAlreadyExists = profilesRecord.Exists(e => e.Email == requestDto.NewName);

            if (profile is null)
            {
                UpdateResult.Fail(UpdateProfileError.ProfileNotFound, "Profile not found");
            }
            if (userNameAlreadyExists)
            {
                return UpdateResult.Fail(UpdateProfileError.UserNameAlreadyExists, "User name is used for other profile");
            }
            if (emailAlreadyExists)
            {
                return UpdateResult.Fail(UpdateProfileError.EmailAlreadyExists, "Email is used for other profile");
            }

            return UpdateResult.Success("Updated");
        }
    }

    public class CreateProfileRequestDto
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class CreateProfileResponseDto
    {
        public required int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public enum CreateProfileError
    {
        UserNameAlreadyExists,
        EmailAlreadyExists
    }

    public class UpdateProfileRequestDto
    {
        /// <summary>
        /// The Id of the Profile to update
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// New name to replace the profile name.
        /// If <c>null</c>, the current name is preserved.
        /// </summary>
        public string? NewName { get; set; }

        /// <summary>
        /// New email to replace the profile email.
        /// If <c>null</c>, the current email is preserved.
        /// </summary>
        public string? NewEmail { get; set; }

        /// <summary>
        /// New password to replace the profile password.
        /// If <c>null</c>, the current password is preserved.
        /// </summary>
        public string? NewPassword { get; set; }
    }

    public class UpdateProfileResponseDto
    {
        /// <summary>
        /// The Id of the Profile to update
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// New name to replace the profile name.
        /// If <c>null</c>, the current name is preserved.
        /// </summary>
        public string? NewName { get; set; }

        /// <summary>
        /// New email to replace the profile email.
        /// If <c>null</c>, the current email is preserved.
        /// </summary>
        public string? NewEmail { get; set; }

        /// <summary>
        /// New password to replace the profile password.
        /// If <c>null</c>, the current password is preserved.
        /// </summary>
        public string? NewPassword { get; set; }
        public DateTime UpdatedTime { get; set; }

    }
    public enum UpdateProfileError
    {
        UserNameAlreadyExists,
        EmailAlreadyExists,
        ProfileNotFound
    }



    public enum FullUpdateProfileError
    {

    }
}
