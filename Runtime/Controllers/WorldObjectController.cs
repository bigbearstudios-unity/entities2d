using UnityEngine;

public class WorldObjectController : MonoBehaviour {
    enum OnDeathActions {
        Nothing
    }

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Trigger me");
        _animator.Play("Death");
    }
    

    private void OnAnimationEvent() {
        _rigidbody.simulated = false;   
    }
}
