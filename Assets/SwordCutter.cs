using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]

public class SwordCutter : MonoBehaviour
{

	public Material capMaterial;
	public Material orangeCapMaterial;
	public Material blueCapMaterial;
	public Material greenCapMaterial;
	public Material redCapMaterial;
	private Material victimCutMaterial;

	public List<AudioClip> sliceSounds;

	public GameObject goopPrefab;
	public GameObject redGoopPrefab;
	public GameObject yellowGoopPrefab;
	private GameObject juiceGoop;


	void OnCollisionEnter(Collision collision)
	{

		GameObject victim = collision.collider.gameObject;
		Rigidbody victimRB = victim.GetComponent<Rigidbody>();
		if (!victimRB.useGravity)
        {
			GameManager.Instance.UpdateCutFruitForRecipe(victim);
			AudioSource audio = victim.GetComponent<AudioSource>();
			audio.clip = sliceSounds[Random.Range(0, sliceSounds.Count)];
			audio.Play();
		}

		if (victim.tag == "tomato")
		{
			victimCutMaterial = redCapMaterial;
			juiceGoop = redGoopPrefab;

		}
		else if (victim.tag == "cabbage")
		{
			victimCutMaterial = greenCapMaterial;
			juiceGoop = goopPrefab;

		}
		else if (victim.tag == "watermelon")
		{
			victimCutMaterial = redCapMaterial;
			juiceGoop = redGoopPrefab;

		}
		else if (victim.tag == "pumpkin")
		{
			victimCutMaterial = orangeCapMaterial;
			juiceGoop = yellowGoopPrefab;

		}
		else if (victim.tag == "cucumber")
		{
			victimCutMaterial = greenCapMaterial;
			juiceGoop = goopPrefab;

		}
		else if (victim.tag == "carrot")
		{
			victimCutMaterial = orangeCapMaterial;
			juiceGoop = yellowGoopPrefab;

		}
		else if (victim.tag == "banana")
		{
			victimCutMaterial = orangeCapMaterial;
			juiceGoop = yellowGoopPrefab;

		}
		else if (victim.tag == "beetroot")
		{
			victimCutMaterial = redCapMaterial;
			juiceGoop = redGoopPrefab;

		}
		else
		{
			victimCutMaterial = capMaterial;
			juiceGoop = redGoopPrefab;
		}

		GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, victimCutMaterial);

		if (!pieces[1].GetComponent<Rigidbody>())
			pieces[1].AddComponent<Rigidbody>();
		MeshCollider temp = pieces[1].AddComponent<MeshCollider>();
		temp.convex = true;

		foreach (GameObject piece in pieces)
		{
			Rigidbody rb = piece.GetComponent<Rigidbody>();
			rb.useGravity = true;
			rb.isKinematic = false;
		}
		GameObject goopParticle = Instantiate(juiceGoop, transform.position, transform.rotation);
		StartCoroutine(DeleteFruitPart(pieces, goopParticle));
	}

	IEnumerator DeleteFruitPart(GameObject[] pieces, GameObject goopParticle)
	{
		yield return new WaitForSeconds(1);
		Destroy(goopParticle);
		foreach (GameObject piece in pieces)
		{
			Destroy(piece);
		}
	}

}
