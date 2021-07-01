using UnityEngine;

namespace _3D
{
    public class ReplaceObjectWithFractured : MonoBehaviour
    {
        public GameObject fracturedModel;
        // Start is called before the first frame update
        void Start()
        {
        }

        private void OnCollisionEnter(Collision other)
        {
            var velocity = gameObject.GetComponent<Rigidbody>().velocity;
            Destroy(gameObject.transform.parent.gameObject);
            var go = Instantiate(fracturedModel, transform.position, transform.rotation);
            foreach (var rb in go.GetComponentsInChildren<Rigidbody>())
            {
                rb.velocity = velocity;
            }
        }
    }
}
