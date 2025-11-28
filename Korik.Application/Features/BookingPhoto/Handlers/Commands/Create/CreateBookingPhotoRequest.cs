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
    public record CreateBookingPhotoRequest(CreateBookingPhotoDTO Model) : IRequest<ServiceResult<BookingPhotoDTO>> { }

    public class CreateBookingPhotoRequestHandler : IRequestHandler<CreateBookingPhotoRequest, ServiceResult<BookingPhotoDTO>>
    {
        #region Dependency Injection

        private readonly IBookingPhotoService _bookingPhotoService;
        private readonly IValidator<CreateBookingPhotoDTO> _validator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public CreateBookingPhotoRequestHandler
            (
            IBookingPhotoService bookingPhotoService,
            IValidator<CreateBookingPhotoDTO> validator,
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

        public async Task<ServiceResult<BookingPhotoDTO>> Handle(CreateBookingPhotoRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<BookingPhotoDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region valid

            #region Save the Images

            var photos = new List<BookingPhoto>();

            foreach (var photo in request.Model.Photos)
            {
                var saveImageResult = await _fileStorageService.SaveFileAsync(photo, "Booking");

                if (!saveImageResult.Success || string.IsNullOrEmpty(saveImageResult.Data))
                    return ServiceResult<BookingPhotoDTO>.Fail($"Error saving file: {saveImageResult.Message}");

                var photoEntity = _mapper.Map<BookingPhoto>(request.Model);
                photoEntity.PhotoUrl = saveImageResult.Data;

                var result = await _bookingPhotoService.CreateAsync(photoEntity);

                if (!result.Success)
                    return ServiceResult<BookingPhotoDTO>.Fail(result.Message ?? "Failed to create subcategory.");

                photos.Add(result.Data);
            }

            #endregion Save the Images

            return ServiceResult<BookingPhotoDTO>.Ok(new BookingPhotoDTO
            {
                BookingId = request.Model.BookingId,
                Photos = _mapper.Map<List<BookingPhotoItemDTO>>(photos)
            });

            #endregion valid
        }
    }
}