
//using UnityEngine;

//namespace Sunbox.Avatars {

//    public class AvatarLocomotion : MonoBehaviour {
        
//        public float MovementAcceleration = 1f;
//        public float MovementDamping = 1f;

//        private AvatarCustomization _avatar;

//        public Vector2 _inputVector;
//        public Vector3 newLoc;

//        void Start()  {
//            _avatar = GetComponent<AvatarCustomization>();

//        }

//        void Update()  {
//            // fetch the location of the player
//            newLoc = DataBaseManager.instance.levelHandler.fetchPlayerLoc(gameObject);
//            float deltaX = newLoc.x - transform.position.x;
//            float deltaY = newLoc.z - transform.position.y;

//            // move avatar location
//            _inputVector.x = Mathf.MoveTowards(_inputVector.x, 0, Time.deltaTime * MovementDamping);
//            _inputVector.y = Mathf.MoveTowards(_inputVector.y, 0, Time.deltaTime * MovementDamping);

//            _inputVector.x += MovementAcceleration * Time.deltaTime * deltaX;
//            _inputVector.y += MovementAcceleration * Time.deltaTime * deltaY;

//            _inputVector.x = Mathf.Clamp(_inputVector.x, -1, 1);
//            _inputVector.y = Mathf.Clamp(_inputVector.y, -1, 1);

//            // animate movment
//            _avatar.Animator.SetFloat("MoveX", _inputVector.x);
//            _avatar.Animator.SetFloat("MoveY", _inputVector.y);
//        }

//        public void Dance(){
//            _avatar.Animator.SetTrigger("Dance01");
//        }

//        public void Wave(){
//            _avatar.Animator.SetTrigger("Wave");
//        }

//        public void Clap(){
//            _avatar.Animator.SetTrigger("Clap");
//        }

//        public void setNewLoc(Vector3 l)
//        {
//            newLoc = l;
//        }
//    }
//}
