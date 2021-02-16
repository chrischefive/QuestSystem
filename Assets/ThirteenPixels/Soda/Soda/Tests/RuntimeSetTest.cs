
namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class RuntimeSetTest
    {
        private class RuntimeSetVisibleObjects : RuntimeSetBase<RuntimeSetVisibleObjects.Element>
        {
            public struct Element
            {
                public readonly GameObject gameObject;
                public readonly MeshRenderer renderer;
                public readonly MeshFilter filter;

                public Element(GameObject gameObject, MeshRenderer renderer, MeshFilter filter)
                {
                    this.gameObject = gameObject;
                    this.renderer = renderer;
                    this.filter = filter;
                }
            }

            // We use the default reflection-based implementation here for testing.
            /*
            protected override bool TryCreateElement(GameObject gameObject, out Components element)
            {
                element = new Components(gameObject.GetComponent<MeshRenderer>(),
                                                gameObject.GetComponent<MeshFilter>());
                return element.renderer && element.filter;
            }
            */
        }

        private RuntimeSetVisibleObjects runtimeSet;
        private List<GameObject> objects;

        [SetUp]
        public void SetUp()
        {
            runtimeSet = ScriptableObject.CreateInstance<RuntimeSetVisibleObjects>();
            objects = new List<GameObject>();
            objects.Add(new GameObject("Test GameObject 1"));
            objects.Add(new GameObject("Test GameObject 2"));
            objects.Add(new GameObject("Test GameObject 3"));
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(runtimeSet);

            foreach (var obj in objects)
            {
                Object.DestroyImmediate(obj);
            }
            objects.Clear();
        }

        [Test]
        public void AddCorrectGameObjects()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();
                
                var success = runtimeSet.Add(obj);
                Assert.IsTrue(success);
            }

            Assert.AreEqual(3, runtimeSet.elementCount);

            var bits = 0;
            runtimeSet.ForEach(item =>
            {
                var index = objects.IndexOf(item.gameObject);
                Assert.GreaterOrEqual(index, 0);
                bits |= 1 << index;
            });
            Assert.AreEqual(7, bits);
        }

        [Test]
        public void AddWrongGameObjects()
        {
            LogAssert.ignoreFailingMessages = true;

            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();

                var success = runtimeSet.Add(obj);
                Assert.IsFalse(success);
            }

            Assert.AreEqual(0, runtimeSet.elementCount);
        }

        [Test]
        public void RemoveGameObjects()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSet.Add(obj);
            }

            var count = 3;
            foreach (var obj in objects)
            {
                runtimeSet.Remove(obj);
                count--;
                Assert.AreEqual(count, runtimeSet.elementCount);
            }

        }

        [Test]
        public void OnElementCountChangeEvent()
        {
            var count = 0;
            runtimeSet.onElementCountChange.AddResponse(newCount => count = newCount);


            var i = 0;
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSet.Add(obj);
                i++;
                Assert.AreEqual(i, count);
            }

            foreach (var obj in objects)
            {
                runtimeSet.Remove(obj);
                i--;
                Assert.AreEqual(i, count);
            }
        }
    }
}
