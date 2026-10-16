using UnityEngine;

public class TestMakeItemScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //테스트
        FirestoreManager.Instance.SaveItem_Test(FirebaseAuthManager.Instance.auth.CurrentUser.UserId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
