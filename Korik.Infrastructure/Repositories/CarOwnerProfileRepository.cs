using Korik.Application.Interfaces.Repositories.User;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class CarOwnerProfileRepository : GenericRepository<CarOwnerProfile>, ICarOwnerProfileRepository
    {
        #region Dependence Injection

        private readonly Korik _context;

        public CarOwnerProfileRepository(Korik context) : base(context)
        {
            _context = context;
        }

        #endregion Dependence Injection
    }
}