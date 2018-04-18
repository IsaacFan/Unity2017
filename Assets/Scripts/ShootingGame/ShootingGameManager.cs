//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGameManager : MonoBehaviour {

    /*
    private static ShootingGameManager instance;
    public static ShootingGameManager Instace
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = GameObject.Find("ShootingGame");
                if (gameObject == null)
                {
                    gameObject = new GameObject("ShootingGame");
                    instance = gameObject.AddComponent<ShootingGameManager>();
                }
                else
                {
                    instance = gameObject.GetComponent<ShootingGameManager>();
                    if (instance == null)
                        instance = gameObject.AddComponent<ShootingGameManager>();
                }
            }

            return instance;
        }
    }
    */

    public PlayerController playerController;
    public Transform gunFrontPoint;
    private int playerHeartNumber;
    private int skillNumber;

    private Timer enemyGeneratorTimer;
    public GameObject originalEnemy;
    private List<EnemyController> enemyList = new List<EnemyController>();
    private List<EnemyController> recycledEnemyPool = new List<EnemyController>();

    public GameObject originalBullet;
    private List<BulletController> bulletList = new List<BulletController>();
    private List<BulletController> recycledBulletPool = new List<BulletController>();

    //private bool isGaming;


    // Use this for initialization
    void Start () {
        enemyGeneratorTimer = TimerManager.Instace.createTimer(-1, 1f, 5f, addOneEnemy);
        enemyGeneratorTimer.isPause = true;

        playerController.init(deleteEnemy, addOneBullet, decreaseOneHeart, useSkill);

        EnemyController enemyController = originalEnemy.GetComponent<EnemyController>();
        if (enemyController == null)
            enemyController = gameObject.AddComponent<EnemyController>();
        enemyController.init(playerController.transform);
        enemyList.Add(enemyController);

        BulletController bulletController = originalBullet.GetComponent<BulletController>();
        if (bulletController == null)
            bulletController = originalBullet.AddComponent<BulletController>();
        bulletController.init(deleteEnemy, deleteBullet);
        bulletController.invisibleBullet();
        recycledBulletPool.Add(bulletController);

        initGame();
    }
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < enemyList.Count; i++)
            enemyList[i].update();

    }
    

    private void initGame()
    {
        enemyGeneratorTimer.resetTimeLimit();
        enemyGeneratorTimer.isPause = false;

        playerHeartNumber = 3;
        CUIShootingGame.Instance.setHeartNumber(playerHeartNumber);
        skillNumber = 1;
        CUIShootingGame.Instance.setSkillNumber(skillNumber);

        playerController.setActive(true);
        //isGaming = true;

        AudioManager.Instance.playMusic("Mirage");
    }
    private void stopGame()
    {
        enemyGeneratorTimer.isPause = true;      
        playerController.setActive(false);
        //isGaming = false;

        AudioManager.Instance.fadeOutMusic(3f);
    }

    private void addOneEnemy()
    {
        EnemyController enemyController;
        if (recycledEnemyPool.Count > 0)
        {
            int index = recycledEnemyPool.Count - 1;
            enemyController = recycledEnemyPool[index];         
            recycledEnemyPool.RemoveAt(index);
        }
        else
        {
            GameObject gameObject = Instantiate(originalEnemy, originalEnemy.transform.parent);
            enemyController = gameObject.GetComponent<EnemyController>();
            if (enemyController == null)
                enemyController = gameObject.AddComponent<EnemyController>();
            enemyController.init(playerController.transform);    
        }

        enemyList.Add(enemyController);
        enemyController.switchEnemyVisible(true);
    }
    private void deleteEnemy(GameObject gameObject)
    {
        EnemyController enemyController = gameObject.GetComponent<EnemyController>();
        if (enemyController != null)
            enemyList.Remove(enemyController);
        else
            enemyController = gameObject.AddComponent<EnemyController>();

        enemyController.switchEnemyVisible(false);
        recycledEnemyPool.Add(enemyController);
    }

    private void addOneBullet()
    {
        BulletController bulletController;
        if (recycledBulletPool.Count > 0)
        {
            int index = recycledBulletPool.Count - 1;
            bulletController = recycledBulletPool[index];
            recycledBulletPool.RemoveAt(index);
        }
        else
        {
            GameObject gameObject = Instantiate(originalBullet, originalBullet.transform.parent);
            bulletController = gameObject.GetComponent<BulletController>();
            if (bulletController == null)
                bulletController = gameObject.AddComponent<BulletController>();
            bulletController.init(deleteEnemy, deleteBullet);
        }

        bulletList.Add(bulletController);
        bulletController.shootBullet(gunFrontPoint.position, playerController.transform.forward);
    }
    private void deleteBullet(GameObject gameObject)
    {
        BulletController bulletController = gameObject.GetComponent<BulletController>();
        if (bulletController != null)
            bulletList.Remove(bulletController);
        else
            bulletController = gameObject.AddComponent<BulletController>();

        bulletController.invisibleBullet();
        recycledBulletPool.Add(bulletController);
    }

    private void decreaseOneHeart()
    {
        playerHeartNumber--;
        CUIShootingGame.Instance.setHeartNumber(playerHeartNumber);

        if (playerHeartNumber <= 0)
            stopGame();
    }

    private void useSkill()
    {
        if (skillNumber <= 0)
            return;

        skillNumber--;
        CUIShootingGame.Instance.setSkillNumber(skillNumber);

        for (int i = enemyList.Count - 1; i >= 0; i--)
            deleteEnemy(enemyList[i].gameObject);

        AudioManager.Instance.playSound("Crash");
    }

}
