using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class WorkshopServiceRepository : GenericRepository<WorkshopService>, IWorkshopServiceRepository
    {
        private readonly Korik context;

        #region Dependence Injection

        public WorkshopServiceRepository(Korik context) : base(context)
        {
            this.context = context;
        }

        #endregion Dependence Injection
    }
}