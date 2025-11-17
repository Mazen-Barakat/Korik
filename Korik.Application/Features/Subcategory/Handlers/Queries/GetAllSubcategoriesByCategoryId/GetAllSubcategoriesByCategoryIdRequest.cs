using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllSubcategoriesByCategoryIdRequest(GetAllSubcategoriesByCategoryIdDTO model) 
        : IRequest<ServiceResult<IEnumerable<SubcategoryDTO>>> { }


    //public class GetAllSubcategoriesByCategoryIdRequestHandler 
    //    : IRequestHandler<GetAllSubcategoriesByCategoryIdRequest, ServiceResult<IEnumerable<SubcategoryDTO>>>
    //{
    //    private readonly ISubcategoryService _subcategoryService;
    //    public GetAllSubcategoriesByCategoryIdRequestHandler(ISubcategoryService subcategoryService)
    //    {
    //        _subcategoryService = subcategoryService;
    //    }
    //    //public async Task<ServiceResult<IEnumerable<SubcategoryDTO>>> Handle(
    //    //    GetAllSubcategoriesByCategoryIdRequest request, 
    //    //    CancellationToken cancellationToken)
    //    //{
    //    //    //var result = await _subcategoryService.GetAllByCategoryIdAsync(request.model.CategoryId);
    //    //    //return result;
    //    //}
    //}

}
