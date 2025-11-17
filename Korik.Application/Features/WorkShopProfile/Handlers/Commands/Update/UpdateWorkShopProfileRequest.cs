using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record UpdateWorkShopProfileRequest(UpdateWorkShopProfileDTO Model) : IRequest<ServiceResult<WorkShopProfileDTO>> { }

    public class UpdateWorkShopProfileRequestHandler : IRequestHandler<UpdateWorkShopProfileRequest, ServiceResult<WorkShopProfileDTO>>
    {
        #region Dependency Injection

        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateWorkShopProfileDTO> _validator;
        private readonly IFileStorageService _fileStorageService;

        public UpdateWorkShopProfileRequestHandler
            (
            IWorkShopProfileService workShopProfileService,
            IMapper mapper,
            IValidator<UpdateWorkShopProfileDTO> validator,
            IFileStorageService fileStorageService
            )
        {
            _workShopProfileService = workShopProfileService;
            _mapper = mapper;
            _validator = validator;
            _fileStorageService = fileStorageService;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkShopProfileDTO>> Handle(UpdateWorkShopProfileRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkShopProfileDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            #region Mapping

            var existingWorkShopProfile = await _workShopProfileService.GetByIdAsync(request.Model.Id);
            if (existingWorkShopProfile?.Data == null)
                return ServiceResult<WorkShopProfileDTO>.Fail("Car Owner Profile not found.");

            #region Handle Image Upload

            if (request.Model.LicenceImage != null)
            {
                if (existingWorkShopProfile.Data.LicenceImageUrl != null)
                    await _fileStorageService.DeleteFileAsync(existingWorkShopProfile.Data.LicenceImageUrl);

                var imageResult = await _fileStorageService.SaveFileAsync(request.Model.LicenceImage, "WorkShopProfile/Licence");

                request.Model.LicenceImageUrl = imageResult.Data;
            }

            if (request.Model.LogoImage != null)
            {
                if (existingWorkShopProfile.Data.LogoImageUrl != null)
                    await _fileStorageService.DeleteFileAsync(existingWorkShopProfile.Data.LogoImageUrl);

                var imageResult = await _fileStorageService.SaveFileAsync(request.Model.LogoImage, "WorkShopProfile/Logo");

                request.Model.LogoImageUrl = imageResult.Data;
            }

            #endregion Handle Image Upload

            _mapper.Map(request.Model, existingWorkShopProfile.Data);

            #endregion Mapping

            var result = await _workShopProfileService.UpdateAsync(existingWorkShopProfile.Data);

            // UpdateResult : Faild
            if (!result.Success)
                return ServiceResult<WorkShopProfileDTO>.Fail(result.Message ?? "Failed to Update Car Owner Profile.");

            //createResult : Success
            //Map Entity => DTO

            var workShopProfileDTO = _mapper.Map<WorkShopProfileDTO>(result.Data);

            return ServiceResult<WorkShopProfileDTO>.Ok(workShopProfileDTO);

            #endregion Valid
        }
    }
}