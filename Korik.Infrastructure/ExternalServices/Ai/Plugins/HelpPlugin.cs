using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    /// <summary>
    /// Plugin for providing help, tips, and general information about Korik.
    /// </summary>
    public class HelpPlugin
    {
        [KernelFunction("GetHelp")]
        [Description("Gets general help information about the Korik app. Use when user asks for help or what they can do.")]
        public Task<string> GetHelpAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine("?? Welcome to Korik AI Assistant!");
            sb.AppendLine("????????????????????????????????????");
            sb.AppendLine();
            sb.AppendLine("I can help you with:");
            sb.AppendLine();
            sb.AppendLine("?? BOOKINGS & APPOINTMENTS");
            sb.AppendLine("   • View upcoming and past bookings");
            sb.AppendLine("   • Check booking status and details");
            sb.AppendLine();
            sb.AppendLine("?? CAR MANAGEMENT");
            sb.AppendLine("   • View your registered cars");
            sb.AppendLine("   • Check maintenance status");
            sb.AppendLine("   • Get maintenance recommendations");
            sb.AppendLine();
            sb.AppendLine("?? EXPENSES");
            sb.AppendLine("   • View car expenses");
            sb.AppendLine("   • Get spending summaries");
            sb.AppendLine("   • Track fuel costs");
            sb.AppendLine();
            sb.AppendLine("?? WORKSHOPS & SERVICES");
            sb.AppendLine("   • Search for workshops");
            sb.AppendLine("   • View ratings and reviews");
            sb.AppendLine("   • Browse available services");
            sb.AppendLine();
            sb.AppendLine("?? Try asking:");
            sb.AppendLine("   \"Show my upcoming bookings\"");
            sb.AppendLine("   \"What's my car maintenance status?\"");
            sb.AppendLine("   \"Find workshops near me\"");
            sb.AppendLine("   \"How much have I spent on my car?\"");

            return Task.FromResult(sb.ToString());
        }

        [KernelFunction("GetMaintenanceTips")]
        [Description("Gets car maintenance tips. Use when user asks for maintenance advice.")]
        public Task<string> GetMaintenanceTipsAsync(
            [Description("Topic: oil, tires, battery, brakes, general")] string? topic = null)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(topic) || topic.ToLower() == "general")
            {
                sb.AppendLine("?? General Car Maintenance Tips:");
                sb.AppendLine("???????????????????????????????????");
                sb.AppendLine();
                sb.AppendLine("? REGULAR CHECKS");
                sb.AppendLine("   • Check oil level monthly");
                sb.AppendLine("   • Inspect tire pressure weekly");
                sb.AppendLine("   • Check all lights weekly");
                sb.AppendLine();
                sb.AppendLine("?? SCHEDULED MAINTENANCE");
                sb.AppendLine("   • Oil change: Every 5,000-10,000 km");
                sb.AppendLine("   • Tire rotation: Every 10,000 km");
                sb.AppendLine("   • Brake inspection: Every 20,000 km");
                sb.AppendLine("   • Air filter: Every 15,000-30,000 km");
                sb.AppendLine();
                sb.AppendLine("?? WARNING SIGNS");
                sb.AppendLine("   • Unusual noises");
                sb.AppendLine("   • Dashboard warning lights");
                sb.AppendLine("   • Vibrations while driving");
                sb.AppendLine("   • Decreased fuel efficiency");
            }
            else if (topic.ToLower() == "oil")
            {
                sb.AppendLine("??? Oil Change Tips:");
                sb.AppendLine();
                sb.AppendLine("?? WHEN TO CHANGE:");
                sb.AppendLine("   • Every 5,000-10,000 km");
                sb.AppendLine("   • At least every 6 months");
                sb.AppendLine();
                sb.AppendLine("?? WARNING SIGNS:");
                sb.AppendLine("   • Dark brown/black oil");
                sb.AppendLine("   • Engine louder than usual");
                sb.AppendLine("   • Oil change light on");
            }
            else if (topic.ToLower().Contains("tire"))
            {
                sb.AppendLine("?? Tire Maintenance:");
                sb.AppendLine();
                sb.AppendLine("?? PRESSURE CHECK:");
                sb.AppendLine("   • Check when cold");
                sb.AppendLine("   • Check weekly");
                sb.AppendLine();
                sb.AppendLine("?? ROTATION:");
                sb.AppendLine("   • Every 8,000-10,000 km");
                sb.AppendLine("   • Replace when tread < 1.6mm");
            }
            else if (topic.ToLower() == "battery")
            {
                sb.AppendLine("?? Battery Tips:");
                sb.AppendLine();
                sb.AppendLine("?? LIFESPAN: 3-5 years");
                sb.AppendLine("?? Keep terminals clean");
                sb.AppendLine();
                sb.AppendLine("?? WARNING SIGNS:");
                sb.AppendLine("   • Slow engine crank");
                sb.AppendLine("   • Dim headlights");
            }
            else if (topic.ToLower() == "brakes")
            {
                sb.AppendLine("?? Brake Tips:");
                sb.AppendLine();
                sb.AppendLine("?? INSPECT: Every 20,000 km");
                sb.AppendLine("?? Replace pads at 3mm");
                sb.AppendLine();
                sb.AppendLine("?? WARNING SIGNS:");
                sb.AppendLine("   • Squealing/grinding noise");
                sb.AppendLine("   • Vibration when braking");
            }
            else
            {
                sb.AppendLine($"No tips for '{topic}'.");
                sb.AppendLine("Try: oil, tires, battery, brakes, general");
            }

            return Task.FromResult(sb.ToString());
        }

        [KernelFunction("GetFAQ")]
        [Description("Gets frequently asked questions about the Korik service.")]
        public Task<string> GetFAQAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine("? Frequently Asked Questions:");
            sb.AppendLine("???????????????????????????????????");
            sb.AppendLine();
            sb.AppendLine("Q: How do I book a service?");
            sb.AppendLine("A: Search for a workshop, select a service, choose your car and date, then confirm.");
            sb.AppendLine();
            sb.AppendLine("Q: Can I cancel a booking?");
            sb.AppendLine("A: Yes, cancel pending bookings from your booking history.");
            sb.AppendLine();
            sb.AppendLine("Q: How do I add a car?");
            sb.AppendLine("A: Go to profile > My Cars > Add Car.");
            sb.AppendLine();
            sb.AppendLine("Q: How do I leave a review?");
            sb.AppendLine("A: After booking completion, rate from booking history.");
            sb.AppendLine();
            sb.AppendLine("Q: How are workshops verified?");
            sb.AppendLine("A: They submit business licenses. Verified = ? badge.");

            return Task.FromResult(sb.ToString());
        }
    }
}
