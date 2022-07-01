using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace ItsReactive.Tests;

[TestClass]
public class SimpleReactiveTests
{
    [TestMethod]
    public void Delay()
    {
        var subject = new Subject<int>();
        var scheduler = new TestScheduler();

        scheduler.Schedule(() => subject.OnNext(1));
        scheduler.Schedule(TimeSpan.FromTicks(100), () => subject.OnNext(2));
        scheduler.Schedule(TimeSpan.FromTicks(200), () => subject.OnNext(3));
        scheduler.Schedule(TimeSpan.FromTicks(300), () => subject.OnNext(4));

        var endResult = scheduler.CreateObserver<int>();
        var startResult = scheduler.CreateObserver<int>();
      
        subject.Subscribe(startResult);
        
        subject
            .Delay(TimeSpan.FromTicks(100), scheduler)
            .Where(x =>
            {
                if (x > 2)
                {
                    var sub = new BehaviorSubject<int>(x + 100)
                        .Delay(TimeSpan.FromTicks(100), scheduler)
                        .Take(1)
                        .Subscribe(endResult);
                }

                return x <= 2;
            })
            .Subscribe(endResult);

        scheduler.Start();
        //scheduler.AdvanceTo(210);
        
        endResult.Messages.Count.ShouldBe(2);

    }
}