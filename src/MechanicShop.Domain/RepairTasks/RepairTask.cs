using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Domain.RepairTasks
{
    public sealed class RepairTask : AuditableEntity
    {
        public string? Name { get; set; }
        public decimal LaborCost { get; set; }
        public RepairDurationInMinutes EstimatedDurationInMins { get; set; }

        private readonly List<Part> _parts = [];
        public IEnumerable<Part> Parts => _parts.AsReadOnly();
        public decimal TotalCost => LaborCost + Parts.Sum(p => p.Cost * p.Quantity);

        private RepairTask() { }

        private RepairTask(Guid id, string name, decimal laborCost, RepairDurationInMinutes estimatedDurationInMins, List<Part> parts)
            : base(id)
        {
            Name = name;
            LaborCost = laborCost;
            EstimatedDurationInMins= estimatedDurationInMins;
            _parts = parts;
        }

        public static Result<RepairTask>Create(Guid id, string name, decimal laborCost, RepairDurationInMinutes estimatedDurationInMins, List<Part> parts)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RepairTaskErrors.NameRequired;

            if (laborCost <= 0 || laborCost > 10000)
                return RepairTaskErrors.LaborCostInvalid;

            if (!Enum.IsDefined(estimatedDurationInMins))
                return RepairTaskErrors.DurationInvalid;

            return new RepairTask(id, name.Trim(), laborCost, estimatedDurationInMins, parts);
        }

        public Result<Updated>Update(string name, decimal laborCost, RepairDurationInMinutes estimatedDurationInMins)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RepairTaskErrors.NameRequired;

            if (laborCost <= 0 || laborCost > 10000)
                return RepairTaskErrors.LaborCostInvalid;

            if (!Enum.IsDefined(estimatedDurationInMins))
                return RepairTaskErrors.DurationInvalid;

            Name = name.Trim();
            LaborCost = laborCost;
            EstimatedDurationInMins = estimatedDurationInMins;

            return Result.Updated;
        }

        public Result<Updated>UpsertParts(List<Part> incomingParts)
        {
            _parts.RemoveAll(exsiting => incomingParts.All(p => p.Id != exsiting.Id));

            foreach (var incoming in incomingParts)
            {
                var exsiting = _parts.FirstOrDefault(p => p.Id == incoming.Id);

                if (exsiting is null)
                    _parts.Add(incoming);
                else
                {
                    var updatePartResult = exsiting.Update(incoming.Name!, incoming.Quantity, incoming.Cost);

                    if (updatePartResult.IsError)
                        return updatePartResult.Errors!;
                }
            }

            return Result.Updated;
        }
    }
}
