using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Resources.ClientTests
{
    public class SetupMonoTests : MonoBehaviour
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene("Assets/Scenes/Scene.unity");
            yield return null;
        }
		
        [Test]
        public void Start_WhenCalled_GeneratesTiles()
        {
            GameObject.Find("GameManager").GetComponent<SetupMono>().StartGame(new());
            
            int numTiles = GameObject.Find("Pool").transform.childCount;
			
            Assert.AreEqual(152, numTiles);
        }
    }
}
