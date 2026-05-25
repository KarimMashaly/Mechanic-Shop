using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Domain.WorkOrders
{
    public sealed class WorkOrder : AuditableEntity
    {
        public DateTimeOffset StartAtUtc { get; private set; }
        public DateTimeOffset EndAtUtc { get; private set; }
        public Spot Spot { get; private set; }
        public WorkOrderState State { get; private set; }
        public decimal? Discout { get; private set; }
        public decimal? Tax { get; private set; }
        public Guid LaborId { get; set; }
        public Employee? Labor {  get;  set; }
        public Guid VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
        public Invoice? Invoice { get; set; }
        public decimal? TotalPartsCost => _repairTasks.SelectMany(rt => rt.Parts).Sum(p => p.Cost);
        public decimal? TotalLaborCost => _repairTasks.Sum(rt => rt.LaborCost);
        public decimal? Total => (TotalPartsCost ?? 0) + (TotalLaborCost ?? 0);

        private readonly List<RepairTask> _repairTasks = [];
        public IEnumerable<RepairTask> RepairTasks => _repairTasks.AsReadOnly();

        public bool IsEditable => State is not (WorkOrderState.Completed or WorkOrderState.Cancelled or WorkOrderState.InProgress);

        private WorkOrder() { }
        private WorkOrder(Guid id, Guid laborId, Guid vehicleId, DateTimeOffset startAtUtc, DateTimeOffset endAtUtc,
            WorkOrderState state, Spot spot, List<RepairTask> repairTasks) : base(id)
        {
            LaborId = laborId;
            VehicleId = vehicleId;
            StartAtUtc = startAtUtc;
            EndAtUtc = endAtUtc;
            State = state;
            Spot = spot;
            _repairTasks = repairTasks;
        }

        public static Result<WorkOrder> Create(Guid id, Guid laborId, Guid vehicleId, DateTimeOffset startAtUtc, DateTimeOffset endAtUtc,
             Spot spot, List<RepairTask> repairTasks)
        {
            if (id == Guid.Empty)
                return WorkOrderErrors.WorkOrderIdRequired;

            if (laborId == Guid.Empty)
                return WorkOrderErrors.LaborIdRequired;

            if (vehicleId == Guid.Empty)
                return WorkOrderErrors.VehicleIdRequired;

            if (endAtUtc <= startAtUtc)
                return WorkOrderErrors.InvalidTiming;

            if (!Enum.IsDefined(spot))
                return WorkOrderErrors.SpotInvalid;

            if(repairTasks is null || repairTasks.Count <= 0)
                return WorkOrderErrors.RepairTasksRequired;

            return new WorkOrder(id, laborId, vehicleId, startAtUtc, endAtUtc, WorkOrderState.Scheduled, spot, repairTasks);
        }

        public Result<Updated>AddRepairTask(RepairTask repairTasks)
        {
            if (!IsEditable)
                return WorkOrderErrors.Readonly;

            if(_repairTasks.Any(rt=> rt.Id == repairTasks.Id))
                return WorkOrderErrors.RepairTaskAlreadyAdded;

            _repairTasks.Add(repairTasks);

            return Result.Updated;
        }

        public Result<Updated>UpdateTiming(DateTimeOffset startAt,  DateTimeOffset endAt)
        {
            if(!IsEditable)
                return WorkOrderErrors.Readonly;

            if(endAt <= startAt)
                return WorkOrderErrors.InvalidTiming;

            StartAtUtc = startAt;
            EndAtUtc = endAt;

            return Result.Updated;
        }

        public Result<Updated>UpdateLabor(Guid laborId)
        {
            if(!IsEditable)
                return WorkOrderErrors.Readonly;

            if (laborId == Guid.Empty)
                return WorkOrderErrors.LaborIdRequired;

            LaborId = laborId;

            return Result.Updated;
        }

        public Result<Updated>UpdateSpot(Spot spot)
        {
            if(!IsEditable)
                return WorkOrderErrors.Readonly;

            if (!Enum.IsDefined(spot))
                return WorkOrderErrors.SpotInvalid;

            Spot = spot;

            return Result.Updated;
        }

        public Result<Updated>ClearRepairTasks()
        {
            if (!IsEditable)
                return WorkOrderErrors.Readonly;

            _repairTasks.Clear();

            return Result.Updated;
        }

        public bool CanTrasitionTo(WorkOrderState newState)
        {
            return (State, newState) switch
            {
                (WorkOrderState.Scheduled, WorkOrderState.InProgress) => true,
                (WorkOrderState.InProgress, WorkOrderState.Completed) => true,
                (_, WorkOrderState.Cancelled) when State != WorkOrderState.Completed => true,
                _ => false
            };
        }

        public Result<Updated> UpdateState(WorkOrderState newState)
        {
            if (!CanTrasitionTo(newState))
                return WorkOrderErrors.InvalidStateTransition(State, newState);

            State = newState;

            return Result.Updated;
        }

        public Result<Updated> Cancel()
        {
            return UpdateState(WorkOrderState.Cancelled);
        }
    }
}
