
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MoveFarmAnimalsWithClicks : MonoBehaviour
    {
        [SerializeField]
        private RuntimeSetFarmAnimal targets = default;
        

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0;
                
                MoveAllTargetsTo(targetPosition);
            }
        }

        private void MoveAllTargetsTo(Vector3 position)
        {
            targets.ForEach(farmAnimal =>
            {
                farmAnimal.MoveTo(position + (Vector3)Random.insideUnitCircle);
            });
        }
    }
}
