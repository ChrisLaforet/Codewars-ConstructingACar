namespace CodewarsConstructingACar;

using System;
	
public class Car : ICar
{
	public IFuelTankDisplay fuelTankDisplay;

	private IEngine engine;

	private IFuelTank fuelTank;

	public Car() : this(FuelTank.DefaultFuelLevel) { }

	public Car(double fuelLevel)
	{
		fuelTank = new FuelTank(fuelLevel);
		engine = new Engine(fuelTank);
		fuelTankDisplay = new FuelTankDisplay(fuelTank);
	}

	public bool EngineIsRunning => this.engine.IsRunning;

	public void EngineStart() => this.engine.Start();

	public void EngineStop() => this.engine.Stop();

	public void Refuel(double liters) => this.fuelTank.Refuel(liters);

	public void RunningIdle()
	{
    if (!EngineIsRunning)
		{
			return;
		}
		engine.Consume(Engine.IdleConsumptionRate);
		if (fuelTank.FillLevel == 0)
		{
			engine.Stop();
		}
	}
}

public class Engine : IEngine
{
	public const double IdleConsumptionRate = 0.0003D;

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
		if (fuelLevel <= 0)
		{
			this.FillLevel = 0;
		}
		else if (fuelLevel > MaximumFuelLevel)
		{
			this.FillLevel = MaximumFuelLevel;
		}
		else
		{
			this.FillLevel = fuelLevel;
		}
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
		if (FillLevel <= 0)
		{
			return;
		}
		FillLevel = FillLevel - liters;
		if (FillLevel < 0)
		{
			FillLevel = 0;
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