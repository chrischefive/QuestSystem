
namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class ScopedVariableTest
    {
        private GlobalInt globalInt;
        private ScopedInt scopedInt;

        [SetUp]
        public void SetUp()
        {
            scopedInt = new ScopedInt(0);
            globalInt = ScriptableObject.CreateInstance<GlobalInt>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(globalInt);
        }

        [Test]
        public void Constructor()
        {
            scopedInt = new ScopedInt(11);
            Assert.AreEqual(11, scopedInt.value);
        }

        [Test]
        public void ValueChange()
        {
            scopedInt.value = 42;
            Assert.AreEqual(42, scopedInt.value);
        }

        [Test]
        public void ScopeChange()
        {
            globalInt.value = 42;
            scopedInt.AssignGlobalVariable(globalInt);
            Assert.AreEqual(42, scopedInt.value);
        }

        [Test]
        public void OnChangeEvent()
        {
            globalInt.value = 42;
            scopedInt.AssignGlobalVariable(globalInt);
            var result = 0;

            scopedInt.onChangeValue.AddResponseAndInvoke(i => result = i);
            Assert.AreEqual(42, result);

            globalInt.value = 1337;
            Assert.AreEqual(1337, result);

            scopedInt.AssignLocalValue(420);
            Assert.AreEqual(420, result);

            globalInt.value = 123456;
            Assert.AreEqual(420, result);
        }

        [Test]
        public void SettingOldValueDoesntTriggerEvent()
        {
            var count = 0;
            scopedInt.onChangeValue.AddResponse(i => count++);

            scopedInt.value = 42;
            scopedInt.value = 42;

            Assert.AreEqual(1, count);
        }

        [Test]
        public void NullValues()
        {
            var globalString = ScriptableObject.CreateInstance<GlobalString>();
            var scopedString = new ScopedString("not null");

            globalString.value = "totally not null";

            // With a local value
            Assert.DoesNotThrow(() =>
            {
                scopedString.value = null;
            });
            Assert.AreEqual(null, scopedString.value);

            Assert.DoesNotThrow(() =>
            {
                scopedString.value = "not null again";
            });
            Assert.AreEqual("not null again", scopedString.value);

            // As a GlobalVariable proxy
            scopedString.AssignGlobalVariable(globalString);
            Assert.DoesNotThrow(() =>
            {
                scopedString.value = null;
            });
            Assert.AreEqual(null, scopedString.value);

            Assert.DoesNotThrow(() =>
            {
                scopedString.value = "not null again";
            });
            Assert.AreEqual("not null again", scopedString.value);

            Object.DestroyImmediate(globalString);
        }
    }
}
