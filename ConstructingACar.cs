namespace CodewarsConstructingACar;

using System;
	
public class Car : ICar
{
	private const int DefaultMaxAcceleration = 10;		// km/h/sec

	public IFuelTankDisplay fuelTankDisplay;

	private IEngine engine;

	private IFuelTank fuelTank;
	
	public IDrivingInformationDisplay drivingInformationDisplay; // car #2  

	private IDrivingProcessor drivingProcessor; // car #2

	public Car(double fuelLevel, int maxAcceleration) // car #2
	{
		fuelTank = new FuelTank(fuelLevel);
		engine = new Engine(fuelTank);
		fuelTankDisplay = new FuelTankDisplay(fuelTank);
		drivingProcessor = new DrivingProcessor(maxAcceleration);
		drivingInformationDisplay = new DrivingInformationDisplay(drivingProcessor);
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
			}
		}
	}

	private void ConsumeGas()
	{
		engine.Consume(FuelConsumptionRate.ConsumptionRateBySpeed(drivingInformationDisplay.ActualSpeed));
		if (fuelTank.FillLevel <= 0)
		{
			EngineStop();
		}
	}

	public void EngineStart() => this.engine.Start();

	public void EngineStop() => this.engine.Stop();
	
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
		engine.Consume(FuelConsumptionRate.ConsumptionRateBySpeed(0));
		if (fuelTank.FillLevel == 0)
		{
			engine.Stop();
		}
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

public class DrivingProcessor : IDrivingProcessor // car #2
{
	private const int MaxSpeed = 250;		// km/h
	private const int MinAcceleration = 5;
	private const int MaxAcceleration = 20;
	
	private const int MaxBraking = 10;		// km/h/sec

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

	public int ActualSpeed => Convert.ToInt32(currentSpeed);

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