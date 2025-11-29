using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Korik.Domain;

namespace Korik.Application
{
    public class CarDTO
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int EngineCapacity { get; set; }
        public int CurrentMileage { get; set; }
        public string LicensePlate { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public FuelType FuelType { get; set; }
        public int CarOwnerProfileId { get; set; }

        public CarOrigin Origin { get; set; }

    }
}
