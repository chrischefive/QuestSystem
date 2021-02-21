
namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class GlobalVariableTest
    {
        private GlobalInt globalInt;

        [SetUp]
        public void SetUp()
        {
            globalInt = ScriptableObject.CreateInstance<GlobalInt>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(globalInt);
        }

        [Test]
        public void ValueChange()
        {
            globalInt.value = 42;
            Assert.AreEqual(42, globalInt.value);
            globalInt.value = 1337;
            Assert.AreEqual(1337, globalInt.value);
        }

        [Test]
        public void AddResponse()
        {
            globalInt.value = 42;
            var result = 0;

            globalInt.onChange.AddResponse(i => result = i);
            Assert.AreEqual(0, result);

            globalInt.value = 1337;
            Assert.AreEqual(1337, result);
        }

        [Test]
        public void ValueIsUpdatedBeforeResponseInvocation()
        {
            globalInt.value = 42;
            var result = 0;

            globalInt.onChange.AddResponse(i => result = globalInt.value);

            globalInt.value = 1337;
            Assert.AreEqual(1337, result);
        }

        /// <summary>
        /// Tests whether the callbackParameter function is hooked up correctly so that AddResponseAndInvoke invokes the response with the current value.
        /// </summary>
        [Test]
        public void AddResponseAndInvoke()
        {
            globalInt.value = 42;
            var result = 0;

            globalInt.onChange.AddResponseAndInvoke(i => result = i);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void SettingOldValueDoesntTriggerEvent()
        {
            var count = 0;
            globalInt.onChange.AddResponse(i => count++);

            globalInt.value = 42;
            globalInt.value = 42;

            Assert.AreEqual(1, count);
        }

        [Test]
        public void NullValues()
        {
            var globalString = ScriptableObject.CreateInstance<GlobalString>();

            globalString.value = "not null";
            Assert.AreEqual("not null", globalString.value);

            Assert.DoesNotThrow(() =>
            {
                globalString.value = null;
            });
            Assert.AreEqual(null, globalString.value);

            Assert.DoesNotThrow(() =>
            {
                globalString.value = "not null again";

            });
            Assert.AreEqual("not null again", globalString.value);

            Object.DestroyImmediate(globalString);
        }
    }
}
