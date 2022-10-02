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
        private readonly Footbridge footbridge;
        private const int MAX_VEHICLES = 5;
        private readonly List<Vehicle> movingVehicles = new List<Vehicle>(MAX_VEHICLES);

        private bool noteIsPlaying = false;

        public Lane(Canvas2DContext context, LaneConfig config, Footbridge footbridge)
        {
            this.context = context;
            this.config = config;
            this.footbridge = footbridge;
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

            if (this.movingVehicles.Any((v) => this.IsUnderFootbridge(v)))
            {
                // We must draw this again instead of 
                // using globalCompositeOperation = "destination-over"
                // Because we constantly erase the vehicle
                await this.DrawFootBridge();
                this.config.PlayNote();
                this.noteIsPlaying = true;
            }
            else if (this.noteIsPlaying)
            {
                this.config.PauseNote();
                this.noteIsPlaying = false;
            }

            // despawn here as we can't modify Lists inside a foreach keyword
            despawnTasks.ForEach((t) => t.Start());
        }

        private async Task DrawFootBridge()
        {
            await this.context.BeginPathAsync();
            await this.context.SetFillStyleAsync(this.footbridge.Colour);
            await this.context.RectAsync(
                this.footbridge.LeftEdge,
                Vehicle.GetTopEdge(this.config.Origin.Y) - Vehicle.DrawHeightOffset,
                this.footbridge.Width,
                Vehicle.OuterWidth
            );
            await this.context.FillAsync();
        }

        private async Task DrawVehicle(Vehicle currentVehicle)
        {
            // Must be called before every drawing. Not including this cost me hours of debugging :(
            await this.context.BeginPathAsync();
            await this.context.SetFillStyleAsync(currentVehicle.Colour);
            await this.context.SetLineWidthAsync(Vehicle.StrokeWidth);
            await this.context.RectAsync(
                currentVehicle.X,
                this.config.Origin.Y - Vehicle.DrawHeightOffset,
                Vehicle.InnerLength,
                Vehicle.InnerWidth
            );
            await this.context.FillAsync();
            await this.context.StrokeAsync();
        }

        private async Task EraseVehicle(Vehicle currentVehicle)
        {
            await this.context.ClearRectAsync(
                currentVehicle.LeftEdge,
                Vehicle.GetTopEdge(this.config.Origin.Y) - Vehicle.DrawHeightOffset,
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

        private bool IsUnderFootbridge(Vehicle vehicle)
        {
            var EdgeIsUnder = (int x) => this.footbridge.LeftEdge <= x && x <= this.footbridge.RightEdge;
            var EdgeOnEitherSide = vehicle.LeftEdge < this.footbridge.LeftEdge && this.footbridge.RightEdge < vehicle.RightEdge;
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
            if (this.config.Direction == Direction.Left)
            {
                vehicle.X = this.config.Origin.X;
            }
            else
            {
                vehicle.X -= vehicle.RightEdge;
            }
            this.movingVehicles.Add(vehicle);
        }
    }
}
