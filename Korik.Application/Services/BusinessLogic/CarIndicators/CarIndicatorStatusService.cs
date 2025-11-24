using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarIndicatorStatusService : ICarIndicatorStatusService
    {
            public CarIndicator CalculateAll(
        IndicatorType type,
        DateTime lastChecked,
        DateTime nextChecked,
        int nextMileage,
        int currentMileage)
        {
            var result = new CarIndicator();

            // -----------------------------
            // Mileage Difference
            // -----------------------------
            result.MileageDifference = nextMileage - currentMileage;

            // -----------------------------
            // Time Difference & Percentage
            // -----------------------------
            var now = DateTime.Now;

            // Total duration in days
            var totalDays = (nextChecked - lastChecked).TotalDays;
            if (totalDays <= 0) totalDays = 1; // avoid divide by zero

            // Remaining days from now to nextChecked
            var remainingDays = (nextChecked - now).TotalDays;

            // Clamp remainingDays to 0..totalDays
            if (remainingDays > totalDays) remainingDays = totalDays;
            if (remainingDays < 0) remainingDays = 0;

            result.TimeDifference = nextChecked - now;
            result.TimeDifferenceAsPercentage = (remainingDays / totalDays) * 100;

            // -----------------------------
            // CarStatus Logic
            // -----------------------------
            result.CarStatus = CalculateStatus(type, result.TimeDifferenceAsPercentage, result.MileageDifference);

            return result;
        }


        // -----------------------------
        // Determine CarStatus based on type
        // -----------------------------
        private CarStatus CalculateStatus(IndicatorType type, double timePercentage, int mileageLeft)
        {
            switch (type)
            {
                case IndicatorType.ACService:
                case IndicatorType.CarLicenseAndEnsuranceExpiry:
                    return TimeBasedStatus(timePercentage);

                case IndicatorType.OilChange:
                    return MileageBasedStatus(mileageLeft);

                case IndicatorType.GeneralMaintenance:
                case IndicatorType.TireChange:
                    return WorstOf(TimeBasedStatus(timePercentage), MileageBasedStatus(mileageLeft));

                case IndicatorType.BatteryHealth:
                    return TimeBasedStatus(timePercentage);

                default:
                    return CarStatus.UnKnown;
            }
        }

        // -----------------------------
        // Time-based status using percentage
        // -----------------------------
        private CarStatus TimeBasedStatus(double timePercentage)
        {
            if (timePercentage > 50) return CarStatus.Normal;
            if (timePercentage > 15 && timePercentage <= 50) return CarStatus.Warning;
            return CarStatus.Critical;
            // >85%
        }

        // -----------------------------
        // Mileage-based status
        // -----------------------------
        private CarStatus MileageBasedStatus(int mileageLeft)
        {
            if (mileageLeft > 2000) return CarStatus.Normal;
            if (mileageLeft > 500) return CarStatus.Warning;
            return CarStatus.Critical;
        }

        // -----------------------------
        // Return worst of two statuses
        // -----------------------------
        private CarStatus WorstOf(CarStatus a, CarStatus b)
            => (CarStatus)Math.Max((int)a, (int)b);
    }

}
