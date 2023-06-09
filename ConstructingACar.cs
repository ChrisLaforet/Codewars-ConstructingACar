namespace CodewarsConstructingACar;

using System;
	
public class Car : ICar
{
	private const int DefaultMaxAcceleration = 10;		// km/h/sec

	public readonly IFuelTankDisplay fuelTankDisplay;

	private readonly IEngine engine;

	private readonly IFuelTank fuelTank;
	
	public readonly IDrivingInformationDisplay drivingInformationDisplay; // car #2  

	private readonly IDrivingProcessor drivingProcessor; // car #2
	
	public readonly IOnBoardComputerDisplay onBoardComputerDisplay; // car #3

	private readonly IOnBoardComputer onBoardComputer; // car #3

	public Car(double fuelLevel, int maxAcceleration) // car #2
	{
		fuelTank = new FuelTank(fuelLevel);
		engine = new Engine(fuelTank);
		fuelTankDisplay = new FuelTankDisplay(fuelTank);
		var drivingProcessor = new DrivingProcessor(maxAcceleration);
		this.drivingProcessor = drivingProcessor;
		onBoardComputer = new OnBoardComputer(drivingProcessor, fuelTank);
		drivingInformationDisplay = new DrivingInformationDisplay(drivingProcessor);
		onBoardComputerDisplay = new OnBoardComputerDisplay(onBoardComputer);
	}

	public Car() : this(FuelTank.DefaultFuelLevel) { }

	public Car(double fuelLevel) : this(fuelLevel, DefaultMaxAcceleration) { }

	public bool EngineIsRunning => this.engine.IsRunning;
	
	public void BrakeBy(int speed)
	{
		if (EngineIsRunning)
		{
			if (drivingInformationDisplay.ActualSpeed > 0)
			{
				drivingProcessor.ReduceSpeed(speed);
				onBoardComputer.ElapseSecond();
			}
			else
			{
				RunningIdle();
			}
		}
	}

	public void Accelerate(int speed)
	{
		if (EngineIsRunning)
		{
			if (drivingInformationDisplay.ActualSpeed > speed) 
			{
				FreeWheel();
			}
			else
			{
				drivingProcessor.IncreaseSpeedTo(speed);
				ConsumeGas();				
				onBoardComputer.ElapseSecond();
			}
		}
	}

	private void ConsumeGas()
	{
		engine.Consume(drivingProcessor.ActualConsumption);
		if (fuelTank.FillLevel <= 0)
		{
			EngineStop();
		}
	}

	public void EngineStart()
	{
		this.engine.Start();
		this.drivingProcessor.EngineStart();
	}

	public void EngineStop()
	{
		this.engine.Stop();
		this.drivingProcessor.EngineStop();
		onBoardComputer.ElapseSecond();
	}

	public void FreeWheel()
	{
		if (drivingInformationDisplay.ActualSpeed == 0)
		{
			RunningIdle();
		}
		else
		{
			BrakeBy(1);
		}
	}

	public void Refuel(double liters) => this.fuelTank.Refuel(liters);

	public void RunningIdle()
	{
	    if (!EngineIsRunning)
		{
			return;
		}
		engine.Consume(drivingProcessor.ActualConsumption);
		if (fuelTank.FillLevel == 0)
		{
			engine.Stop();
		}
		onBoardComputer.ElapseSecond();
	}
}

class FuelConsumptionRate
{
	public const double IdleConsumptionRate = 0.0003D;

	public static double ConsumptionRateBySpeed(int speed)
	{
		return speed switch
		{
			< 1 => IdleConsumptionRate,
			< 61 => 0.0020D,
			< 101 => 0.0014D,
			< 141 => 0.0020D,
			< 201 => 0.0025D,
			_ => 0.0030D
		};
	}
}

public class Engine : IEngine
{
	private IFuelTank fuelTank;

	internal Engine(IFuelTank fuelTank) => this.fuelTank = fuelTank;

	public bool IsRunning
	{
		get;
		private set;
	}

	public void Consume(double liters) => fuelTank.Consume(liters);

	public void Start() => IsRunning = (fuelTank.FillLevel == 0) ? false : true;

