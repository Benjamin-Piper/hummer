using Blazor.Extensions.Canvas.Canvas2D;

namespace Hummer.Model
{
    public interface ISimulation
    {
        Task StartClock();
    }

    public class Traffic : ISimulation
    {
        private readonly Random random = new Random();
        private Canvas2DContext context;
        private int[] laneHeightList;
        private Vehicle[] vehicleList;

        public Traffic(Canvas2DContext context, int[] laneHeightList, Vehicle[] vehicleList)
        {
            this.context = context;
            this.laneHeightList = laneHeightList;
            this.vehicleList = vehicleList;
        }

        private Vehicle RandomlyChooseVehicle()
        {
            var spawnNumber = this.random.NextDouble();
            var lowerBound = 0d;
            var upperBound = 0d;
            foreach (var candidateVehicle in vehicleList)
            {
                upperBound += candidateVehicle.SpawnRate;
                if (lowerBound <= spawnNumber && spawnNumber < upperBound)
                {
                    return candidateVehicle;
                }
                lowerBound += candidateVehicle.SpawnRate;
            }
            throw new SpawnRateOutOfRangeException("There must be at least one vehicle.SpawnRate between 0 and 1");
        }

        private int RandomlyChooseLaneHeight()
        {
            var midpointList = new List<int>();
            for (int i = 0; i < laneHeightList.Length - 1; i++)
            {
                var midpoint = (laneHeightList[i] + laneHeightList[i + 1]) / 2;
                midpointList.Add(midpoint);
            }
            return midpointList[this.random.Next(midpointList.Count)];
        }

        private async Task DrawVehicle(Vehicle vehicle, int laneHeight)
        {
            var drawHeight = laneHeight - (vehicle.Width / 2);
            await this.context.SetFillStyleAsync(vehicle.Colour);
            await this.context.FillRectAsync(0, drawHeight, vehicle.Length, vehicle.Width);
        }

        private async Task SpawnVehicle()
        {
            try
            {
              await DrawVehicle(RandomlyChooseVehicle(), RandomlyChooseLaneHeight());  
            } catch (SpawnRateOutOfRangeException)
            {
                Console.WriteLine("uh oh");
                // TODO remove this catch and have a UI update for this
            }
        }

        public async Task StartClock()
        {
            // TODO infinite loop!
            await SpawnVehicle();
        }      
    }
}
