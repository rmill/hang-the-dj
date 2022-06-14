using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    private GameObject testObject;
    private PlayerController player;
    private int startingHealth;

    [SetUp]
    public void Setup()
    {
        testObject = GameObject.Instantiate(new GameObject());
        player = testObject.AddComponent<PlayerController>();
    }

    [UnityTest]
    public IEnumerator StartingHealthIsGreaterThanZero()
    {
        var startingHealth = player.health;
        Assert.Greater(startingHealth, 0);
        yield return null;
    }

    [UnityTest]
    public IEnumerator HealthCanBeReduced()
    {
        var amount = -1;
        var startingHealth = player.health;
        player.ChangeHealth(amount);
        yield return null;
        Assert.AreEqual(startingHealth + amount, player.health);
    }

    [UnityTest]
    public IEnumerator HealthCannotBeReducedPastZero()
    {
        var startingHealth = player.health;
        var amount = startingHealth * -10;
        player.ChangeHealth(amount);
        yield return null;
        Assert.AreEqual(0, player.health);
    }

    [UnityTest]
    public IEnumerator HealthCanBeIncreased()
    {
        var amount = 1;
        var startingHealth = player.health;
        player.ChangeHealth(startingHealth * -1);
        player.ChangeHealth(amount);
        yield return null;
        Assert.AreEqual(1, player.health);
    }

    [UnityTest]
    public IEnumerator HealthCannotBeIncreasedPastMax()
    {
        var startingHealth = player.health;
        player.ChangeHealth(100);
        yield return null;
        Assert.AreEqual(startingHealth, player.health);
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(testObject);
    }
}
