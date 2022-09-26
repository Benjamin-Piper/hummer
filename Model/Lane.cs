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
                await this.EraseVehicle(vehicle);
                if (this.direction == Direction.Left)
                {
                    vehicle.X--;
                }
                else
                {
                    vehicle.X++;
                }
                await this.DrawVehicle(vehicle);
            }
        }

        /*
            O = origin (x,y)
            L = length
            W = width

            +-------------+
            |             |
            |    O---L    |
            |    |   |    |
            |    W---+    |
            |             |
            +-------------+
        */
        private async Task DrawVehicle(Vehicle vehicle)
        {
            // Must be called before every drawing. Not including this cost me hours of debugging :(
            await this.context.BeginPathAsync();
            await this.context.SetFillStyleAsync(vehicle.Colour);
            await this.context.SetLineWidthAsync(Vehicle.StrokeWidth);
            await this.context.RectAsync(
                vehicle.X,
                GetDrawHeight(vehicle),
                vehicle.Length,
                vehicle.Width
            );
            await this.context.FillAsync();
            await this.context.StrokeAsync();
        }

        private double GetDrawHeight(Vehicle vehicle)
        {
            return this.origin.Y - (vehicle.Width / 2) - Vehicle.StrokeWidth;
        }

        /*
            O = origin (x,y)
            L = length
            W = width

            O-------------L
            |             |
            |    +---+    |
            |    |   |    |
            |    +---+    |
            |             |
            W-------------+
        */
        private async Task EraseVehicle(Vehicle vehicle)
        {
            await this.context.ClearRectAsync(
                vehicle.LeftEdge,
                GetDrawHeight(vehicle) - Vehicle.StrokeWidth,
                vehicle.TotalLength,
                vehicle.TotalWidth
            );
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
