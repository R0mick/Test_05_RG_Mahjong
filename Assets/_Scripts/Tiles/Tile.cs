using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Tiles
{
    /// <summary>
    /// Represents tile on the scene.
    /// </summary>
    public class Tile : MonoBehaviour, ITile
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject tileBody;
        [SerializeField] private int tileId;    
        [SerializeField] private string tileName;    
        [SerializeField] private Vector3 boardPosition;    
        [SerializeField] private bool isOpen;
        
        private bool _isSelected;
        private Material _tileMaterial;
        private Color _defaultTileColor;
        private bool _isInputBlocked;

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;
                UpdateTileColor(isOpen);
            }
        }

        public int TileId
        {
            get => tileId;
            set => tileId = value;
        }

        public string TileName
        {
            get => tileName;
            set => tileName = value;
        }

        public Vector3 BoardPosition
        {
            get => boardPosition;
            set => boardPosition = value;
        }


        private void Awake()
        {
            _tileMaterial = tileBody.GetComponent<MeshRenderer>().material;
            _defaultTileColor = _tileMaterial.color;
            UpdateTileColor(isOpen);
        }

        private void OnEnable()
        {
            SimpleEventManager.Instance.OnUpdateInputBlockStatus += SetBlockInputStatus;
        }
        private void OnDisable()
        {
            SimpleEventManager.Instance.OnUpdateInputBlockStatus -= SetBlockInputStatus;
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        private void OnMouseDown()
        {
            if (_isInputBlocked)
            {
                Debug.Log("Input Blocked");
                return;
            }
            
            if (!isOpen)
            {
                Debug.Log("Tile isn't open!");
                return;
            }
            //Debug.Log(TileId);
            ToggleSelected();
            SimpleEventManager.Instance.TileClicked(this);
        }

        private void SetBlockInputStatus(bool isBlocked)
        {
            _isInputBlocked = isBlocked;
        }

        public void ToggleSelected()
        {
            _isSelected = !_isSelected;
            if (_isSelected)
            {
                _tileMaterial.color = Color.yellow;
            }
            else
            {
                _tileMaterial.color = Color.white;
            }
        }

        private void UpdateTileColor(bool isAvailable)
        {
            if (isAvailable)
            {
                _tileMaterial.color = _defaultTileColor;
            }
            else
            {
                _tileMaterial.color = new Color(0.7f,0.7f,0.7f);
            }
        }
    }
}
