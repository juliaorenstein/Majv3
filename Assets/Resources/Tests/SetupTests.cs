using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;
using Fusion;
using UnityEngine;

namespace Resources
{
	public class SetupTests
	{
		[UnitySetUp]
		public IEnumerator Setup()
		{
			SceneManager.LoadScene("Assets/Scenes/Scene.unity");
			yield return null;
		}
		
		[Test]
		public void LoadScene_NoExceptions()
		{
			// no test code here - test will fail if there's an exception during Setup
			Assert.AreEqual(1, 1);
		}

		[Test]
		public void StartGameFromFusion_SinglePlayer_NoExceptions()
		{
			FusionBootstrap bootstrap = GameObject.Find("Network Start").GetComponent<FusionBootstrap>();
			bootstrap.StartMode = FusionBootstrap.StartModes.Manual;
			bootstrap.StartSinglePlayer();
		}
		
		[Test]
		public void StartGameFromFusion_HostMode_NoExceptions()
		{
			FusionBootstrap bootstrap = GameObject.Find("Network Start").GetComponent<FusionBootstrap>();
			bootstrap.StartMode = FusionBootstrap.StartModes.Manual;
			bootstrap.StartHost();
		}
		
		[Test]
		public void StartGameFromFusion_ClientMode_NoExceptions()
		{
			// don't know how to test this yet
		}
	}
}