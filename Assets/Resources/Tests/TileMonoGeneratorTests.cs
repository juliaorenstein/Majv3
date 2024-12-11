using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Resources
{
	public class TileMonoGeneratorTests
	{
		[UnityTest]
		public IEnumerator Start_WhenCalled_GeneratesTiles()
		{
			SceneManager.LoadScene("Scene");
			yield return null;
			
			int numTiles = GameObject.Find("Pool").transform.childCount;
			
			Assert.AreEqual(152, numTiles);
		}
	}
}