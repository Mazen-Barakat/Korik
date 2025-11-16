using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllCategoriesRequest : IRequest<ServiceResult<IEnumerable<CategoryDTO>>> { }


    public class GetAllCategoriesRequestHandler : IRequestHandler<GetAllCategoriesRequest, ServiceResult<IEnumerable<CategoryDTO>>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public GetAllCategoriesRequestHandler(
            ICategoryService categoryService,
            IMapper mapper
            )
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<CategoryDTO>>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetAllAsync();
            if (!result.Success)
            {
                return ServiceResult<IEnumerable<CategoryDTO>>.Fail(result.Message ?? "Failed to retrieve categories.");
            }


            var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(result.Data);
            return ServiceResult<IEnumerable<CategoryDTO>>.Ok(categoryDTOs);
        }
    }

}
