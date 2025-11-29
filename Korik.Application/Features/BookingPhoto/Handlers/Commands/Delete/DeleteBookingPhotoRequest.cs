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
    public record DeleteBookingPhotoRequest(DeleteBookingPhotoByIdDTO Model) : IRequest<ServiceResult<BookingPhotoItemDTO>> { }

    public class DeleteBookingPhotoRequestHandler : IRequestHandler<DeleteBookingPhotoRequest, ServiceResult<BookingPhotoItemDTO>>
    {
        #region Dependency Injection

        private readonly IBookingPhotoService _bookingPhotoService;
        private readonly IValidator<DeleteBookingPhotoByIdDTO> _validator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public DeleteBookingPhotoRequestHandler
            (
            IBookingPhotoService bookingPhotoService,
            IValidator<DeleteBookingPhotoByIdDTO> validator,
            IMapper mapper,
            IFileStorageService fileStorageService
            )
        {
            _bookingPhotoService = bookingPhotoService;
            _validator = validator;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<BookingPhotoItemDTO>> Handle(DeleteBookingPhotoRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<BookingPhotoItemDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region valid

            var result = await _bookingPhotoService.DeleteAsync(request.Model.Id);
            if (!result.Success)
            {
                return ServiceResult<BookingPhotoItemDTO>.Fail(result.Message ?? "Failed to delete Image.");
            }
            await _fileStorageService.DeleteFileAsync(result.Data.PhotoUrl);

            // Map deleted entity to DTO
            var workShopPhotoItemDTO = _mapper.Map<BookingPhotoItemDTO>(result.Data);

            return ServiceResult<BookingPhotoItemDTO>.Ok(workShopPhotoItemDTO);

            #endregion valid
        }
    }
}