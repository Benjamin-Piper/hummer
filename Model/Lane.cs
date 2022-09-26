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
        private readonly Direction direction;
        private readonly Point origin;
        private readonly int width;
        private const int MAX_VEHICLES = 5;
        private readonly List<Vehicle> movingVehicles = new List<Vehicle>(MAX_VEHICLES);

        public Lane(Canvas2DContext context, Direction direction, Point origin, int width)
        {
            this.context = context;
            this.direction = direction;
            this.origin = origin;
            this.width = width;
        }  

        public async Task Animate()
        {
            var despawnTasks = new List<Task>();
            
            foreach (var currentVehicle in this.movingVehicles)
            {
                await this.EraseVehicle(currentVehicle);
                if (this.direction == Direction.Left)
                {
                    currentVehicle.X--;
                }
                else
                {
                    currentVehicle.X++;
                }

                if (this.IsOffscreen(currentVehicle))
                {
                    despawnTasks.Add(new Task(() => DespawnVehicle(currentVehicle)));
                }

                await this.DrawVehicle(currentVehicle);
            }

            // Despawn outside of foreach, we can't modify a List in a foreach
            foreach (var task in despawnTasks)
            {
                task.Start();
            }
        }

        private async Task DrawVehicle(Vehicle currentVehicle)
        {
            // Must be called before every drawing. Not including this cost me hours of debugging :(
            await this.context.BeginPathAsync();
            await this.context.SetFillStyleAsync(currentVehicle.Colour);
            await this.context.SetLineWidthAsync(Vehicle.StrokeWidth);
            await this.context.RectAsync(
                currentVehicle.X,
                GetDrawHeight(currentVehicle),
                currentVehicle.Length,
                currentVehicle.Width
            );
            await this.context.FillAsync();
            await this.context.StrokeAsync();
        }

        private double GetDrawHeight(Vehicle currentVehicle)
        {
            return this.origin.Y - (currentVehicle.Width / 2) - Vehicle.StrokeWidth;
        }

        private async Task EraseVehicle(Vehicle currentVehicle)
        {
            await this.context.ClearRectAsync(
                currentVehicle.LeftEdge,
                GetDrawHeight(currentVehicle) - Vehicle.StrokeWidth,
                currentVehicle.TotalLength,
                currentVehicle.TotalWidth
            );
        }

        public bool IsAvailabe()
        {
            if (movingVehicles.Count == MAX_VEHICLES)
            {
                return false;
            }

            var someVehicleIsInTheWay = movingVehicles.Any((currentVehicle) => {
                if (this.direction == Direction.Left)
                {
                    return (this.origin.X - Vehicle.MinBumperDistance) < currentVehicle.RightEdge;
                }
                else
                {
                    return currentVehicle.LeftEdge < (this.origin.X + Vehicle.MinBumperDistance);
                }
            });

            return !someVehicleIsInTheWay;
        }

        public bool IsOffscreen(Vehicle vehicle)
        {
            if (this.direction == Direction.Left)
            {
                return vehicle.RightEdge < (this.origin.X - this.width);
            }
            else
            {
                return (this.origin.X + this.width) < vehicle.LeftEdge;
            }
        }

        public void DespawnVehicle(Vehicle vehicle)
        {
            this.movingVehicles.Remove(vehicle);
        }

        public void SpawnVehicle(Vehicle vehicle)
        {
            vehicle.X = this.origin.X;
            this.movingVehicles.Add(vehicle);
        }
    }
}
