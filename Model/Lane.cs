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
                await this.DrawVehicle(currentVehicle);
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
            return true;
        }

        public void SpawnVehicle(Vehicle vehicle)
        {
            vehicle.X = this.origin.X;
            this.movingVehicles.Add(vehicle);
        }
    }
}
