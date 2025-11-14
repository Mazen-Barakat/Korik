using System;
using Korik.Domain;

namespace Korik.Application
{
    public class UpdateCarExpenseDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public int CarId { get; set; }
    }
}
