using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---14/05/2017---
public class Drive : MonoBehaviour {

	float speed = 20.0F;        //velocidade de movimento
    float rotationSpeed = 120.0F;  //velocidadede rotação
    public GameObject bulletPrefab;   //seta o prefab dos tiros
    public Transform bulletSpawn;       // local de onde o tiro sai

    void Update() {
        float translation = Input.GetAxis("Vertical") * speed;  //botões que movem o tanque para frente e para trás
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;   //botões que fazem a rotação do tanque
        translation *= Time.deltaTime;  
        rotation *= Time.deltaTime;     
        transform.Translate(0, 0, translation);     //movimentação do tanque
        transform.Rotate(0, rotation, 0);       // rotação do tanque

        if(Input.GetKeyDown("space")) // aperte espaço para disparar os projeteis
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
