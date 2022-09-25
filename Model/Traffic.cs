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
        private readonly List<ILane> lanes;
        private readonly List<Vehicle> vehicleTypes;

        public Traffic(List<ILane> lanes, List<Vehicle> vehicleTypes)
        {
            this.lanes = lanes;
            this.vehicleTypes = vehicleTypes;
        }

        private ILane RandomlyChooseLane(List<ILane> availableLanes)
        {
            return availableLanes[this.random.Next(availableLanes.Count)];
        }

        private Vehicle RandomlyChooseVehicle()
        {
            var spawnNumber = this.random.NextDouble();
            var lowerBound = 0d;
            var upperBound = 0d;
            foreach (var candidateVehicle in vehicleTypes)
            {
                upperBound += candidateVehicle.SpawnRate;
                if (lowerBound <= spawnNumber && spawnNumber < upperBound)
                {
                    return candidateVehicle with {}; // shallow copy, we'll be modifying its X when it starts moving
                }
                lowerBound += candidateVehicle.SpawnRate;
            }
            throw new SpawnRateOutOfRangeException("There must be at least one vehicle.SpawnRate between 0 and 1");
        }

        public async Task StartClock()
        {
            var lane = RandomlyChooseLane(this.lanes.FindAll(l => l.IsAvailabe()));
            lane.SpawnVehicle(RandomlyChooseVehicle());
            while (true)
            {
                foreach (var currentLane in this.lanes)
                {
                    await currentLane.Animate();
                }
                await Task.Delay(10);
            }
        }      
    }
}
