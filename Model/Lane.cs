using Blazor.Extensions.Canvas.Canvas2D;

namespace Hummer.Model
{
    public interface ILane
    {
        Task Animate();
        bool IsAvailabe();
        void SpawnVehicle(Vehicle vehicle);
    }

    public class Lane : ILane
    {
        private readonly Canvas2DContext context;
        private readonly LaneConfig config;
        private readonly FootBridge footBridge;
        private const int MAX_VEHICLES = 5;
        private readonly List<Vehicle> movingVehicles = new List<Vehicle>(MAX_VEHICLES);

        public Lane(Canvas2DContext context, LaneConfig config, FootBridge footBridge)
        {
            this.context = context;
            this.config = config;
            this.footBridge = footBridge;
        }  

        public async Task Animate()
        {
            var despawnTasks = new List<Task>();
            
            foreach (var currentVehicle in this.movingVehicles)
            {
                await this.EraseVehicle(currentVehicle);  
                this.MoveVehicle(currentVehicle);
                if (this.IsOffscreen(currentVehicle))
                {
                    despawnTasks.Add(new Task(() => DespawnVehicle(currentVehicle)));
                }
                await this.DrawVehicle(currentVehicle);
            }

            // despawn here as we can't modify Lists inside a foreach keyword
            despawnTasks.ForEach((t) => t.Start());
        }

        private async Task DrawVehicle(Vehicle currentVehicle)
        {
            // Must be called before every drawing. Not including this cost me hours of debugging :(
            await this.context.BeginPathAsync();
            await this.context.SetFillStyleAsync(currentVehicle.Colour);
            await this.context.SetLineWidthAsync(Vehicle.StrokeWidth);
            await this.context.RectAsync(
                currentVehicle.X,
                GetDrawHeight(),
                Vehicle.InnerLength,
                Vehicle.InnerWidth
            );
            await this.context.FillAsync();
            await this.context.StrokeAsync();

            if (this.IsUnderFootBridge(currentVehicle))
            {
                // We redraw this instead of setting globalCompositeOperation to destination-over
                // Is because we are constantly erasing the vehicles
                await this.context.BeginPathAsync();
                await this.context.SetFillStyleAsync(this.footBridge.Colour);
                await this.context.RectAsync(
                    this.footBridge.LeftEdge,
                    GetDrawHeight() - Vehicle.StrokeWidth,
                    this.footBridge.Width,
                    Vehicle.OuterWidth
                );
                await this.context.FillAsync();
            }
        }

        private double GetDrawHeight()
        {
            return this.config.Origin.Y - (Vehicle.InnerWidth / 2) - Vehicle.StrokeWidth;
        }

        private async Task EraseVehicle(Vehicle currentVehicle)
        {
            await this.context.ClearRectAsync(
                currentVehicle.LeftEdge,
                GetDrawHeight() - Vehicle.StrokeWidth,
                Vehicle.OuterLength,
                Vehicle.OuterWidth
            );
        }

        public bool IsAvailabe()
        {
            if (movingVehicles.Count == MAX_VEHICLES)
            {
                return false;
            }

            var someVehicleIsInTheWay = movingVehicles.Any((currentVehicle) => {
                if (this.config.Direction == Direction.Left)
                {
                    return (this.config.Origin.X - Vehicle.MinBumperDistance) < currentVehicle.RightEdge;
                }
                else
                {
                    return currentVehicle.LeftEdge < (this.config.Origin.X + Vehicle.MinBumperDistance);
                }
            });

            return !someVehicleIsInTheWay;
        }

        private bool IsUnderFootBridge(Vehicle vehicle)
        {
            var EdgeIsUnder = (int x) => this.footBridge.LeftEdge <= x && x <= this.footBridge.RightEdge;
            var EdgeOnEitherSide = vehicle.LeftEdge < this.footBridge.LeftEdge && this.footBridge.RightEdge < vehicle.RightEdge;
            return new List<bool>()
            {
                EdgeIsUnder(vehicle.LeftEdge),
                EdgeIsUnder(vehicle.RightEdge),
                EdgeOnEitherSide,
            }.Any((x) => x);    
        }

        private bool IsOffscreen(Vehicle vehicle)
        {
            if (this.config.Direction == Direction.Left)
            {
                return vehicle.RightEdge < (this.config.Origin.X - this.config.Width);
            }
            else
            {
                return (this.config.Origin.X + this.config.Width) < vehicle.LeftEdge;
            }
        }

        private void MoveVehicle(Vehicle vehicle)
        {
            if (this.config.Direction == Direction.Left)
            {
                vehicle.X--;
            }
            else
            {
                vehicle.X++;
            }
        }

        private void DespawnVehicle(Vehicle vehicle)
        {
            this.movingVehicles.Remove(vehicle);
        }

        public void SpawnVehicle(Vehicle vehicle)
        {
            vehicle.X = this.config.Origin.X;
            this.movingVehicles.Add(vehicle);
        }
    }
}
