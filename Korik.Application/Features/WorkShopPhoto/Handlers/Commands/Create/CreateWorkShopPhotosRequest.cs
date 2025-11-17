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
    public record CreateWorkShopPhotosRequest(CreateWorkShopPhotosDTO Model) : IRequest<ServiceResult<WorkShopPhotosDTO>> { }

    public class CreateWorkShopPhotosRequestHandler : IRequestHandler<CreateWorkShopPhotosRequest, ServiceResult<WorkShopPhotosDTO>>
    {
        #region Dependency Injection

        private readonly IWorkShopPhotoService _workShopPhotoService;
        private readonly IValidator<CreateWorkShopPhotosDTO> _validator;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public CreateWorkShopPhotosRequestHandler
            (
            IWorkShopPhotoService workShopPhotoService,
            IValidator<CreateWorkShopPhotosDTO> validator,
            IFileStorageService fileStorageService,
            IMapper mapper

            )
        {
            _workShopPhotoService = workShopPhotoService;
            _validator = validator;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkShopPhotosDTO>> Handle(CreateWorkShopPhotosRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkShopPhotosDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region valid

            #region Save the Images

            var photos = new List<WorkShopPhoto>();

            foreach (var photo in request.Model.Photos)
            {
                var saveResult = await _fileStorageService.SaveFileAsync(photo, "WorkShopProfile/photos");

                if (!saveResult.Success)
                    return ServiceResult<WorkShopPhotosDTO>.Fail($"Error saving file: {saveResult.Message}");

                var photoEntity = _mapper.Map<WorkShopPhoto>(request.Model);
                photoEntity.PhotoUrl = saveResult.Data;

                var result = await _workShopPhotoService.CreateAsync(photoEntity);

                if (!result.Success)
                    return ServiceResult<WorkShopPhotosDTO>.Fail(result.Message ?? "Failed to create subcategory.");

                photos.Add(result.Data);
            }

            #endregion Save the Images

            return ServiceResult<WorkShopPhotosDTO>.Ok(new WorkShopPhotosDTO
            {
                WorkShopProfileId = request.Model.WorkShopProfileId,
                Photos = _mapper.Map<List<WorkShopPhotoItemDTO>>(photos)
            });

            #endregion valid
        }
    }
}