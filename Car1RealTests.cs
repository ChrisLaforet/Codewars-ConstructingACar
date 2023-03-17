namespace CodewarsConstructingACar;

using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
public class Car1RealTests
{
  [Test] 
  public void TestMotorStartAndStop()
  {
    var car = new Car();

    Assert.IsFalse(car.EngineIsRunning, "Engine could not be running.");

    car.EngineStart();

    Assert.IsTrue(car.EngineIsRunning, "Engine should be running.");

    car.EngineStop();

    Assert.IsFalse(car.EngineIsRunning, "Engine could not be running.");
  }

  [Test]
  public void TestMotorStartAgainAndStopAgain()
  {
    var car = new Car();

    car.EngineStart();

    car.EngineStart();

    Assert.IsTrue(car.EngineIsRunning, "Engine should be running.");

    car.EngineStop();

    car.EngineStop();

    Assert.IsFalse(car.EngineIsRunning, "Engine could not be running.");
  }

  [Test]
  public void TestMotorDoesntStartWithEmptyFuelTank()
  {
    var car = new Car(0);

    car.EngineStart();

    Assert.IsFalse(car.EngineIsRunning, "Engine could not be running.");
  }

  [Test]
  public void TestFuelConsumptionOnIdle()
  {
    var car = new Car(1);

    car.EngineStart();

    Enumerable.Range(0, 3000).ToList().ForEach(s => car.RunningIdle());

    Assert.AreEqual(0.10, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
  }

  [Test]
  public void TestEngineStopsCauseOfNoFuelExactly()
  {
    var car = new Car(6);

    car.EngineStart();

    Enumerable.Range(0, 20000).ToList().ForEach(s => car.RunningIdle());

    Assert.IsFalse(car.EngineIsRunning, "Engine could not be running.");
  }

  [Test]
  public void TestEngineStopsCauseOfNoFuelOver()
  {
    var car = new Car(1);

    car.EngineStart();

    Enumerable.Range(0,10000).ToList().ForEach(s => car.RunningIdle());
        
    Assert.IsFalse(car.EngineIsRunning, "Engine could not be running.");
  }

  [Test]
  public void TestNoConsumptionWhenEngineNotRunning()
  {
    var car = new Car(1);

    Enumerable.Range(0, 1000).ToList().ForEach(s => car.RunningIdle());

    Assert.AreEqual(1, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
  }

  [Test]
  public void TestFuelTankDisplayIsNotComplete()
  {
    var car = new Car();

    Assert.IsFalse(car.fuelTankDisplay.IsComplete, "Fuel tank must be not complete!");
  }

  [Test]
  public void TestFuelTankDisplayIsComplete()
  {
    var car = new Car(60);

    Assert.IsTrue(car.fuelTankDisplay.IsComplete, "Fuel tank must be complete!");
  }

  [Test]
  public void TestFuelTankDisplayIsNotOnReserve()
  {
    var car = new Car();

    Assert.IsFalse(car.fuelTankDisplay.IsOnReserve, "Fuel tank must be not on reserve!");
  }

  [Test]
  public void TestFuelTankDisplayIsOnReserve()
  {
    var car = new Car(4);

    Assert.IsTrue(car.fuelTankDisplay.IsOnReserve, "Fuel tank must be on reserve!");
  }

  [Test]
  public void TestRefuel()
  {
    var car = new Car(5);

    car.Refuel(40);

    Assert.AreEqual(45, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
  }

  [Test]
  public void TestRefuelOverMaximum()
  {
    var car = new Car(5);

    car.Refuel(80);

    Assert.AreEqual(60, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
  }

  [Test]
  public void TestNoNegativeFuelLevelAllowed()
  {
    var car = new Car(-5);

    Assert.AreEqual(0, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
  }

  [Test]
  public void TestFuelLevelAllowedUpTo60()
  {
    var car = new Car(65);
     
    Assert.AreEqual(60, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
  }

  [Test]
  public void Car1RandomTests()
  {
    var rand = new Random();

    for (int i = 0; i < 20; i++)
    {
      var car1 = new Car(5);

      var refuelLiter = rand.Next(60);

      car1.Refuel(refuelLiter);

      double expectedFuelLevel = Math.Min(5 + refuelLiter, 60);
      Assert.AreEqual(expectedFuelLevel, car1.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
               
      var car2 = new Car(5);
      car2.EngineStart();
                
      var runningIdleSeconds = rand.Next(7);
                
      Enumerable.Range(0, runningIdleSeconds * 10001 / 3).ToList().ForEach(s => { car2.RunningIdle(); });

      expectedFuelLevel = Math.Max(5 - runningIdleSeconds, 0);
                
      Assert.AreEqual(expectedFuelLevel, car2.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
      if(expectedFuelLevel == 0)
      {                    
        Assert.IsFalse(car2.EngineIsRunning, "Engine could not be running.");
      }
    }
  }
}