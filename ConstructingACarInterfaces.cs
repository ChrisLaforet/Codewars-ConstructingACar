namespace CodewarsConstructingACar;

public interface ICar
{
    bool EngineIsRunning { get; }        

    void BrakeBy(int speed); // car #2

    void Accelerate(int speed); // car #2

    void EngineStart();

    void EngineStop();

    void FreeWheel(); // car #2

    void Refuel(double liters);

    void RunningIdle();
}

public interface IDrivingInformationDisplay // car #2
{
    int ActualSpeed { get; }
}

public interface IDrivingProcessor // car #2
{
    double ActualConsumption { get; } // car #3
    
    int ActualSpeed { get; }
    
    void EngineStart(); // car #3

    void EngineStop(); // car #3

    void IncreaseSpeedTo(int speed);

    void ReduceSpeed(int speed);
}

public interface IEngine
{
    bool IsRunning { get; }

    void Consume(double liters);

    void Start();

    void Stop();
}

public interface IFuelTank
{
    double FillLevel { get; }

    bool IsOnReserve { get; }

    bool IsComplete { get; }

    void Consume(double liters);

    void Refuel(double liters);        
}

public interface IFuelTankDisplay
{
    double FillLevel { get; }

    bool IsOnReserve { get; }

    bool IsComplete { get; }
}

public interface IOnBoardComputer // car #3
{
    int TripRealTime { get; }

    int TripDrivingTime { get; }

    int TripDrivenDistance { get; }

    int TotalRealTime { get; }

    int TotalDrivingTime { get; }

    int TotalDrivenDistance { get; }

    double TripAverageSpeed { get; }

    double TotalAverageSpeed { get; }

    int ActualSpeed { get; }

    double ActualConsumptionByTime { get; }

    double ActualConsumptionByDistance { get; }

    double TripAverageConsumptionByTime { get; }

    double TotalAverageConsumptionByTime { get; }

    double TripAverageConsumptionByDistance { get; }

    double TotalAverageConsumptionByDistance { get; }

    int EstimatedRange { get; }

    void ElapseSecond();

    void TripReset();

    void TotalReset();
}

public interface IOnBoardComputerDisplay // car #3
{
    int TripRealTime { get; }

    int TripDrivingTime { get; }

    double TripDrivenDistance { get; }

    int TotalRealTime { get; }

    int TotalDrivingTime { get; }

    double TotalDrivenDistance { get; }

    int ActualSpeed { get; }

    double TripAverageSpeed { get; }

    double TotalAverageSpeed { get; }

    double ActualConsumptionByTime { get; }

    double ActualConsumptionByDistance { get; }

    double TripAverageConsumptionByTime { get; }

    double TotalAverageConsumptionByTime { get; }

    double TripAverageConsumptionByDistance { get; }

    double TotalAverageConsumptionByDistance { get; }

    int EstimatedRange { get; }

    void TripReset();

    void TotalReset();
}