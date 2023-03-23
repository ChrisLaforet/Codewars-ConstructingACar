namespace CodewarsConstructingACar;

using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
public class Car3RealTests
{
    [Test]
    public void TestRealAndDrivingTimeBeforeStarting()
    {
        var car = new Car();

        Assert.AreEqual(0, car.onBoardComputerDisplay.TripRealTime, "Wrong Trip-Real-Time!");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TripDrivingTime, "Wrong Trip-Driving-Time!");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TotalRealTime, "Wrong Total-Real-Time!");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TotalDrivingTime, "Wrong Total-Driving-Time!");
    }

    [Test]
    public void TestRealAndDrivingTimeAfterDriving()
    {
        var car = new Car();

        car.EngineStart();

        car.RunningIdle();
        car.RunningIdle();

        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);

        car.BrakeBy(10);
        car.BrakeBy(10);

        car.Accelerate(30);

        Assert.AreEqual(11, car.onBoardComputerDisplay.TripRealTime, "Wrong Trip-Real-Time!");
        Assert.AreEqual(8, car.onBoardComputerDisplay.TripDrivingTime, "Wrong Trip-Driving-Time!");
        Assert.AreEqual(11, car.onBoardComputerDisplay.TotalRealTime, "Wrong Total-Real-Time!");
        Assert.AreEqual(8, car.onBoardComputerDisplay.TotalDrivingTime, "Wrong Total-Driving-Time!");
    }

    [Test]
    public void TestActualSpeedBeforeDriving()
    {
        var car = new Car();

        car.EngineStart();

        car.RunningIdle();
        car.RunningIdle();

        Assert.AreEqual(0, car.onBoardComputerDisplay.ActualSpeed, "Wrong actual speed.");
    }

    [Test]
    public void TestAverageSpeed1()
    {
        var car = new Car();

        car.EngineStart();

        car.RunningIdle();
        car.RunningIdle();

        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);

        car.BrakeBy(10);
        car.BrakeBy(10);
        car.BrakeBy(10);

        Assert.AreEqual(18, car.onBoardComputerDisplay.TripAverageSpeed, "Wrong Trip-Average-Speed.");
        Assert.AreEqual(18, car.onBoardComputerDisplay.TotalAverageSpeed, "Wrong Total-Average-Speed.");
    }    

    [Test]
    public void TestAverageSpeedAfterTripReset()
    {
        var car = new Car();

        car.EngineStart();

        car.RunningIdle();
        car.RunningIdle();

        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);

        car.BrakeBy(10);
        car.BrakeBy(10);
        car.BrakeBy(10);

        car.onBoardComputerDisplay.TripReset();

        car.Accelerate(20);
        
        car.Accelerate(20);

        Assert.AreEqual(15, car.onBoardComputerDisplay.TripAverageSpeed, "Wrong Trip-Average-Speed.");
        Assert.AreEqual(20, car.onBoardComputerDisplay.TotalAverageSpeed, "Wrong Total-Average-Speed.");
    }

    [Test]
    public void TestActualConsumptionEngineStart()
    {
        var car = new Car();

        car.EngineStart();

        Assert.AreEqual(0, car.onBoardComputerDisplay.ActualConsumptionByTime, "Wrong Actual-Consumption-By-Time");
        Assert.AreEqual(double.NaN, car.onBoardComputerDisplay.ActualConsumptionByDistance, "Wrong Actual-Consumption-By-Distance");
    }

    [Test]
    public void TestActualConsumptionRunningIdle()
    {
        var car = new Car();

        car.EngineStart();

        car.RunningIdle();

        Assert.AreEqual(0.0003, car.onBoardComputerDisplay.ActualConsumptionByTime, "Wrong Actual-Consumption-By-Time");
        Assert.AreEqual(double.NaN, car.onBoardComputerDisplay.ActualConsumptionByDistance, "Wrong Actual-Consumption-By-Distance");
    }

    [Test]
    public void TestActualConsumptionAccelerateTo100()
    {
        var car = new Car(40, 20);

        car.EngineStart();

        car.Accelerate(100);
        car.Accelerate(100);
        car.Accelerate(100);
        car.Accelerate(100);
        car.Accelerate(100);

        Assert.AreEqual(5, car.onBoardComputerDisplay.ActualConsumptionByDistance, "Wrong Actual-Consumption-By-Distance");
        Assert.AreEqual(0.0014, car.onBoardComputerDisplay.ActualConsumptionByTime, "Wrong Actual-Consumption-By-Time");
    }

    [Test]
    public void TestActualConsumptionFreeWheel()
    {
        var car = new Car(40, 20);

        car.EngineStart();

        car.Accelerate(100);
        car.Accelerate(100);
        car.Accelerate(100);
        car.Accelerate(100);
        car.Accelerate(100);

        car.FreeWheel();

        Assert.AreEqual(0, car.onBoardComputerDisplay.ActualConsumptionByTime, "Wrong Actual-Consumption-By-Time");
        Assert.AreEqual(0, car.onBoardComputerDisplay.ActualConsumptionByDistance, "Wrong Actual-Consumption-By-Distance");
    }

    [Test]
    public void TestAverageConsumptionsAfterEngineStart()
    {
        var car = new Car();

        car.EngineStart();

        Assert.AreEqual(0, car.onBoardComputerDisplay.TripAverageConsumptionByTime, "Wrong Trip-Average-Consumption-By-Time");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TotalAverageConsumptionByTime, "Wrong Total-Average-Consumption-By-Time");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TripAverageConsumptionByDistance, "Wrong Trip-Average-Consumption-By-Distance");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TotalAverageConsumptionByDistance, "Wrong Total-Average-Consumption-By-Distance");
    }

    [Test]
    public void TestAverageConsumptionsAfterAccelerating()
    {
        var car = new Car();

        car.EngineStart();

        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);
        
        Assert.AreEqual(0.0015, car.onBoardComputerDisplay.TripAverageConsumptionByTime, "Wrong Trip-Average-Consumption-By-Time");
        Assert.AreEqual(0.0015, car.onBoardComputerDisplay.TotalAverageConsumptionByTime, "Wrong Total-Average-Consumption-By-Time");
        Assert.AreEqual(44, car.onBoardComputerDisplay.TripAverageConsumptionByDistance, "Wrong Trip-Average-Consumption-By-Distance");
        Assert.AreEqual(44, car.onBoardComputerDisplay.TotalAverageConsumptionByDistance, "Wrong Total-Average-Consumption-By-Distance");
    }
    
    [Test]
    public void TestAverageConsumptionsAfterBraking()
    {
        var car = new Car();

        car.EngineStart();

        car.Accelerate(30);
        car.Accelerate(30);
        car.Accelerate(30);

        car.BrakeBy(10);
        car.BrakeBy(10);
        car.BrakeBy(10);

        Assert.AreEqual(0.0009, car.onBoardComputerDisplay.TripAverageConsumptionByTime, "Wrong Trip-Average-Consumption-By-Time");
        Assert.AreEqual(0.0009, car.onBoardComputerDisplay.TotalAverageConsumptionByTime, "Wrong Total-Average-Consumption-By-Time");
        Assert.AreEqual(26.4, car.onBoardComputerDisplay.TripAverageConsumptionByDistance, "Wrong Trip-Average-Consumption-By-Distance");
        Assert.AreEqual(26.4, car.onBoardComputerDisplay.TotalAverageConsumptionByDistance, "Wrong Total-Average-Consumption-By-Distance");
    }

    [Test]
    public void TestDrivenDistancesAfterEngineStart()
    {
        var car = new Car();

        car.EngineStart();

        Assert.AreEqual(0, car.onBoardComputerDisplay.TripDrivenDistance, "Wrong Trip-Driven-Distance.");
        Assert.AreEqual(0, car.onBoardComputerDisplay.TotalDrivenDistance, "Wrong Total-Driven-Distance.");
    }

    [Test]
    public void TestDrivenDistancesAfterAccelerating()
    {
        var car = new Car();

        car.EngineStart();

        Enumerable.Range(0, 30).ToList().ForEach(c => car.Accelerate(30));

        Assert.AreEqual(0.24, car.onBoardComputerDisplay.TripDrivenDistance, "Wrong Trip-Driven-Distance.");
        Assert.AreEqual(0.24, car.onBoardComputerDisplay.TotalDrivenDistance, "Wrong Total-Driven-Distance.");
    }   
   
    [Test]
    public void TestEstimatedRangeAfterDrivingOptimumSpeedForMoreThan100Seconds()
    {
        var car = new Car();

        car.EngineStart();

        Enumerable.Range(0, 150).ToList().ForEach(c => car.Accelerate(100));
       
        Assert.AreEqual(393, car.onBoardComputerDisplay.EstimatedRange, "Wrong Estimated-Range.");
    }
}
