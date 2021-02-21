
namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class SodaEventTest
    {
        private SodaEvent sodaEvent;
        private SodaEvent<int> sodaEventInt;
        private int callbackValue;

        [SetUp]
        public void SetUp()
        {
            sodaEvent = new SodaEvent();
            sodaEventInt = new SodaEvent<int>(() => callbackValue);
            callbackValue = 0;
        }

        [Test]
        public void Invocation()
        {
            var result = false;

            sodaEvent.AddResponse(() => result = true);
            Assert.AreEqual(false, result);

            sodaEvent.Invoke();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void MultiInvocation()
        {
            var result = 0;
            sodaEvent.AddResponse(() => result++);
            sodaEvent.AddResponse(() => result += 2);
            sodaEvent.AddResponse(() => result += 5);
            sodaEvent.Invoke();

            Assert.AreEqual(8, result);
        }

        [Test]
        public void ResponseRemoval()
        {
            var result = 0;
            System.Action response = () => result += 3;

            sodaEvent.AddResponse(() => result += 2);
            sodaEvent.AddResponse(response);
            sodaEvent.AddResponse(() => result += 4);
            sodaEvent.Invoke();
            sodaEvent.RemoveResponse(response);
            sodaEvent.Invoke();

            Assert.AreEqual(15, result);
        }

        [Test]
        public void ParameterInvocation()
        {
            var result = 0;
            sodaEventInt.AddResponse(param => result += param);
            sodaEventInt.Invoke(5);
            sodaEventInt.Invoke(10);

            Assert.AreEqual(15, result);
        }

        [Test]
        public void AddResponseAndInvoke()
        {
            callbackValue = 10;
            var error = false;

            // This response should not be triggered by the upcoming AddResponseAndInvoke
            sodaEventInt.AddResponse(i => error = true);

            sodaEventInt.AddResponseAndInvoke(param => callbackValue += param);

            Assert.AreEqual(20, callbackValue);
            Assert.AreEqual(false, error);
        }

        [Test]
        public void RecursiveInvocation()
        {
            var result = 0;
            sodaEvent.AddResponse(() =>
            {
                result++;
                if (result < 10)
                {
                    sodaEvent.Invoke();
                }
            });
            sodaEvent.Invoke();

            Assert.AreEqual(10, result);
        }

        [Test]
        public void ExceptionInResponse()
        {
            LogAssert.ignoreFailingMessages = true;

            var result = 0;
            sodaEvent.AddResponse(() => result += 10);
            sodaEvent.AddResponse(() => throw new System.Exception("This exception is meant to be thrown."));
            sodaEvent.AddResponse(() => result += 20);
            sodaEvent.Invoke();

            Assert.AreEqual(30, result);
        }
    }
}
