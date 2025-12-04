using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class WorkShopProfileRepository : GenericRepository<WorkShopProfile>, IWorkShopProfileRepository
    {
        private readonly Korik _context;

        public WorkShopProfileRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<WorkShopProfile?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            var entity = await _context.Set<WorkShopProfile>().AsNoTracking().FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);
            return entity;
        }

        public async Task<WorkShopProfile?> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<WorkShopProfile, object>>[] includes)
        {
            var query = _context.Set<WorkShopProfile>().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var entity = await query.AsNoTracking().FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);
            return entity;
        }

        public async Task<PagedResult<WorkShopProfile>> FilterWorkshopsAsync
            (
            int PageNumber,
            int PageSize,
            string? Name,
            decimal? Latitude,
            decimal? Longitude,
            string? Country,
            string? Governorate,
            string? City,
            bool? DESCRating,
            WorkShopType? WorkShopType,
            CarOrigin? Origin
            )
        {
            var query = _context.Set<WorkShopProfile>()
                .Include(w => w.WorkshopServices)
                .AsNoTracking()
                .Where(w => string.IsNullOrWhiteSpace(Name) || w.Name.Contains(Name))
                .Where(w => string.IsNullOrWhiteSpace(Country) || w.Country == Country)
                .Where(w => string.IsNullOrWhiteSpace(Governorate) || w.Governorate == Governorate)
                .Where(w => string.IsNullOrWhiteSpace(City) || w.City == City)
                .Where(w => !WorkShopType.HasValue || w.WorkShopType == WorkShopType)
                .Where(w => !Origin.HasValue || w.WorkshopServices.Any(s => s.Origin == Origin))
                .Where(w => w.VerificationStatus == VerificationStatus.Verified);

            if (Latitude.HasValue && Longitude.HasValue)
            {
                const double radiusInKm = 500;
                var userLat = (double)Latitude.Value;
                var userLon = (double)Longitude.Value;

                // Filter workshops with valid coordinates (non-zero)
                query = query.Where(w => w.Latitude != 0 && w.Longitude != 0);

                // Bring to memory for distance calculation
                var allWorkshops = await query.ToListAsync();

                // Calculate distance and filter
                var workshopsWithDistance = allWorkshops
                    .Select(w =>
                    {
                        const double R = 6371; // Earth's radius in km
                        var lat1 = userLat * Math.PI / 180;
                        var lat2 = (double)w.Latitude * Math.PI / 180;
                        var dLat = ((double)w.Latitude - userLat) * Math.PI / 180;
                        var dLon = ((double)w.Longitude - userLon) * Math.PI / 180;

                        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                                Math.Cos(lat1) * Math.Cos(lat2) *
                                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                        var distance = R * c;

                        return new { Workshop = w, Distance = distance };
                    })
                    .Where(x => x.Distance <= radiusInKm)
                    .ToList();

                // Apply sorting - OPTION 1: Distance first, then rating
                IEnumerable<WorkShopProfile> sorted;

                if (DESCRating.HasValue)
                {
                    // Sort by distance first, then by rating
                    sorted = DESCRating == true
                        ? workshopsWithDistance
                            .OrderBy(x => x.Distance)
                            .ThenByDescending(x => x.Workshop.Rating)
                            .Select(x => x.Workshop)
                        : workshopsWithDistance
                            .OrderBy(x => x.Distance)
                            .ThenBy(x => x.Workshop.Rating)
                            .Select(x => x.Workshop);
                }
                else
                {
                    // Sort by distance only
                    sorted = workshopsWithDistance
                        .OrderBy(x => x.Distance)
                        .Select(x => x.Workshop);
                }

                var sortedList = sorted.ToList();

                // Manual pagination
                var totalCount1 = sortedList.Count;
                var items1 = sortedList
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                return new PagedResult<WorkShopProfile>
                {
                    Items = items1,
                    TotalRecords = totalCount1,
                    PageSize = PageSize,
                    PageNumber = PageNumber
                };
            }

            //Apply sorting

            if (DESCRating.HasValue)
            {
                query = DESCRating == true
                    ? query.OrderByDescending(w => w.Rating)
                    : query.OrderBy(w => w.Rating);
            }

            // Get total and paginate
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return new PagedResult<WorkShopProfile>
            {
                Items = items,
                TotalRecords = totalCount,
                PageSize = PageSize,
                PageNumber = PageNumber
            };
        }
    }
}