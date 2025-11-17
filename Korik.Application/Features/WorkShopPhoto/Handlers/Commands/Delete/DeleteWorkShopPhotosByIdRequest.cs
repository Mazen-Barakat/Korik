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
    public record DeleteWorkShopPhotosByIdRequest(DeleteWorkShopPhotosByIdDTO Model) : IRequest<ServiceResult<WorkShopPhotoItemDTO>> { }

    public class DeleteWorkShopPhotosByIdRequestHandler : IRequestHandler<DeleteWorkShopPhotosByIdRequest, ServiceResult<WorkShopPhotoItemDTO>>
    {
        #region Dependency Injection

        private readonly IWorkShopPhotoService _workShopPhotoService;
        private readonly IMapper _mapper;
        private readonly IValidator<DeleteWorkShopPhotosByIdDTO> _validator;
        private readonly IFileStorageService _fileStorageService;

        public DeleteWorkShopPhotosByIdRequestHandler
            (
            IWorkShopPhotoService workShopPhotoService,
            IMapper mapper,
            IValidator<DeleteWorkShopPhotosByIdDTO> validator,
            IFileStorageService fileStorageService
            )
        {
            _workShopPhotoService = workShopPhotoService;
            _mapper = mapper;
            _validator = validator;
            _fileStorageService = fileStorageService;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkShopPhotoItemDTO>> Handle(DeleteWorkShopPhotosByIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkShopPhotoItemDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _workShopPhotoService.DeleteAsync(request.Model.Id);
            if (!result.Success)
            {
                return ServiceResult<WorkShopPhotoItemDTO>.Fail(result.Message ?? "Failed to delete Image.");
            }
            await _fileStorageService.DeleteFileAsync(result.Data.PhotoUrl);

            // Map deleted entity to DTO
            var workShopPhotoItemDTO = _mapper.Map<WorkShopPhotoItemDTO>(result.Data);

            return ServiceResult<WorkShopPhotoItemDTO>.Ok(workShopPhotoItemDTO);

            #endregion Valid
        }
    }
}