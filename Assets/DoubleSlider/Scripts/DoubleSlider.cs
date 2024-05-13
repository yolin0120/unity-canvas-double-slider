#region Includes
using UnityEngine;
using UnityEngine.Events;
#endregion

namespace TS.DoubleSlider
{
    [RequireComponent(typeof(RectTransform))]
    public class DoubleSlider : MonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] private SingleSlider _sliderMin;
        [SerializeField] private SingleSlider _sliderMax;
        [SerializeField] private RectTransform _fillArea;

        [Header("Configuration")]
        [SerializeField] private bool _setupOnStart;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private float _minDistance;
        [SerializeField] private float _maxDistance;
        [SerializeField] private bool _wholeNumbers;
        [SerializeField] private float _initialMinValue;
        [SerializeField] private float _initialMaxValue;

        [Header("Events")]
        public UnityEvent<float, float> OnValueChanged;

        public bool IsEnabled
        {
            get { return _sliderMax.IsEnabled && _sliderMin.IsEnabled; }
            set
            {
                _sliderMin.IsEnabled = value;
                _sliderMax.IsEnabled = value;
            }
        }
        public float MinValue
        {
            get { return _sliderMin.Value; }
        }
        public float MaxValue
        {
            get { return _sliderMax.Value; }
        }
        public bool WholeNumbers
        {
            get { return _wholeNumbers; }
            set
            {
                _wholeNumbers = value;

                _sliderMin.WholeNumbers = _wholeNumbers;
                _sliderMax.WholeNumbers = _wholeNumbers;
            }
        }

        private RectTransform _fillRect;

        #endregion

        private void Awake()
        {
            if (_sliderMin == null || _sliderMax == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

                Debug.LogError("Missing slider min: " + _sliderMin + ", max: " + _sliderMax);
#endif
                return;
            }

            if (_fillArea == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

                Debug.LogError("Missing fill area");
#endif
                return;
            }

            _fillRect = _fillArea.transform.GetChild(0).transform as RectTransform;
        }
        private void Start()
        {
            if (!_setupOnStart) 
                return;
            Setup(_minValue, _maxValue, _initialMinValue, _initialMaxValue, _minDistance, _maxDistance);
        }

        public void Setup(float minValue, float maxValue, float initialMinValue, float initialMaxValue, float minDistance, float maxDistance)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _initialMinValue = initialMinValue;
            _initialMaxValue = initialMaxValue;
            _minDistance = minDistance;
            _maxDistance = maxDistance;
        
            _sliderMin.WholeNumbers = _wholeNumbers;
            _sliderMax.WholeNumbers = _wholeNumbers;
        
            _sliderMin.Setup(_initialMinValue, minValue, maxValue, MinValueChanged);
            _sliderMax.Setup(_initialMaxValue, minValue, maxValue, MaxValueChanged);
    
            MinValueChanged(_initialMinValue);
            MaxValueChanged(_initialMaxValue);
        }

        private void MinValueChanged(float value)
        {
            float offset = ((MinValue - _minValue) / (_maxValue - _minValue)) * _fillArea.rect.width;
            _fillRect.offsetMin = new Vector2(offset, _fillRect.offsetMin.y);

            if ((MaxValue - value) < _minDistance)
            {
                float fixValue = value + _minDistance;
                if (fixValue > _maxValue)
                {
                    _sliderMax.Value = _maxValue;
                    _sliderMin.Value = _maxValue - _minDistance;
                }
                else
                {
                    _sliderMax.Value = value + _minDistance;
                    _sliderMin.Value = MaxValue - _minDistance;
                }
            }

            if ((MaxValue - value) > _maxDistance)
            {
                _sliderMax.Value = value + _maxDistance;
                _sliderMin.Value = Mathf.Clamp(MaxValue - _maxDistance, _minValue, _maxValue);
            }

            OnValueChanged.Invoke(MinValue, MaxValue);
        }
        private void MaxValueChanged(float value)
        {
            float offset = (1 - ((MaxValue - _minValue) / (_maxValue - _minValue))) * _fillArea.rect.width;
            _fillRect.offsetMax = new Vector2(-offset, _fillRect.offsetMax.y);

            if ((value - MinValue) < _minDistance)
            {
                float fixValue = value - _maxDistance;
                if (fixValue < _minValue)
                {
                    _sliderMin.Value = value - _minDistance;
                    _sliderMax.Value = _minValue + _minDistance;
                }
                else
                {
                    _sliderMin.Value = _minValue;
                    _sliderMax.Value = MinValue + _minDistance;
                }
            }    

            if ((value - MinValue) > _maxDistance)
            {
                _sliderMin.Value = value - _maxDistance;
                _sliderMax.Value = Mathf.Clamp(MinValue + _maxDistance, _minValue, _maxValue);
            }
            
            OnValueChanged.Invoke(MinValue, MaxValue);
        }
    }
}