	public void Stop() => IsRunning = false;
}

public class FuelTank : IFuelTank
{
	public const double MaximumFuelLevel = 60.0D;
	public const double DefaultFuelLevel = 20.0D;
	public const double ReserveFuelLevel = 5.0D;

	internal FuelTank(double fuelLevel)
	{
		this.FillLevel = fuelLevel switch
		{
			<= 0 => 0,
			> MaximumFuelLevel => MaximumFuelLevel,
			_ => fuelLevel
		};
	}

	public double FillLevel
	{
		get;
		private set;
	}

	public bool IsOnReserve => FillLevel <= FuelTank.ReserveFuelLevel;

	public bool IsComplete => FillLevel == FuelTank.MaximumFuelLevel;

	public void Consume(double liters)
	{
		if (FillLevel > 0)
		{
			FillLevel -= liters;
			if (FillLevel < 0)
			{
				FillLevel = 0;
			}
		}
	}

	public void Refuel(double liters)
	{
		if (liters <= 0)
		{
			return;
		}
		double total = FillLevel + liters;
		if (total >= FuelTank.MaximumFuelLevel)
		{
			FillLevel = FuelTank.MaximumFuelLevel;
		} 
		else
		{
			FillLevel = total;
		}
	}
}

public class FuelTankDisplay : IFuelTankDisplay
{
	private IFuelTank fuelTank;

	internal FuelTankDisplay(IFuelTank fuelTank) => this.fuelTank = fuelTank;

	public double FillLevel => Math.Round(this.fuelTank.FillLevel, 2);

	public bool IsOnReserve => this.fuelTank.IsOnReserve;

	public bool IsComplete => this.fuelTank.IsComplete;
}

public class DrivingInformationDisplay : IDrivingInformationDisplay // car #2
{
	private IDrivingProcessor drivingProcessor;
	
	internal DrivingInformationDisplay(IDrivingProcessor drivingProcessor) => this.drivingProcessor = drivingProcessor;

	public int ActualSpeed => drivingProcessor.ActualSpeed;
}

public interface IEngineStartStop
{
	void EngineStart();
	void EngineStop();
}

public class DrivingProcessor : IDrivingProcessor // car #2
{
	private const int MaxSpeed = 250; // km/h
	private const int MinAcceleration = 5;
	private const int MaxAcceleration = 20;

	private const int MaxBraking = 10; // km/h/sec

	private IEngineStartStop engineStartStop;
	private readonly int maxAcceleration;
	private double currentSpeed;

	internal DrivingProcessor(int maxAcceleration)
	{
		this.maxAcceleration = maxAcceleration switch
		{
			< MinAcceleration => MinAcceleration,
			> MaxAcceleration => MaxAcceleration,
			_ => maxAcceleration
		};
	}

	internal void SetEngineStartStop(IEngineStartStop engineStartStop)
	{
		this.engineStartStop = engineStartStop;
	}

	public double ActualConsumption => FuelConsumptionRate.ConsumptionRateBySpeed(ActualSpeed);

	public int ActualSpeed => Convert.ToInt32(currentSpeed);

	public void EngineStart() => engineStartStop.EngineStart();
	public void EngineStop() => engineStartStop.EngineStop();

	public void IncreaseSpeedTo(int speed)
	{
		if (speed > MaxSpeed)
		{
			speed = MaxSpeed;
		}
		if (currentSpeed < MaxSpeed)
		{
			double finalSpeed = CalculateFinalVelocityFor(currentSpeed, maxAcceleration, 1);
			currentSpeed = finalSpeed <= speed ? finalSpeed : speed;
		}
	}

	public void ReduceSpeed(int speed)
	{
		if (speed > MaxBraking)
		{
			speed = MaxBraking;
		}

		double finalSpeed = CalculateFinalVelocityFor(currentSpeed, -1D * speed, 1);
		currentSpeed = finalSpeed >= 0 ? finalSpeed : 0D;
	}
	
	private static double CalculateFinalVelocityFor(double initialVelocityKmPerHour, double accelerationKmPerHourPerSecond, int seconds)
	{
		double acceleration = accelerationKmPerHourPerSecond;
		double finalVelocity =  initialVelocityKmPerHour + acceleration * seconds;
		return finalVelocity;
	}
}

