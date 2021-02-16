
namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class GlobalGameObjectTest
    {
        private class GlobalVisibleGameObject : GlobalGameObjectWithComponentCacheBase<GlobalVisibleGameObject.Components>
        {
            public struct Components
            {
                public readonly MeshRenderer renderer;
                public readonly MeshFilter filter;

                public Components(MeshRenderer renderer, MeshFilter filter)
                {
                    this.renderer = renderer;
                    this.filter = filter;
                }
            }

            // We use the default reflection-based implementation here for testing.
            /*
            protected override bool TryCreateComponentCache(GameObject gameObject, out Components componentCache)
            {
                componentCache = new Components(gameObject.GetComponent<MeshRenderer>(),
                                                gameObject.GetComponent<MeshFilter>());
                return componentCache.renderer && componentCache.filter;
            }
            */
        }

        private GlobalVisibleGameObject globalGameObject;
        private GameObject targetGameObject;

        [SetUp]
        public void SetUp()
        {
            globalGameObject = ScriptableObject.CreateInstance<GlobalVisibleGameObject>();
            targetGameObject = new GameObject("Test GameObject");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(globalGameObject);
            Object.DestroyImmediate(targetGameObject);
        }

        [Test]
        public void SetCorrectGameObject()
        {
            targetGameObject.AddComponent<MeshRenderer>();
            targetGameObject.AddComponent<MeshFilter>();

            Assert.DoesNotThrow(() => globalGameObject.value = targetGameObject);

            Assert.AreSame(targetGameObject, globalGameObject.value);

            Assert.AreSame(targetGameObject.GetComponent<MeshRenderer>(), globalGameObject.componentCache.renderer);
            Assert.AreSame(targetGameObject.GetComponent<MeshFilter>(), globalGameObject.componentCache.filter);
        }

        [Test]
        public void SetCorrectGameObjectWithTrySetValue()
        {
            targetGameObject.AddComponent<MeshRenderer>();
            targetGameObject.AddComponent<MeshFilter>();

            var success = globalGameObject.TrySetValue(targetGameObject);
            Assert.IsTrue(success);
            Assert.AreSame(targetGameObject, globalGameObject.value);

            Assert.AreSame(targetGameObject.GetComponent<MeshRenderer>(), globalGameObject.componentCache.renderer);
            Assert.AreSame(targetGameObject.GetComponent<MeshFilter>(), globalGameObject.componentCache.filter);
        }

        [Test]
        public void SetWrongGameObject()
        {
            targetGameObject.AddComponent<MeshRenderer>();

            Assert.Throws<System.Exception>(() => globalGameObject.value = targetGameObject);

            Assert.IsNull(globalGameObject.value);
        }

        [Test]
        public void SetNull()
        {
            Assert.IsNull(globalGameObject.value);

            SetCorrectGameObject();

            globalGameObject.value = null;
            Assert.IsNull(globalGameObject.value);
        }

        [Test]
        public void SetWrongGameObjectWithTrySetValue()
        {
            targetGameObject.AddComponent<MeshRenderer>();

            var success = globalGameObject.TrySetValue(targetGameObject);
            Assert.IsFalse(success);
            Assert.IsNull(globalGameObject.value);
        }

        [Test]
        public void OnChangeComponentCache()
        {
            var renderer = targetGameObject.AddComponent<MeshRenderer>();

            var success = false;
            var cacheRenderer = default(MeshRenderer);

            globalGameObject.onChangeComponentCache.AddResponse(cache =>
            {
                success = true;
                cacheRenderer = cache.renderer;
            });
            Assert.False(globalGameObject.TrySetValue(targetGameObject));
            Assert.False(success);
            Assert.AreEqual(null, cacheRenderer);

            targetGameObject.AddComponent<MeshFilter>();

            Assert.True(globalGameObject.TrySetValue(targetGameObject));
            Assert.True(success);
            Assert.AreEqual(renderer, cacheRenderer);
        }
    }
}
