using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopProfileService : GenericService<WorkShopProfile>, IWorkShopProfileService
    {
        private readonly IWorkShopProfileRepository _workShopProfileRepository;
        public WorkShopProfileService(IWorkShopProfileRepository workShopProfileRepository) : base(workShopProfileRepository)
        {
            _workShopProfileRepository = workShopProfileRepository;
        }

    }
}