public class OnBoardComputer : IOnBoardComputer, IEngineStartStop // car #3
{
	private const double SecondsPerHour = 3600D;
	private const double CentimetersPerKilometer = 100000D;

	private readonly IFuelTank fuelTank;
	private readonly IDrivingProcessor drivingProcessor;

	private Tally total;
	private Tally trip;

	internal OnBoardComputer(DrivingProcessor drivingProcessor, IFuelTank fuelTank)
	{
		this.drivingProcessor = drivingProcessor;
		drivingProcessor.SetEngineStartStop(this);
		this.fuelTank = fuelTank;
		this.total = new Tally(fuelTank);
		this.trip = new Tally(fuelTank);
	}

	public int TripRealTime => trip.Seconds;
	public int TripDrivingTime => trip.DrivingSeconds;
	public int TripDrivenDistance => trip.Distance;		// centimeters
	public int TotalRealTime => total.Seconds;
	public int TotalDrivingTime => total.DrivingSeconds;
	public int TotalDrivenDistance => total.Distance;
	public double TripAverageSpeed  => trip.TotalSpeedReadings != 0 ? Math.Round((double)trip.SumOfSpeedReadings / trip.TotalSpeedReadings, 1) : 0;
	public double TotalAverageSpeed => total.TotalSpeedReadings != 0 ? Math.Round((double)total.SumOfSpeedReadings / total.TotalSpeedReadings, 1) : 0;
	public int ActualSpeed => total.ActualSpeed;

	public double ActualConsumptionByTime
	{
		get => System.Math.Round(total.FuelUsed / (double)total.Seconds, 5);
	}

	public double ActualConsumptionByDistance
	{
		get
		{
			if (total.Distance == 0)
			{
				return double.NaN;
			}

			return System.Math.Round(total.Distance / 1000D * total.FuelUsed, 1);

//			return System.Math.Round(total.FuelUsed / total.Distance, 1);
		}
	}

	public double TripAverageConsumptionByTime
	{
		get => System.Math.Round(trip.FuelUsed / trip.Seconds, 5);
	}

	public double TotalAverageConsumptionByTime
	{
		get => System.Math.Round(total.FuelUsed / total.Seconds, 5);
	}

	public double TripAverageConsumptionByDistance
	{
		get
		{
			if (trip.Distance == 0)
			{
				return double.NaN;
			}

			return System.Math.Round(trip.FuelUsed / total.Distance, 1);
		}
	}

	public double TotalAverageConsumptionByDistance
	{
		get
		{
			// Average consumption by distance is calculated by taking the average of the values of consumption by distance every second, which is inconsistent to how average speed or other averages are calculated (total distance/total time).
			
			//I still don't see what is expected for average consumption by distance. Edit: I found out, but it's hard to explain. For each second, the average of all values is calculated again, with all previous values replaced by the cumulative average thus far.
			if (total.Distance == 0)
			{
				return double.NaN;
			}

			return System.Math.Round(total.FuelUsed / total.Distance, 1);
		}
	}

	public int EstimatedRange
	{
		get
		{
			double rateOfConsumption = (double)trip.Distance / trip.FuelUsed;		// cm per liter
			Console.Error.WriteLine("RATE=" + rateOfConsumption);
			Console.Error.WriteLine("FuelUsed=" + trip.FuelUsed);
			Console.Error.WriteLine("Distance=" + trip.Distance);
			Console.Error.WriteLine("Tank=" + fuelTank.FillLevel);
			return (int)((fuelTank.FillLevel * rateOfConsumption) / CentimetersPerKilometer);
		}
	}

	public void ElapseSecond()
	{
		trip.AddSeconds(1);
		total.AddSeconds(1);

		if (drivingProcessor.ActualSpeed > 0)
		{
			double distance = CalculateDistanceFromSpeedAndSeconds(drivingProcessor.ActualSpeed, 1);
			trip.AddDrivingSeconds(1);
			trip.AddDistance((int)Math.Round(distance * CentimetersPerKilometer, 6));
			total.AddDrivingSeconds(1);
			total.AddDistance((int)Math.Round(distance * CentimetersPerKilometer, 6));
		}
		trip.SetActualSpeed(drivingProcessor.ActualSpeed);
		total.SetActualSpeed(drivingProcessor.ActualSpeed);
	}

