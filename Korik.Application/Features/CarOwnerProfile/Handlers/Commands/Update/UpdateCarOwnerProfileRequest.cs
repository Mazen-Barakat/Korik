using AutoMapper;
using FluentValidation;
using Korik.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record UpdateCarOwnerProfileRequest(UpdateCarOwnerProfileDTO Model) : IRequest<ServiceResult<CarOwnerProfileDTO>> { }

    public class UpdateCarOwnerProfileRequestHandler : IRequestHandler<UpdateCarOwnerProfileRequest, ServiceResult<CarOwnerProfileDTO>>
    {
        #region Dependency Injection

        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IValidator<UpdateCarOwnerProfileDTO> _validator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public UpdateCarOwnerProfileRequestHandler
            (
            ICarOwnerProfileService carOwnerProfileService,
            IValidator<UpdateCarOwnerProfileDTO> validator,
            IMapper mapper,
            IFileStorageService fileStorageService
            )
        {
            _carOwnerProfileService = carOwnerProfileService;
            _validator = validator;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<CarOwnerProfileDTO>> Handle(UpdateCarOwnerProfileRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(s => s.ErrorMessage));
                return ServiceResult<CarOwnerProfileDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            #region Mapping

            // Fetch the existing CarOwnerProfile to preserve relationships

            var existingCarOwnerProfile = await _carOwnerProfileService.GetByIdAsync(request.Model.Id);

            if (existingCarOwnerProfile == null || existingCarOwnerProfile.Data == null)
                return ServiceResult<CarOwnerProfileDTO>.Fail("Car Owner Profile not found.");

            #region Handle Image Upload

            // Image is optional
            if (request.Model.ProfileImage != null)
            {
                if (existingCarOwnerProfile.Data.ProfileImageUrl != null)
                    await _fileStorageService.DeleteFileAsync(existingCarOwnerProfile.Data.ProfileImageUrl);
                // Save image
                var imageResult = await _fileStorageService.SaveFileAsync(request.Model.ProfileImage, "CarOwnerProfiles");

                // Update the DTO with new image path
                request.Model.ProfileImageUrl = imageResult.Data;
            }

            #endregion Handle Image Upload

            // Map only the updatable properties, preserving the relationship
            _mapper.Map(request.Model, existingCarOwnerProfile.Data);

            #endregion Mapping

            var result = await _carOwnerProfileService.UpdateAsync(existingCarOwnerProfile.Data);

            //UpdateResult : Faild
            if (!result.Success)
                return ServiceResult<CarOwnerProfileDTO>.Fail(result.Message ?? "Failed to Update Car Owner Profile.");

            //createResult : Success
            //Map Entity => DTO

            var carOwnerProfileDTO = _mapper.Map<CarOwnerProfileDTO>(result.Data);

            return ServiceResult<CarOwnerProfileDTO>.Ok(carOwnerProfileDTO);

            #endregion Valid
        }
    }
}