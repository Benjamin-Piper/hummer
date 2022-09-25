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
        private readonly List<Vehicle> movingVehicles = new List<Vehicle>();
        private readonly int width;

        public Lane(Canvas2DContext context, Direction direction, Point origin, int width)
        {
            this.context = context;
            this.direction = direction;
            this.origin = origin;
            this.width = width;
        }  

        public async Task Animate()
        {
            foreach (var vehicle in this.movingVehicles)
            {
                await EraseVehicle(vehicle);
                if (this.direction == Direction.Left)
                {
                    vehicle.X--;
                }
                else
                {
                    vehicle.X++;
                }
                await DrawVehicle(vehicle);
            } 
        }

        private async Task DrawVehicle(Vehicle vehicle)
        {
            await this.context.SetFillStyleAsync(vehicle.Colour);
            await this.context.FillRectAsync(vehicle.X, GetDrawHeight(vehicle), vehicle.Length, vehicle.Width);
        }

        private double GetDrawHeight(Vehicle vehicle)
        {
            return this.origin.Y - (vehicle.Width / 2);
        }

        private async Task EraseVehicle(Vehicle vehicle)
        {
            await this.context.ClearRectAsync(vehicle.X, GetDrawHeight(vehicle), vehicle.Length, vehicle.Width);
        }

        public bool IsAvailabe()
        {
            return true;
        }

        public void SpawnVehicle(Vehicle vehicle)
        {
            vehicle.X = this.origin.X;
            this.movingVehicles.Add(vehicle);
        }
    }
}