	private static double CalculateDistanceFromSpeedAndSeconds(double speedInKmPerHour, int seconds)
	{
		return (speedInKmPerHour * (double)seconds) / SecondsPerHour;  // km/h -> km/sec * sec
	}

	public void TripReset()
	{
		trip = new Tally(fuelTank);
	}

	public void TotalReset()
	{
		total = new Tally(fuelTank);
	}

	public void EngineStart()
	{
		trip = new Tally(fuelTank);		// "since" engine start for trip
		total.AddSeconds(1);
		trip.AddSeconds(1);
	}

	public void EngineStop()
	{
		// does nothing currently
	}
}

class Tally
{
	private readonly IFuelTank fuelTank;
	
	private double startFuelLevel;
	
	public Tally(IFuelTank fuelTank)
	{
		this.fuelTank = fuelTank;
		this.startFuelLevel = fuelTank.FillLevel;
	}
	
	public int Seconds
	{
		get;
		private set;
	}

	public void AddSeconds(int seconds)
	{
		Seconds += seconds;
	}
	
	public int DrivingSeconds
	{
		get;
		private set;
	}

	public void AddDrivingSeconds(int seconds)
	{
		DrivingSeconds += seconds;
	}

	public int Distance
	{
		get;
		private set;
	}

	public void AddDistance(int distance)
	{
		Distance += distance;
	}

	public int TotalSpeedReadings
	{
		get;
		private set;
	}

	public long SumOfSpeedReadings
	{
		get;
		private set;
	}

	public int ActualSpeed
	{
		get; 
		private set;
	}

	public void SetActualSpeed(int speed)
	{
		ActualSpeed = speed;
		if (speed != 0)
		{
			SumOfSpeedReadings += speed;
			++TotalSpeedReadings;
		}
	}

	public double FuelUsed
	{
		get
		{
			return startFuelLevel - fuelTank.FillLevel;
		}
	}
}

public class OnBoardComputerDisplay : IOnBoardComputerDisplay // car #3
{
	private const double CentimetersPerKilometer = 100000D;
	
	private IOnBoardComputer onBoardComputer;
	
	internal OnBoardComputerDisplay(IOnBoardComputer onBoardComputer) => this.onBoardComputer = onBoardComputer;

	public int TripRealTime => onBoardComputer.TripRealTime;
	public int TripDrivingTime => onBoardComputer.TripDrivingTime;
	public double TripDrivenDistance => Math.Round(onBoardComputer.TripDrivenDistance / CentimetersPerKilometer, 2);
	public int TotalRealTime => onBoardComputer.TotalRealTime;
	public int TotalDrivingTime => onBoardComputer.TotalDrivingTime;
	public double TotalDrivenDistance => Math.Round(onBoardComputer.TotalDrivenDistance / CentimetersPerKilometer, 2);
	public int ActualSpeed => onBoardComputer.ActualSpeed;
	public double TripAverageSpeed => onBoardComputer.TripAverageSpeed;
	public double TotalAverageSpeed => onBoardComputer.TotalAverageSpeed;
	public double ActualConsumptionByTime => onBoardComputer.ActualConsumptionByTime;
	public double ActualConsumptionByDistance => onBoardComputer.ActualConsumptionByDistance;
	public double TripAverageConsumptionByTime { get; }
	public double TotalAverageConsumptionByTime { get; }
	public double TripAverageConsumptionByDistance => onBoardComputer.TripAverageConsumptionByDistance;
	public double TotalAverageConsumptionByDistance => onBoardComputer.TotalAverageConsumptionByDistance;
	public int EstimatedRange => onBoardComputer.EstimatedRange;
	public void TripReset()
	{
		onBoardComputer.TripReset();
	}

	public void TotalReset()
	{
		onBoardComputer.TotalReset();
	}
}